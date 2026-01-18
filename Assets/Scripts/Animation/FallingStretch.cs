using UnityEngine;

public class FallingStretch : StateMachineBehaviour
{
    Rigidbody rb;
    private float maxFallSpeed = 10f;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rb == null)
            rb = animator.GetComponent<Rigidbody>();

        float fallSpeed = Mathf.Abs(rb.linearVelocity.magnitude);
        float targetStretchPercent = Mathf.Clamp(fallSpeed / maxFallSpeed, 0f, 1f);

        animator.SetFloat("Fall Stretch", targetStretchPercent);
    }
}
