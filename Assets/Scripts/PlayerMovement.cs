using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Collider _sphereCollider;
    private Camera playerCamera;
    private Animator animator;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 groundNormal;
    private bool shouldJump;
    private bool isActive = true;

    private Vector3 currentForward;

    // --- NUOVE VARIABILI PER IL RESET ---
    private Vector3 lastSafePosition;
    // -------------------------------------

    [Header("Character Controller Settings")]
    public Vector3 center;

    [Header("Movement Settings")]
    public float moveSpeed = 4.0f;
    public float runSpeed = 20.0f;
    public float acceleration = 6.0f;
    public float deceleration = 10.0f;
    public float jumpHeight = 1.5f;
    public float airResistance = 1f;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference runAction;
    public InputActionReference jumpAction;

    [Header("Ground Detection")]
    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer = 1;

    public bool Grounded => groundedPlayer;

    public void stopMoving()
    {
        isActive = false;
    }

    public void startMoving()
    {
        isActive = true;
    }

    private void Awake()
    {
        playerCamera = Camera.main;
        animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider>();
        _sphereCollider = GetComponent<SphereCollider>();
        _rigidbody = GetComponent<Rigidbody>();

        // Inizializza la posizione sicura all'avvio
        lastSafePosition = transform.position;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    void FixedUpdate()
    {
        if (!isActive)
            return;

        groundedPlayer = CheckGrounded();

        // --- AGGIORNAMENTO POSIZIONE SICURA ---
        // Se lo slime è a terra, memorizziamo dove si trova
        if (groundedPlayer)
        {
            lastSafePosition = transform.position;
        }
        // --------------------------------------

        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Read input
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = forward * input.y + right * input.x;
        move = Vector3.ClampMagnitude(move, 1f);

        Vector3 horizontalMove = new Vector3(move.x, 0, move.z);
        Vector3 horizontalVelocity = new Vector3(playerVelocity.x, 0.0f, playerVelocity.z);

        // Change Collider
        _collider.enabled = groundedPlayer;
        _sphereCollider.enabled = !groundedPlayer;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Player is falling
        if (!groundedPlayer)
        {
            Vector3 moveDir = _rigidbody.linearVelocity.normalized;
            Vector3 horizDir = horizontalVelocity.normalized;
            if (horizDir == Vector3.zero)
                horizDir = new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 lookDir = Vector3.Cross(moveDir, horizDir).normalized;
            if (moveDir.y > 0)
                lookDir = Vector3.Cross(moveDir, lookDir).normalized;
            else
                lookDir = Vector3.Cross(lookDir, moveDir).normalized;

            _rigidbody.MoveRotation(Quaternion.LookRotation(-lookDir, moveDir));

            float deltaX = Mathf.Abs(playerVelocity.x - _rigidbody.linearVelocity.x);
            float deltaZ = Mathf.Abs(playerVelocity.z - _rigidbody.linearVelocity.z);

            if (deltaX > 0)
                playerVelocity.x = 0.0f;
            else
                playerVelocity.x = Mathf.MoveTowards(playerVelocity.x, 0, airResistance * Time.deltaTime);

            if (deltaZ > 0)
                playerVelocity.z = 0.0f;
            else
                playerVelocity.z = Mathf.MoveTowards(playerVelocity.z, 0, airResistance * Time.deltaTime);
        }
        // Player is running
        else if (runAction.action.IsPressed())
        {
            Vector3 lookDir = Vector3.Cross(horizontalVelocity, groundNormal).normalized;
            lookDir = Vector3.Cross(groundNormal, lookDir).normalized;

            currentForward = Vector3.Lerp(transform.forward, lookDir, 0.1f);
            _rigidbody.MoveRotation(Quaternion.LookRotation(currentForward));

            Vector3 targetVelocity = horizontalMove * runSpeed;
            if (horizontalVelocity.magnitude < targetVelocity.magnitude)
            {
                playerVelocity.x = Mathf.MoveTowards(playerVelocity.x, targetVelocity.x, acceleration * Time.deltaTime);
                playerVelocity.z = Mathf.MoveTowards(playerVelocity.z, targetVelocity.z, acceleration * Time.deltaTime);
            }
            else
            {
                playerVelocity.x = Mathf.MoveTowards(playerVelocity.x, targetVelocity.x, deceleration * Time.deltaTime);
                playerVelocity.z = Mathf.MoveTowards(playerVelocity.z, targetVelocity.z, deceleration * Time.deltaTime);
            }
        }
        // Player is not running
        else
        {
            Vector3 lookDir = Vector3.Cross(horizontalVelocity, groundNormal).normalized;
            lookDir = Vector3.Cross(groundNormal, lookDir).normalized;

            currentForward = Vector3.Lerp(transform.forward, lookDir, 0.1f);
            _rigidbody.MoveRotation(Quaternion.LookRotation(currentForward));

            Vector3 targetVelocity = horizontalMove * moveSpeed;
            if (horizontalVelocity.magnitude < targetVelocity.magnitude)
            {
                playerVelocity.x = targetVelocity.x;
                playerVelocity.z = targetVelocity.z;
            }
            else
            {
                playerVelocity.x = Mathf.MoveTowards(playerVelocity.x, targetVelocity.x, deceleration * Time.deltaTime);
                playerVelocity.z = Mathf.MoveTowards(playerVelocity.z, targetVelocity.z, deceleration * Time.deltaTime);
            }
        }

        if (groundedPlayer && animator.GetInteger("Falling") > 0 && horizontalVelocity != Vector3.zero)
            _rigidbody.MoveRotation(Quaternion.LookRotation(horizontalVelocity.normalized));

        // Set Animation Flags
        if (groundedPlayer)
            animator.SetInteger("Falling", 0);
        else if (animator.GetInteger("Falling") == 0)
            animator.SetInteger("Falling", 1);

        animator.SetFloat("Roll Speed", playerVelocity.magnitude / 7.0f);

        if (runAction.action.IsPressed() && groundedPlayer)
            animator.SetInteger("Move", 2);
        else if (horizontalVelocity.magnitude > 0.1 && groundedPlayer)
        {
            if (animator.GetInteger("Move") == 2)
            {
                animator.SetInteger("Falling", 2);
                _rigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            }
            animator.SetInteger("Move", 1);
        }
        else
        {
            animator.SetInteger("Move", 0);
        }

        // Apply Horizontal Movement
        horizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        _rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, _rigidbody.linearVelocity.y, horizontalVelocity.z);
        
        // Jump
        if (shouldJump)
        {
            animator.SetTrigger("Jump");
            _rigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            shouldJump = false;
        }
    }

    private void Update()
    {
        if (jumpAction.action.triggered && groundedPlayer)
            shouldJump = true;

        // --- CONTROLLO TASTO RESET ---
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToLastSafePosition();
        }
        // ------------------------------
    }

    // Funzione per resettare lo slime
    private void ResetToLastSafePosition()
    {
        // Sposta lo slime aggiungendo un piccolo offset in alto per non farlo incastrare nel suolo
        transform.position = lastSafePosition + Vector3.up * 0.1f;
        
        // Azzera la velocità per non farlo "volare via" al reset
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        playerVelocity = Vector3.zero; // Azzera anche la velocità del nostro calcolo custom

        Debug.Log("Slime resettato alla posizione sicura!");
    }

    private bool CheckGrounded()
    {
        Bounds bounds = _sphereCollider.bounds;
        Vector3[] raycastOrigins = new Vector3[]
        {
        new Vector3(bounds.center.x, bounds.center.y + 0.2f, bounds.center.z),
        new Vector3(bounds.min.x, bounds.center.y + 0.2f, bounds.center.z),
        new Vector3(bounds.max.x, bounds.center.y + 0.2f, bounds.center.z),
        new Vector3(bounds.center.x, bounds.center.y + 0.2f, bounds.min.z),
        new Vector3(bounds.center.x, bounds.center.y + 0.2f, bounds.max.z)
        };

        int groundedCount = 0;
        bool centerGrounded = false;

        foreach (Vector3 origin in raycastOrigins)
        {
            RaycastHit hit;
            bool hitGround = Physics.Raycast(origin, -Vector3.up, out hit, groundCheckDistance + 0.1f, groundLayer);

            if (hitGround)
            {
                Debug.DrawLine(origin, hit.point, Color.green, Time.deltaTime);

                if (origin == raycastOrigins[0])
                {
                    centerGrounded = true;
                    groundNormal = hit.normal;
                }
                groundedCount++;
                if (groundedCount >= 2)
                    groundNormal = hit.normal;
            }
            else
            {
                Debug.DrawRay(origin, -Vector3.up * (groundCheckDistance + 0.1f), Color.red, Time.deltaTime);
            }
        }

        return centerGrounded || groundedCount >= 2;
    }

    private void LateUpdate()
    {
    }
}
