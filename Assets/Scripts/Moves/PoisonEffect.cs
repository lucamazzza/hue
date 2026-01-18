using UnityEngine;

[System.Serializable]
public class PoisonEffect : ISideEffect
{
    [SerializeField] public float damagePerTick = 5f;
    [SerializeField] public int duration = 3;
    
    public void Apply(Minion target, Minion attacker)
    {
        StatusEffectManager effectManager = target.GetComponent<StatusEffectManager>();
        if (effectManager != null)
        {
            effectManager.ApplyPoison(damagePerTick, duration);
        }
    }
    
    public string GetDescription()
    {
        return $"Poison: {damagePerTick} damage/turn for {duration} turns";
    }
}
