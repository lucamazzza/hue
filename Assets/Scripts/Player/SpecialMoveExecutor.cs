using System.Collections;
using UnityEngine;

public class SpecialMoveExecutor : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    
    public IEnumerator ExecuteSpecialMove(SpecialMoveSO specialMove, Minion attacker, Minion target)
    {
        if (specialMove == null || attacker == null || target == null)
            yield break;
        Debug.Log($"---{attacker.name} executes {specialMove.moveName}!---");
       if (!string.IsNullOrEmpty(specialMove.animationTrigger))
        {
            Animator animator = attacker.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger(specialMove.animationTrigger);
                yield return new WaitForSeconds(0.3f);
            }
        }
        if (specialMove.soundEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(specialMove.soundEffect);
        }
        GameObject vfxInstance = null;
        if (specialMove.vfxPrefab != null)
        {
            Transform vfxTarget = specialMove.vfxOnAttacker ? attacker.transform : target.transform;
            Vector3 vfxPosition = vfxTarget.position + specialMove.vfxOffset;
            vfxInstance = Instantiate(specialMove.vfxPrefab, vfxPosition, Quaternion.identity);
            vfxInstance.transform.SetParent(vfxTarget);
        }
        yield return new WaitForSeconds(0.2f);
        if (specialMove.ShouldApplyEffect())
        {
            ISideEffect sideEffect = specialMove.GetSideEffect();
            if (sideEffect != null)
            {
                sideEffect.Apply(target, attacker);
                Debug.Log($"✨ {sideEffect.GetDescription()}");
            }
        }
        else if (specialMove.effectType != SideEffectType.None)
        {
            Debug.Log($"Side effect failed to apply ({specialMove.effectChance}% chance)");
        }
        if (vfxInstance != null && specialMove.vfxDuration > 0)
        {
            yield return new WaitForSeconds(specialMove.vfxDuration);
            Destroy(vfxInstance);
        }
        Debug.Log($"---{specialMove.moveName} complete!---");
    }
}
