using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float velocity = 1.0f;

    [SerializeField] private float _maxHealth = 3.0f;

    private float _currentHealth;
    private MeshRenderer _meshRenderer;

    void Start()
    {
        _currentHealth = _maxHealth;

        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;
       
        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput.x = -1;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput.x = 1;
            
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            moveInput.y = -1;
        else if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            moveInput.y = 1;
        
        float moveX = moveInput.x * velocity * Time.deltaTime;
        float moveZ = moveInput.y * velocity * Time.deltaTime;
        transform.Translate(moveX, 0, moveZ);

    }

    public bool GetDamaged(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        return _currentHealth > 0;
    }

    public void GetCure(float cure)
    {
        _currentHealth += cure;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
    }
}
