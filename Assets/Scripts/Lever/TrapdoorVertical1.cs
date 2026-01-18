using UnityEngine;

public class TrapdoorVertical : Trapdoor
{
    [SerializeField] private float openHeight = 3f;
    [SerializeField] private float openSpeed = 2f;
    
    private bool isOpening = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    private void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.Lerp(
                transform.position, 
                openPosition, 
                Time.deltaTime * openSpeed
            );
        }
    }

    public override void Open()
    {
        isOpening = true;
    }
}