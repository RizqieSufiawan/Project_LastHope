using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    public Vector2 MoveInput => moveInput;

    private float facingX = 1f;
    public float FacingX => facingX;

    [Header("Audio")]
    public AudioSource footstepSource;
    public AudioClip footstepClip;
    public float footstepInterval = 0.35f;
    private float footstepTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;

        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
        animator.SetBool("isWalking", moveInput.sqrMagnitude > 0.01f);

        if (moveInput.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);

            if (moveInput.x != 0f)
                facingX = Mathf.Sign(moveInput.x);

            HandleFootsteps();
        }
        else
        {
            footstepTimer = 0f;
            if (footstepSource != null && footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
    }

    private void HandleFootsteps()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            if (footstepSource != null && footstepClip != null)
            {
                footstepSource.PlayOneShot(footstepClip);
            }
            footstepTimer = footstepInterval;
        }
    }

    public void move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}   