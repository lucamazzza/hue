using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BattleLauncher
{
    public static MinionStatsSO rewardMinion { get; private set; }
    public static List<MinionStatsSO> enemySquad { get; private set; }
    public static EnemyDifficultySO difficulty { get; private set; }
    public static bool isBossBattle { get; private set; }

    public static string lastOverworldScene { get; private set; } 
    public static string lastBattleScene { get; set; }
    public static string nextSceneAfterWin { get; set; }

    public static void PrepareBattle(string battleScene, MinionStatsSO reward, List<MinionStatsSO> squad, EnemyDifficultySO diff, bool boss)
    {
        ResetBattleData();

        if (squad == null || squad.Count == 0 || diff == null)
        {
            Debug.LogError("BattleLauncher: Invalid squad or difficulty provided.");
            return;
        }

        rewardMinion = reward;
        difficulty = diff;
        isBossBattle = boss;

        lastOverworldScene = SceneManager.GetActiveScene().name;
        lastBattleScene = battleScene;

        if (squad.Count > 4)
        {
            Debug.LogWarning("BattleLauncher: Squad size exceeds 4. Truncating list.");
            enemySquad = squad.GetRange(0, 4);
        }
        else
        {
            enemySquad = new List<MinionStatsSO>(squad);
        }

        SceneManager.LoadScene(battleScene);
    }

    public static void ReprepareBattle(string battleScene)
    {
        if (enemySquad == null || enemySquad.Count == 0 || difficulty == null)
        {
            Debug.LogError("BattleLauncher: Cannot re-prepare battle. Missing squad or difficulty.");
            return;
        }

        SceneManager.LoadScene(battleScene);
    }

    public static void ExitBattle()
    {
        if (!string.IsNullOrEmpty(lastOverworldScene))
        {
            SceneManager.LoadScene(lastOverworldScene);
        }
        else
        {
            Debug.LogError("BattleLauncher: lastOverworldScene is empty!");
        }
    }

    public static void ExitBattle(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            ExitBattle();
            return;
        }
            
        SceneManager.LoadScene(sceneName);
    }

    public static void ResetBattleData()
    {
        rewardMinion = null;
        enemySquad = null;
        difficulty = null;
        nextSceneAfterWin = null; // Puliamo anche la scena successiva
    }
}