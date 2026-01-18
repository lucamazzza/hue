using UnityEngine;

public class GameOverHandlers : MonoBehaviour
{
    public void OnRetryClicked()
    {
        BattleLauncher.ReprepareBattle(BattleLauncher.lastBattleScene);
    }

    public void OnExitClicked()
    {
        BattleLauncher.ExitBattle();
    }
}