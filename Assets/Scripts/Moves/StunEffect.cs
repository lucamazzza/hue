using UnityEngine;

[System.Serializable]
public class StunEffect : ISideEffect
{
    [SerializeField] public int duration = 1;
    
    public void Apply(Minion target, Minion attacker)
    {
        StatusEffectManager effectManager = target.GetComponent<StatusEffectManager>();
        if (effectManager != null)
        {
            effectManager.ApplyStun(duration);
        }
    }
    
    public string GetDescription()
    {
        return $"Stun: Target cannot move for {duration} turn(s)";
    }
}
