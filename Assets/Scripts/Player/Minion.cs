using UnityEngine;

public class Minion : MonoBehaviour
{
    public MinionStatsSO stats;
    public MoveSO[] availableMoves;

    private float _currentHealth;
    private MinionAttack _minionAttack;
    private bool _isPlayerOwned;

    public bool IsPlayerOwned => _isPlayerOwned;
    public bool IsAlive => _currentHealth > 0;
    public float AttackDamage
    {
        get
        {
            float attackModifier = 0f;
            StatusEffectManager statusManager = GetComponent<StatusEffectManager>();
            if (statusManager != null)
                attackModifier = statusManager.GetAttackModifier();

            return stats.attack + attackModifier;
        }
    }
    public MoveSO[] AvailableMoves => availableMoves;
    public float CurrentHealth => _currentHealth;
    public float PercentHealth => _currentHealth / stats.maxHealth;

    void Start()
    {
        _currentHealth = stats.maxHealth;
        _minionAttack = GetComponent<MinionAttack>();

        if (availableMoves == null || availableMoves.Length == 0)
        {
            availableMoves = new MoveSO[4];
            availableMoves[0] = stats.attackMove;
            availableMoves[1] = stats.defendMove;
            availableMoves[2] = stats.specialMove;
            availableMoves[3] = stats.fleeMove;
        }
    }

    public void SetOwnership(bool isPlayerOwned)
    {
        _isPlayerOwned = isPlayerOwned;
    }

    public bool TakeDamage(float damage)
    {
        float defenseModifier = 0f;
        StatusEffectManager statusManager = GetComponent<StatusEffectManager>();
        if (statusManager != null)
            defenseModifier = statusManager.GetDefenseModifier();

        float totalDefense = stats.defence + defenseModifier;
        float actualDamage = Mathf.Max(0, damage - totalDefense);

        _currentHealth -= actualDamage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, stats.maxHealth);

        SoundFXManager.instance.PlayRandomSoundFXClip(stats.damageSoundClips, transform, 1f);
        Debug.Log($"{name} took {actualDamage} damage (Defense: {totalDefense})");
        bool isAlive = _currentHealth > 0;

        if (!isAlive)
        {
            Debug.Log($"{name} has been defeated!");
            Kill();
        }

        return isAlive;
    }

    public void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, stats.maxHealth);
    }

    public void Attack(Minion target)
    {
        if (_minionAttack != null)
            _minionAttack.ExecuteAttack(target);
    }

    public void Kill()
    {
        _currentHealth = 0;

        Vector3 deathPosition = transform.position;
        deathPosition.y = 0.1f;

        Vector3 deathRotation = transform.eulerAngles;
        deathRotation.z = -90f;

        transform.position = deathPosition;
        transform.eulerAngles = deathRotation;
    }

    public void Revive()
    {
        _currentHealth = stats.maxHealth;

        Vector3 revivePosition = transform.position;
        revivePosition.y = 0.5f;

        Vector3 reviveRotation = transform.eulerAngles;
        reviveRotation.z = 0f;

        transform.position = revivePosition;
        transform.eulerAngles = reviveRotation;
    }
}
