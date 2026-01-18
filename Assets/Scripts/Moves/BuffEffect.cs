using UnityEngine;

[System.Serializable]
public class BuffEffect : ISideEffect
{
    [SerializeField] public float attackBoost = 10f;
    [SerializeField] public float defenseBoost = 5f;
    [SerializeField] public int duration = 2;
    [SerializeField] public bool buffSelf = true;
    
    public void Apply(Minion target, Minion attacker)
    {
        Minion buffTarget = buffSelf ? attacker : target;
        StatusEffectManager effectManager = buffTarget.GetComponent<StatusEffectManager>();
        if (effectManager != null)
        {
            effectManager.ApplyBuff(attackBoost, defenseBoost, duration);
        }
    }
    
    public string GetDescription()
    {
        return $"Buff: +{attackBoost} ATK, +{defenseBoost} DEF for {duration} turns on {(buffSelf ? "self" : "target")}";
    }
}
