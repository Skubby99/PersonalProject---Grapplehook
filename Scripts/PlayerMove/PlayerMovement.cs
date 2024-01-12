using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [Header("Movement")]// Creates a Grouping name for public variables
    public float moveSpeed, SprintSpeed, groundDrag, maxMoveSpeed;
    float defaultSpeed;

    public float jumpForce;
    public float jumpCooldown;
    // allow for double jump
    public float extraJumps; // grounded check is triggered automatically when player leaves the ground giving them a free extra jump check myInput function
    float maxJumps;

    public float airFloat;
    bool allowJump;

    public float gForce;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool isGrounded;

    [Header("Object Reference")]
    public Transform orientation;
    public Rigidbody rb;

    [Header("Misc. Variables")]
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    // Start is called before the first frame update
    private void Start()
    {
        //Grabs rigidbody elements
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        allowJump = true;

        defaultSpeed = moveSpeed;
        maxJumps = extraJumps;
    }

    // Update is called once per frame
    private void Update()
    {
        MyInput();
        SpeedControl();

        // Handle drag
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        //Ground check (raycast from player position, in the down direction, with length of player height * half of players height + 20% longer, Check for layer whatisground)
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MovePlayer();
    }

    private void MyInput()
    {
        // Get Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump input
        if(Input.GetKeyDown(jumpKey) && allowJump)
        {
            Jump();
            if(extraJumps <= 0)
                allowJump = false;
        }
        // After changing GetKey to GetKeyDown it gives a free extra jump most likely because grounded is triggered as the player leaves the ground causing it to auto reset.
        if (isGrounded)
            Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void MovePlayer()
    {
        // Calculate movement direction (so you move in the direction your looking
        if (Input.GetKey(sprintKey) && isGrounded && moveSpeed <= maxMoveSpeed)
            moveSpeed += SprintSpeed * Time.fixedDeltaTime;
        else if(!Input.GetKey(sprintKey))
            moveSpeed = defaultSpeed;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if(!isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airFloat , ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        if (!isGrounded) // adds gravity to the player to force them down
        {
            rb.AddForce(transform.up * -gForce * Time.fixedDeltaTime, ForceMode.Impulse);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        extraJumps -= 1;
    }

    private void ResetJump()
    {
        allowJump = true;
        extraJumps = maxJumps;
    }

}
