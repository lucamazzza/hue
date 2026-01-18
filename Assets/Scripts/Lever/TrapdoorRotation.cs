using UnityEngine;

public class TrapdoorRotation : Trapdoor
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    
    private bool isOpening = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(openAngle, 0, 0);
    }

    private void Update()
    {
        if (isOpening)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation, 
                openRotation, 
                Time.deltaTime * openSpeed
            );
        }
    }

    public override void Open()
    {
        isOpening = true;
    }
}