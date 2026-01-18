using UnityEngine;

public class PlayerWalkTimer : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.4f;
    [SerializeField] private float volume = 0.5f;

    private float stepTimer;
    private Rigidbody rb;
    private PlayerMovement pm;

    void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude > 0.1f && pm.Grounded) 
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0;
        }
    }

    private void PlayFootstep()
    {
        SoundFXManager.instance.PlayRandomSoundFXClip(footstepClips, transform, volume);
    }
}
