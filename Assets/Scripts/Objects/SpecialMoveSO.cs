using UnityEngine;

[CreateAssetMenu(fileName = "SpecialMove", menuName = "Scriptable Objects/Special Move")]
public class SpecialMoveSO : MoveSO
{
    [Header("Special Move Properties")]
    [Tooltip("Damage multiplier applied to base attack (e.g., 2.0 = double damage)")]
    public float damageMultiplier = 1.5f;
    
    [Header("Visual Effects")]
    [Tooltip("VFX prefab to spawn when move is executed")]
    public GameObject vfxPrefab;
    
    [Tooltip("Position offset for VFX (relative to target)")]
    public Vector3 vfxOffset = Vector3.zero;
    
    [Tooltip("Duration before VFX is destroyed (0 = auto-destroy)")]
    public float vfxDuration = 2f;
    
    [Tooltip("Play VFX on attacker instead of target")]
    public bool vfxOnAttacker = false;
    
    [Header("Audio")]
    [Tooltip("Sound effect to play when move is executed")]
    public AudioClip soundEffect;
    
    [Header("Side Effects")]
    [Tooltip("Type of side effect to apply")]
    public SideEffectType effectType = SideEffectType.None;
    
    [Tooltip("Chance to apply side effect (0-100%)")]
    [Range(0f, 100f)]
    public float effectChance = 100f;
    
    [Header("Poison Effect Settings")]
    public float poisonDamagePerTick = 5f;
    public int poisonDuration = 3;
    
    [Header("Heal Effect Settings")]
    public float healAmount = 20f;
    public bool healSelf = false;
    
    [Header("Burn Effect Settings")]
    public float burnDamagePerTick = 8f;
    public int burnDuration = 2;
    
    [Header("Stun Effect Settings")]
    public int stunDuration = 1;
    
    [Header("Buff Effect Settings")]
    public float attackBoost = 10f;
    public float defenseBoost = 5f;
    public int buffDuration = 2;
    public bool buffSelf = true;
    
    [Header("Animation")]
    [Tooltip("Animation trigger name for this special move")]
    public string animationTrigger = "SpecialAttack";
    
    [Header("Cooldown")]
    [Tooltip("Number of turns before this move can be used again (0 = no cooldown)")]
    public int cooldownTurns = 0;
    
    public ISideEffect GetSideEffect()
    {
        switch (effectType)
        {
            case SideEffectType.Poison:
                return new PoisonEffect { damagePerTick = poisonDamagePerTick, duration = poisonDuration };
                
            case SideEffectType.Heal:
                return new HealEffect { healAmount = healAmount, healSelf = healSelf };
                
            case SideEffectType.Burn:
                return new BurnEffect { damagePerTick = burnDamagePerTick, duration = burnDuration };
                
            case SideEffectType.Stun:
                return new StunEffect { duration = stunDuration };
                
            case SideEffectType.Buff:
                return new BuffEffect 
                { 
                    attackBoost = attackBoost, 
                    defenseBoost = defenseBoost, 
                    duration = buffDuration,
                    buffSelf = buffSelf
                };
                
            default:
                return null;
        }
    }
    
    public bool ShouldApplyEffect()
    {
        return effectType != SideEffectType.None && Random.Range(0f, 100f) <= effectChance;
    }
}