using UnityEngine;

public class WinHandlers : MonoBehaviour
{
    public void OnContinueClicked()
    {
        BattleLauncher.ExitBattle(BattleLauncher.nextSceneAfterWin);
    }
}