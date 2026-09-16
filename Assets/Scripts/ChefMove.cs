using UnityEngine;
using UnityEngine.InputSystem;

public class ChefMove : MonoBehaviour
{   [Header("Movement Settings")]
    [SerializeField] public float PlayerSpeed =5f;

    private Rigidbody rb;
    private Animator animator;
    private Vector2 movementInput;
    private InputAction moveAction;

    private static readonly int IsWalkingHash = Animator.StringToHash("Walking");
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        animator = GetComponentInChildren<Animator>();

        moveAction = new InputAction("Move",binding: "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("2DVector")
        .With("Up","<Keyboard>/w")
        .With("Up","<Keyboard>/upArrow")
        .With("Down","<Keyboard>/s")
        .With("Down","<Keyboard>/downArrow")
        .With("Left","<Keyboard>/a")
        .With("Left","<Keyboard>/leftArrow")
        .With("Right","<Keyboard>/d")
        .With("Right","<Keyboard>/rightArrow");
    }

    private void OnEnable(){
        moveAction.Enable();
    }
    private void OnDisable(){
        moveAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {  
       movementInput =moveAction.ReadValue<Vector2>();
       
       if ( movementInput.sqrMagnitude >1){
            movementInput.Normalize();
       }
        
       if(animator!=null){
            bool isMoving = movementInput.sqrMagnitude>0.01f;
            animator.SetBool(IsWalkingHash,isMoving);
       }
    }
    private void FixedUpdate(){
        Vector3 moveDirection = new Vector3(movementInput.x,0f,movementInput.y);
        if(moveDirection.sqrMagnitude>0.01f){
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
        

        rb.MovePosition(rb.position+ moveDirection*PlayerSpeed*Time.fixedDeltaTime);

    }
    public void StopMovement()
    {
        movementInput = Vector2.zero;
        enabled = false;
    }
}
