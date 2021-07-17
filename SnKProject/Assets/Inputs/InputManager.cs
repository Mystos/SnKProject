using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;

    public CharacterController controller;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Transform cam;
    public Vector2 movementInput;

    Vector3 velocity;
    Vector3 oldVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float gravityValue = -9.81f;


    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;

    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;

    private float maxHeightReached = Mathf.NegativeInfinity;
    private float startHeight = Mathf.NegativeInfinity;
    private bool reachedApex = true;
    float jumpTimer = 0;


    private float Gravity => -2 * jumpHeight / (timeToJumpApex * timeToJumpApex);
    private float JumpForce => 2 * jumpHeight / timeToJumpApex;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if(playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.Gameplay.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {

        groundedPlayer = controller.isGrounded;
        //if (groundedPlayer && velocity.y > 0)
        //{
        //    velocity.y = 0f;
        //}

        if (playerControls.Gameplay.Jump.triggered && groundedPlayer)
        {
            Jump();
            //velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        if (!groundedPlayer && !reachedApex)
        {
            jumpTimer += Time.deltaTime;
        }

        if (!reachedApex && maxHeightReached > transform.position.y)
        {
            float delta = maxHeightReached - startHeight;
            float error = jumpHeight - delta;
            Debug.Log($"jump result: start:{startHeight:F4}, end:{maxHeightReached:F4}, delta:{delta:F4}, error:{error:F4}, time:{jumpTimer:F4}, gravity:{Gravity:F4}, jumpForce:{JumpForce:F4}");
            reachedApex = true;
        }
        maxHeightReached = Mathf.Max(transform.position.y, maxHeightReached);

        oldVelocity = velocity;

        Vector3 direction = new Vector3(movementInput.x, 0, movementInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y ;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDir.normalized * playerSpeed * Time.deltaTime);
        }

        velocity.y += gravityValue * Time.deltaTime;

        // Calculate Delta Pos et Move
        Vector3 deltaPosition = (oldVelocity + velocity) * 0.5f * Time.deltaTime;
        controller.Move(deltaPosition);

        if (groundedPlayer)
        {
            velocity.y = 0;
        }
    }

    private void Jump()
    {
        jumpTimer = 0;
        maxHeightReached = Mathf.NegativeInfinity;
        velocity.y = JumpForce;
        startHeight = transform.position.y;
        reachedApex = false;
    }



}
