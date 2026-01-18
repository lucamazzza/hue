using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(0)]
public class CombatManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _minionLayer;
    [SerializeField] private MoveSO _defaultMove;
    [SerializeField] private GameObject indicator;

    [Header("Scene Navigation")]
    [Tooltip("Name of the scene to return after winning or fleeing")]
    [SerializeField] private string _overworldSceneName;
    [Tooltip("Name of the scene to return after losing")]
    [SerializeField] private string _gameOverSceneName = "GameOver";
    [Tooltip("Name of the scene to return after winning")]
    [SerializeField] private string _winSceneName = "WinScene";

    private SpecialMoveExecutor _specialMoveExecutor;
    private Minion _selectedMinion;
    private Minion _targetMinion;
    private MoveSO _selectedMove;
    private CombatState _currentState = CombatState.SelectingMinion;
    protected List<Minion> _allMinions = new List<Minion>();
    private PQueue<MinionMoveState> _moveQueue = new PQueue<MinionMoveState>(new SpeedMaxComparer());
    private List<Minion> _playerMinions = new List<Minion>();
    private List<Minion> _enemyMinions = new List<Minion>();
    private List<Minion> _possibleTargets = new List<Minion>();
    private int _movesQueued = 0;
    private int _selectedMoveIndex = 0;

    private int _currentSelectedMinionIndex = 0;
    private int _currentSelectedTargetMinionIndex = 0;

    private enum CombatState
    {
        SelectingMinion,
        SelectingMove,
        SelectingTarget,
        ExecutingMoves,
        Waiting
    }

    protected virtual void Start()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_specialMoveExecutor == null)
            _specialMoveExecutor = this.GetComponent<SpecialMoveExecutor>();

        // NOTE: Delay refresh to ensure MinionSetup has run first
        Invoke(nameof(RefreshMinionList), 0.1f);

        if (_defaultMove == null)
            Debug.LogWarning("No default move assigned! Please assign a MoveSO in the inspector.");
    }

    protected virtual void Update()
    {
        if (_currentState == CombatState.ExecutingMoves)
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            HandleMouseClick();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            CancelSelection();

        if (Keyboard.current != null && Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            _currentSelectedTargetMinionIndex--;

            if (_currentSelectedTargetMinionIndex < 0)
                _currentSelectedTargetMinionIndex = _possibleTargets.Count() - 1;
        }

        if (Keyboard.current != null && Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            _currentSelectedTargetMinionIndex++;
            _currentSelectedTargetMinionIndex %= _possibleTargets.Count();
        }

        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Debug.Log("Selected target");
            SelectTarget(_possibleTargets[_currentSelectedTargetMinionIndex]);
            _currentSelectedMinionIndex++;
            Debug.Log($"Current selected minion index: {_currentSelectedMinionIndex}; {_playerMinions.Count}");
            _currentSelectedMinionIndex %= _playerMinions.Count();
        }

        switch (_currentState)
        {
            case CombatState.SelectingMinion:
                if (_playerMinions.Count == 0)
                {
                    Debug.Log("No player minions");
                    int enemyCount = _enemyMinions.Count;
                    Debug.Log("Enemy: " + enemyCount);
                    break;
                }

                if (_moveQueue.Count >= _playerMinions.Count)
                {
                    StartCoroutine(ExecuteAllMoves());
                    break;
                }

                SelectMinion(_playerMinions[_currentSelectedMinionIndex % _playerMinions.Count()]);
                break;

            case CombatState.SelectingTarget:
                if (_possibleTargets.Count == 0)
                {
                    Debug.Log("No target minions");
                    int targetCount = _possibleTargets.Count;
                    Debug.Log("Target: " + targetCount);
                    break;
                }

                ShowMinionIndicator(_possibleTargets[_currentSelectedTargetMinionIndex]);
                break;

            case CombatState.SelectingMove:
                HandleMoveSelection();
                break;

            case CombatState.Waiting:
                break;
        }
    }

    private void ShowMinionIndicator(Minion minion)
    {
        if (minion == null) return;

        Collider collider = minion.GetComponent<MeshCollider>();
        Bounds bounds = collider.bounds;

        float max_height = bounds.max.y;
        Vector3 indicator_position = indicator.transform.position;

        indicator_position.y = max_height + 1.0f;
        indicator_position.x = bounds.center.x;
        indicator_position.z = bounds.center.z;

        indicator.transform.position = indicator_position;
    }

    private void HandleMouseClick()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _minionLayer))
        {
            Minion clickedMinion = hit.collider.GetComponent<Minion>();

            if (clickedMinion != null && clickedMinion.IsAlive)
            {
                switch (_currentState)
                {
                    case CombatState.SelectingMinion:
                        SelectMinion(clickedMinion);
                        break;

                    case CombatState.SelectingTarget:
                        SelectTarget(clickedMinion);
                        break;
                }
            }
        }
    }

    private void SelectMinion(Minion minion)
    {
        if (!minion.IsPlayerOwned)
        {
            Debug.Log("This minion is not owned by the player!");
            return;
        }
        _selectedMinion = minion;
        if (_selectedMinion.AvailableMoves != null && _selectedMinion.AvailableMoves.Length > 0)
        {
            _currentState = CombatState.SelectingMove;
            _selectedMoveIndex = 0;
            Debug.Log($"Selected minion: {minion.name}. Choose a move (1-{_selectedMinion.AvailableMoves.Length})");
        }
        else
        {
            _selectedMove = _defaultMove;
            _currentState = CombatState.SelectingTarget;
            Debug.Log($"Selected minion: {minion.name}. Now select a target to attack.");
        }
        ShowMinionIndicator(_selectedMinion);
    }

    private void HandleMoveSelection()
    {
        if (Keyboard.current == null) return;

        int moveCount = _selectedMinion.AvailableMoves.Length;

        for (int i = 0; i < Mathf.Min(moveCount, 9); i++)
        {
            if (Keyboard.current[(Key)(Key.Digit1 + i)].wasPressedThisFrame)
            {
                _selectedMoveIndex = i;
                _selectedMove = _selectedMinion.AvailableMoves[i];
                _currentState = CombatState.SelectingTarget;
                Debug.Log($"Selected move: {_selectedMove.name}. Now select a target.");
                return;
            }
        }
    }

    private void SelectTarget(Minion target)
    {
        _targetMinion = target;
        MoveSO moveToUse = _selectedMove != null ? _selectedMove : _defaultMove;
        EnqueueMove(_selectedMinion, _targetMinion, moveToUse);
        ResetSelection();
    }

    private void EnqueueMove(Minion attacker, Minion target, MoveSO move)
    {
        MinionMoveState moveState = new MinionMoveState(attacker, target, move);
        _moveQueue.Enqueue(moveState);
        _movesQueued++;
        Debug.Log($"Move queued: {attacker.name} -> {target.name} (Speed: {moveState.Speed}). Total queued: {_movesQueued}/{_playerMinions.Count}");
    }

    public void EnqueueAttack()
    {
        Debug.Log("Attack move enqueued");

        if (_selectedMinion == null)
        {
            Debug.LogWarning("No minion selected!");
            return;
        }

        if (_selectedMinion.AvailableMoves.Length < 1)
        {
            Debug.LogWarning($"{_selectedMinion.name} doesn't have any moves assigned!");
            return;
        }

        _selectedMove = _selectedMinion.AvailableMoves[0];
        _currentState = CombatState.SelectingTarget;
        _possibleTargets = _enemyMinions;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void EnqueueDefend()
    {
        Debug.Log("Defend move enqueued");

        if (_selectedMinion.AvailableMoves.Length < 2) return;

        if (_selectedMinion.AvailableMoves.Length < 2)
        {
            Debug.LogWarning($"{_selectedMinion.name} doesn't have a defend move assigned (needs at least 2 moves)!");
            return;
        }

        _selectedMove = _selectedMinion.AvailableMoves[1];
        _currentState = CombatState.SelectingTarget;
        _currentSelectedTargetMinionIndex = _playerMinions.IndexOf(_selectedMinion);
        _possibleTargets = _playerMinions;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void EnqueueSpecial()
    {
        Debug.Log("Special move enqueued");

        if (_selectedMinion.AvailableMoves.Length < 3) return;

        if (_selectedMinion.AvailableMoves.Length < 3)
        {
            Debug.LogWarning($"{_selectedMinion.name} doesn't have a special move assigned (needs at least 3 moves)!");
            return;
        }

        _selectedMove = _selectedMinion.AvailableMoves[2];

        if (_selectedMove is SpecialMoveSO specialMove)
        {
            Debug.Log($"Special move selected: {specialMove.moveName} (x{specialMove.damageMultiplier} damage)");
        }
        else
        {
            Debug.LogWarning($"Move at index 2 is not a SpecialMoveSO! It's a {_selectedMove.GetType().Name}. Special moves will use basic attack instead.");
        }

        _currentState = CombatState.SelectingTarget;
        _possibleTargets = _enemyMinions;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void EnqueueFlee()
    {
        Debug.Log("Flee move enqueued");
        if (_selectedMinion == null)
        {
            Debug.LogWarning("No minion selected!");
            return;
        }
        if (_selectedMinion.AvailableMoves.Length < 4)
        {
            Debug.LogWarning($"{_selectedMinion.name} doesn't have a flee move assigned (needs at least 4 moves)!");
            return;
        }

        _selectedMove = _selectedMinion.AvailableMoves[3];
        _currentState = CombatState.SelectingTarget;

        MoveSO moveToUse = _selectedMove != null ? _selectedMove : _defaultMove;
        EnqueueMove(_selectedMinion, _selectedMinion, moveToUse);
        ResetSelection();

        _currentSelectedMinionIndex++;
        _currentSelectedMinionIndex %= _playerMinions.Count();

        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator ExecuteAllMoves()
    {
        indicator.SetActive(false);
        ProcessEnemyTurn();
        _currentState = CombatState.ExecutingMoves;
        Debug.Log($"Executing {_moveQueue.Count} moves in speed order...");

        while (_moveQueue.Count > 0)
        {
            MinionMoveState move = _moveQueue.Dequeue();

            if (move.Attacker == null || !move.Attacker.IsAlive)
            {
                Debug.Log($"Attacker is dead or null, skipping move.");
                continue;
            }

            if (move.Target == null || !move.Target.IsAlive)
            {
                Debug.Log($"{move.Attacker.name}'s target is dead, skipping move.");
                continue;
            }

            Debug.Log($"{move.Attacker.name} attacks {move.Target.name} with speed {move.Speed}!");
            StatusEffectManager statusManager = move.Attacker.GetComponent<StatusEffectManager>();
            if (statusManager != null && statusManager.IsStunned())
            {
                Debug.Log($"{move.Attacker.name} is stunned and cannot move!");
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            switch (move.ActiveMove.moveType)
            {
                case MoveType.AttackMove:
                    move.Attacker.Attack(move.Target);
                    break;

                case MoveType.DefenseMove:
                    move.Target.Heal(move.Defence);
                    break;

                case MoveType.SpecialMove:
                    if (move.ActiveMove is SpecialMoveSO specialMove && _specialMoveExecutor != null)
                        yield return StartCoroutine(_specialMoveExecutor.ExecuteSpecialMove(specialMove, move.Attacker, move.Target));
                    else
                    {
                        Debug.LogWarning("Special move not properly configured, using basic attack");
                        move.Attacker.Attack(move.Target);
                    }
                    break;

                case MoveType.FleeMove:
                    TryToFlee(move.Attacker.stats.luck);
                    break;
            }

            // Wait for attack animation to complete (only for non-special moves)
            if (move.ActiveMove.moveType != MoveType.SpecialMove)
            {
                MinionAttack attackComponent = move.Attacker.GetComponent<MinionAttack>();
                if (attackComponent != null)
                {
                    while (attackComponent.IsAttacking)
                    {
                        yield return null;
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Processing end-of-turn status effects for all minions...");
        ProcessAllStatusEffects();
        _movesQueued = 0;
        Debug.Log("All moves executed! Select minions for next round.");
        InitNewTurn();
    }

    private void InitNewTurn()
    {
        // Remove deads mininons
        List<Minion> _deadMinions = _allMinions
            .Where(minion => !minion.IsAlive)
            .ToList();

        _deadMinions.ForEach(minion =>
        {
            if (minion.IsPlayerOwned)
                _playerMinions.Remove(minion);
            else
                _enemyMinions.Remove(minion);

            _allMinions.Remove(minion);
        });

        if (_enemyMinions.Count == 0)
        {
            Debug.Log("Victory! All enemy minions defeated.");
            EndBattle(true);
            return;
        }
        else if (_playerMinions.Count == 0)
        {
            Debug.Log("Defeat! All player minions defeated.");
            EndBattle(false);
            return;
        }

        // Reset selection indices
        _currentSelectedMinionIndex = 0;

        if (_currentSelectedTargetMinionIndex >= _enemyMinions.Count)
        {
            _currentSelectedTargetMinionIndex = _enemyMinions.Count - 1;
        }

        _currentState = CombatState.SelectingMinion;
        indicator.SetActive(true);
    }

    private void EndBattle(bool playerWon) 
    {
        if (playerWon)
            BattleWon();
        else
            BattleLost();
    }

    protected virtual void BattleWon()
    {
        GiveReward();

        if (BattleLauncher.isBossBattle)
        {
            BattleLauncher.nextSceneAfterWin = _overworldSceneName;
        }
        else
        {
            BattleLauncher.nextSceneAfterWin = BattleLauncher.lastOverworldScene;
        }
        BattleLauncher.ExitBattle(_winSceneName);
    }

    protected virtual void BattleLost()
    {
        BattleLauncher.lastBattleScene = SceneManager.GetActiveScene().name;
        BattleLauncher.ExitBattle(_gameOverSceneName);
    }

    protected virtual void GiveReward()
    {
        if (BattleLauncher.rewardMinion != null)
            PartyManager.Instance.UnlockMinion(BattleLauncher.rewardMinion);
    }


    private void ProcessAllStatusEffects()
    {
        foreach (Minion minion in _allMinions)
        {
            if (minion != null && minion.IsAlive)
            {
                StatusEffectManager statusManager = minion.GetComponent<StatusEffectManager>();
                if (statusManager != null)
                    statusManager.ProcessTurnEffects();
            }
        }
    }

    private void ProcessEnemyTurn()
    {
        Debug.Log("ProcessEnemyTurn");
        foreach (Minion minion in _enemyMinions)
        {
            EnemyAI ai = minion.GetComponent<EnemyAI>();
            if (ai != null)
            {
                MinionMoveState state = ai.DecideAction(_playerMinions, _enemyMinions);
                Debug.Log("ProcessEnemyTurn: equeued: " + state.ActiveMove.moveName);
                _moveQueue.Enqueue(state);
                _movesQueued++;
            }
            else
                Debug.LogError("ProcessEnemyTurn: No EnemyAI attached");
        }
    }

    private void TryToFlee(float luck)
    {
        int successRate = Random.Range(0, 100);

        if (successRate < luck * 20)
        {
            Debug.Log("Flee successful");
            // Back to overworld
            BattleLauncher.ExitBattle();
        }
        else
        {
            Debug.Log("Flee unsuccessful");
        }
    }

    private void CancelSelection()
    {
        if (_currentState == CombatState.ExecutingMoves) return;
        ResetSelection();
        Debug.Log("Selection cancelled.");
    }

    private void ResetSelection()
    {
        _selectedMinion = null;
        _targetMinion = null;
        _selectedMove = null;
        _selectedMoveIndex = 0;
        _currentState = CombatState.SelectingMinion;
    }

    protected void RefreshMinionList()
    {
        _allMinions.Clear();
        _playerMinions.Clear();
        _enemyMinions.Clear();

        _allMinions.AddRange(FindObjectsByType<Minion>(FindObjectsSortMode.InstanceID));

        foreach (var minion in _allMinions)
        {
            if (minion.IsPlayerOwned)
                _playerMinions.Add(minion);
            else
                _enemyMinions.Add(minion);
        }

        Debug.Log($"Found {_playerMinions.Count} player minions and {_enemyMinions.Count} enemy minions.");
    }

    public void SetMinionOwnership(Minion minion, bool isPlayerOwned)
    {
        minion.SetOwnership(isPlayerOwned);
    }

    void EnqueueMove(MinionMoveState move)
    {
        _moveQueue.Enqueue(move);
    }
}
