using UnityEngine;
using UnityEngine.InputSystem;

public class ChefMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float PlayerSpeed = 5f;

    [Header("Animation Settings")]
    [SerializeField] private string idleAnimName = "Standing Idle";
    [SerializeField] private string walkingAnimName = "Walking";

    private Rigidbody rb;
    private Animator animator;
    private Vector2 movementInput;
    private InputAction moveAction;
    private bool isWalking = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/a")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d")
            .With("Right", "<Keyboard>/rightArrow");
    }

    private void Start()
    {
        moveAction.Enable();

        // Default animation to Standing Idle on start
        if (animator != null)
        {
            isWalking = false;
            animator.Play(idleAnimName);
        }

        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive())
        {
            this.enabled = false;
        }
    }

    void Update()
    {
        movementInput = moveAction.ReadValue<Vector2>();

        if (movementInput.sqrMagnitude > 1)
        {
            movementInput.Normalize();
        }

        bool isMoving = movementInput.sqrMagnitude > 0.01f;

        // Switch animation using animator.Play
        if (animator != null)
        {
            if (isMoving && !isWalking)
            {
                isWalking = true;
                animator.Play(walkingAnimName);
            }
            else if (!isMoving && isWalking)
            {
                isWalking = false;
                animator.Play(idleAnimName);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        rb.MovePosition(rb.position + moveDirection * PlayerSpeed * Time.fixedDeltaTime);
    }

    public void StopMovement()
    {
        movementInput = Vector2.zero;
        if (animator != null && isWalking)
        {
            isWalking = false;
            animator.Play(idleAnimName);
        }
        enabled = false;
    }

    private void OnDisable()
    {
        movementInput = Vector2.zero;
        if (animator != null && isWalking)
        {
            isWalking = false;
            animator.Play(idleAnimName);
        }
    }
}
