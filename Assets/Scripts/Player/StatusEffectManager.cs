using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    private List<StatusEffect> activeEffects = new List<StatusEffect>();
    private Minion minion;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject poisonVFX;
    [SerializeField] private GameObject burnVFX;
    [SerializeField] private GameObject stunVFX;
    [SerializeField] private GameObject buffVFX;
    
    private Dictionary<string, GameObject> activeVFX = new Dictionary<string, GameObject>();
    
    void Awake()
    {
        minion = GetComponent<Minion>();
    }
    
    public void ApplyPoison(float damagePerTick, int duration)
    {
        RemoveEffectByType("Poison");
        activeEffects.Add(new PoisonStatusEffect(damagePerTick, duration));
        SpawnEffectVFX("Poison", poisonVFX);
        Debug.Log($"{minion.name} is POISONED! ({damagePerTick} dmg/turn for {duration} turns)");
    }
    
    public void ApplyBurn(float damagePerTick, int duration)
    {
        RemoveEffectByType("Burn");
        activeEffects.Add(new BurnStatusEffect(damagePerTick, duration));
        SpawnEffectVFX("Burn", burnVFX);
        Debug.Log($"{minion.name} is BURNING! ({damagePerTick} dmg/turn for {duration} turns)");
    }
    
    public void ApplyStun(int duration)
    {
        RemoveEffectByType("Stun");
        activeEffects.Add(new StunStatusEffect(duration + 1));
        SpawnEffectVFX("Stun", stunVFX);
        Debug.Log($"{minion.name} is STUNNED for {duration} turn(s)!");
    }
    
    public void ApplyBuff(float attackBoost, float defenseBoost, int duration)
    {
        RemoveEffectByType("Buff");
        activeEffects.Add(new BuffStatusEffect(attackBoost, defenseBoost, duration));
        SpawnEffectVFX("Buff", buffVFX);
        Debug.Log($"{minion.name} is BUFFED! (+{attackBoost} ATK, +{defenseBoost} DEF for {duration} turns)");
    }
    
    public void ProcessTurnEffects()
    {
        if (activeEffects.Count == 0)
            return;
            
        Debug.Log($"Processing status effects on {minion.name}:");
        
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            StatusEffect effect = activeEffects[i];
            
            // Apply the effect (damage, etc)
            effect.ApplyEffect(minion);
            
            // Decrement duration
            effect.DecrementDuration();
            
            // Check if expired
            if (effect.IsExpired())
            {
                Debug.Log($"{effect.GetName()} effect expired on {minion.name}");
                RemoveEffectVFX(effect.GetName());
                activeEffects.RemoveAt(i);
            }
            else
            {
                Debug.Log($"{effect.GetName()} - {effect.GetRemainingTurns()} turn(s) remaining");
            }
        }
    }
    
    public bool IsStunned()
    {
        foreach (var effect in activeEffects)
        {
            if (effect is StunStatusEffect)
                return true;
        }
        return false;
    }
    
    public float GetAttackModifier()
    {
        float modifier = 0f;
        foreach (var effect in activeEffects)
        {
            if (effect is BuffStatusEffect buff)
                modifier += buff.attackBoost;
        }
        return modifier;
    }
    
    public float GetDefenseModifier()
    {
        float modifier = 0f;
        foreach (var effect in activeEffects)
        {
            if (effect is BuffStatusEffect buff)
                modifier += buff.defenseBoost;
        }
        return modifier;
    }
    
    private void RemoveEffectByType(string effectName)
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i].GetName() == effectName)
            {
                RemoveEffectVFX(effectName);
                activeEffects.RemoveAt(i);
            }
        }
    }
    
    private void SpawnEffectVFX(string effectName, GameObject vfxPrefab)
    {
        if (vfxPrefab != null && !activeVFX.ContainsKey(effectName))
        {
            GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity, transform);
            activeVFX[effectName] = vfx;
        }
    }
    
    private void RemoveEffectVFX(string effectName)
    {
        if (activeVFX.ContainsKey(effectName))
        {
            Destroy(activeVFX[effectName]);
            activeVFX.Remove(effectName);
        }
    }
    
    public List<string> GetActiveEffectNames()
    {
        List<string> names = new List<string>();
        foreach (var effect in activeEffects)
        {
            names.Add(effect.GetName());
        }
        return names;
    }
}

public abstract class StatusEffect
{
    protected int remainingTurns;
    
    public StatusEffect(int duration)
    {
        remainingTurns = duration;
    }
    
    public abstract void ApplyEffect(Minion target);
    public abstract string GetName();
    
    public void DecrementDuration()
    {
        remainingTurns--;
    }
    
    public bool IsExpired()
    {
        return remainingTurns <= 0;
    }
    
    public int GetRemainingTurns()
    {
        return remainingTurns;
    }
}

public class PoisonStatusEffect : StatusEffect
{
    public float damagePerTick;
    
    public PoisonStatusEffect(float damage, int duration) : base(duration)
    {
        damagePerTick = damage;
    }
    
    public override void ApplyEffect(Minion target)
    {
        target.TakeDamage(damagePerTick);
        Debug.Log($"Poison: {target.name} takes {damagePerTick} poison damage!");
    }
    
    public override string GetName() => "Poison";
}

public class BurnStatusEffect : StatusEffect
{
    public float damagePerTick;
    
    public BurnStatusEffect(float damage, int duration) : base(duration)
    {
        damagePerTick = damage;
    }
    
    public override void ApplyEffect(Minion target)
    {
        target.TakeDamage(damagePerTick);
        Debug.Log($"Burn: {target.name} takes {damagePerTick} burn damage!");
    }
    
    public override string GetName() => "Burn";
}

public class StunStatusEffect : StatusEffect
{
    public StunStatusEffect(int duration) : base(duration) { }
    
    public override void ApplyEffect(Minion target)
    {
        // TODO: skip target's turn
    }
    
    public override string GetName() => "Stun";
}

public class BuffStatusEffect : StatusEffect
{
    public float attackBoost;
    public float defenseBoost;
    
    public BuffStatusEffect(float attack, float defense, int duration) : base(duration)
    {
        attackBoost = attack;
        defenseBoost = defense;
    }
    
    public override void ApplyEffect(Minion target)
    {
        // NOTE: Buff effects are passive; no action needed each turn
    }
    
    public override string GetName() => "Buff";
}
