using UnityEngine;

[System.Serializable]
public class HealEffect : ISideEffect
{
    [SerializeField] public float healAmount = 20f;
    [SerializeField] public bool healSelf = true;
    
    public void Apply(Minion target, Minion attacker)
    {
        Minion healTarget = healSelf ? attacker : target;
        if (healTarget != null)
        {
            healTarget.Heal(healAmount);
        }
    }
    
    public string GetDescription()
    {
        return $"Heal: Restores {healAmount} HP to {(healSelf ? "self" : "target")}";
    }
}
