using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Player))]
public class MainPlayer : MonoBehaviour
{
    public bool isInFight = false;
    public float rotSpeed = 15.0f;
    public float moveSpeed = 6.0f;
    public float jumpSpeed = 15.0f;
    public float gravity = -9.8f;
    public float terminalVelocity = -10.0f;
    public float minFall = -1.5f;

    [SerializeField] private Transform _target;
    private CharacterController _characterController;
    private Player _player;
    private float _vertSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _player = GetComponent<Player>();

        _vertSpeed = minFall;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if new Input System devices are available
        if (Keyboard.current == null)
            return;
            
        Vector3 movement = Vector3.zero;
        Vector2 moveInput = Vector2.zero;
        
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput.x = -1;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput.x = 1;
            
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            moveInput.y = -1;
        else if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            moveInput.y = 1;

        if (moveInput.x != 0 || moveInput.y != 0)
        {
            movement.x = moveInput.x;
            movement.z = moveInput.y;
            movement = Vector3.ClampMagnitude(movement, moveSpeed);

            Quaternion tmp = _target.rotation;
            _target.eulerAngles = new Vector3(0, _target.eulerAngles.y, 0);
            movement = _target.TransformDirection(movement);
            _target.rotation = tmp;

            Quaternion direction = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
        }

        if (_characterController.isGrounded)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _vertSpeed = jumpSpeed;
            }
            else
            {
                _vertSpeed = minFall;
            }
        }
        else
        {
            _vertSpeed += gravity * 5 * Time.deltaTime;

            if (_vertSpeed > terminalVelocity)
                _vertSpeed = terminalVelocity;
        }

        movement.y = _vertSpeed;
        movement *= Time.deltaTime;
        _characterController.Move(movement);
    }
}
