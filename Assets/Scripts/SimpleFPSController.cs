using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class JumpGravityFixed : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 6f;      // velocità iniziale del salto (impostalo a mano)
    public float gravity = -9.81f;    // deve essere negativo
    public float fallMultiplier = 2.5f; // quando si cade, gravity è ancora più forte
    public Camera cam;
    public float mouseSensitivity = 0.12f;

    CharacterController cc;
    Vector3 velocity;
    float pitch;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        if (cam == null) cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // look (mouse)
        if (Mouse.current != null)
        {
            Vector2 md = Mouse.current.delta.ReadValue() * mouseSensitivity;
            transform.Rotate(Vector3.up * md.x);
            pitch -= md.y;
            pitch = Mathf.Clamp(pitch, -85f, 85f);
            cam.transform.localEulerAngles = Vector3.right * pitch;
        }

        // move (WASD)
        Vector3 input = Vector3.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input += transform.forward;
            if (Keyboard.current.sKey.isPressed) input -= transform.forward;
            if (Keyboard.current.aKey.isPressed) input -= transform.right;
            if (Keyboard.current.dKey.isPressed) input += transform.right;
        }
        Vector3 horizontal = input.normalized * speed;

        // ground check (CharacterController.isGrounded)
        bool grounded = cc.isGrounded;
        if (grounded && velocity.y < 0f) velocity.y = -2f; // piccolo valore per restare attaccati

        // jump
        bool jumpPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        if (jumpPressed && grounded)
        {
            velocity.y = jumpForce; // imposta velocità iniziale fissa
            Debug.Log("Jump! initial vy = " + velocity.y);
        }

        // gravity apply with fall multiplier
        if (velocity.y < 0)
        {
            // stiamo cadendo — rendi la caduta più rapida
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            // salendo / normale
            velocity.y += gravity * Time.deltaTime;
        }

        // applica movimento
        Vector3 finalMove = horizontal * Time.deltaTime + new Vector3(0, velocity.y * Time.deltaTime, 0);
        cc.Move(finalMove);
    }
}
