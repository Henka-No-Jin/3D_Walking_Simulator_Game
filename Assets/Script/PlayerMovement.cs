using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private InputActionReference movementControl;

    [SerializeField]
    private InputActionReference jumpingControl;

    [SerializeField]
    private InputActionReference sprintControl;

    private float rotationSpeed = 4f;
    private float playerSpeed = 10.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -25.81f;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private Transform cameraController;
    private bool isWalking = false;
    private Animator animator;

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpingControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpingControl.action.Disable();
    }

    private void Start()    
    {
        controller = gameObject.GetComponent<CharacterController>();
        cameraController = Camera.main.transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = movementControl.action.ReadValue<Vector2>();

        Vector3 player_movement = new Vector3(movement.x, 0 , movement.y);

        player_movement = cameraController.forward * player_movement.z + cameraController.right * player_movement.x;

        player_movement.y = 0f;

        controller.Move(player_movement * Time.deltaTime * playerSpeed);

        //magnitude = x/y player position in game
        isWalking = movement.magnitude > 0.1f;


        if (sprintControl.action.triggered)
        {
            playerSpeed = 15.0f;
        }

        if (jumpingControl.action.triggered && groundedPlayer)
        {
            //used to calculate the square root of the value by multiplying the jump height with -3.0f and gravity value
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animator.SetTrigger("Jump");
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if(movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x,movement.y) * Mathf.Rad2Deg + cameraController.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle,0);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);    
        }

        animator.SetBool("isWalking", isWalking);
    }
}