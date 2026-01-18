using UnityEngine;

[System.Serializable]
public class BurnEffect : ISideEffect
{
    [SerializeField] public float damagePerTick = 8f;
    [SerializeField] public int duration = 2;
    
    public void Apply(Minion target, Minion attacker)
    {
        StatusEffectManager effectManager = target.GetComponent<StatusEffectManager>();
        if (effectManager != null)
        {
            effectManager.ApplyBurn(damagePerTick, duration);
        }
    }
    
    public string GetDescription()
    {
        return $"Burn: {damagePerTick} damage/turn for {duration} turns";
    }
}
