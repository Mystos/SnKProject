using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerControllerRb : NetworkBehaviour
{
    PlayerControls playerControls;

    public float playerHeight = 2f;
    public float moveSpeed = 6f;
    public float groundDrag = 6f;
    public float airDrag = 2f;

    public float movementMultiplier = 10f;
    public float airMultiplier = 0.4f;
    public float jumpForce = 5f;

    public Vector2 movementInput;
    public Vector3 moveDirection;

    bool isGrounded;

    Rigidbody rb;
    public GameObject playerCamera;
    public GameObject cameraBrain;
    //public AudioListener playerListener;
    



    public void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.Gameplay.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        }

        playerControls.Enable();
    }

    public void OnDisable()
    {
        playerControls.Disable();
    }

    // Start is called before the first frame update
    public void Start()
    {
        if (IsLocalPlayer && IsOwner)
        {
            playerCamera.SetActive(true);
            cameraBrain.SetActive(true);
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
        }
        else
        {
            playerCamera.SetActive(false);
            cameraBrain.SetActive(false);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (IsLocalPlayer)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f);


            moveDirection = movementInput.y * transform.forward + movementInput.x * transform.right;
            Debug.Log(moveDirection);
            rb.drag = isGrounded ? groundDrag : airDrag;

            if (playerControls.Gameplay.Jump.triggered && isGrounded)
            {
                Jump();
            }
        }

    }

    public void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            if (isGrounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
            }
            else if (!isGrounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
            }
        }

    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
}
