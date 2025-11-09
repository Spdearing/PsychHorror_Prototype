using UnityEngine;

public class Player_Final_Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.2f;  // meters
    [SerializeField] private float gravity = -20f;     // m/s^2 (negative)
    [SerializeField] private float groundedSnap = -2f; // small downward force to stick to ground

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;    // place at feet
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // --- Ground check ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask, QueryTriggerInteraction.Ignore);

        if (isGrounded && velocity.y < 0f)
        {
            // Small downward value helps keep the controller grounded on slopes
            velocity.y = groundedSnap;
        }

        // --- Movement input ---
        float x = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float z = Input.GetAxisRaw("Vertical");   // W/S or Up/Down

        Vector3 move = (transform.right * x + transform.forward * z);
        if (move.sqrMagnitude > 1f) move.Normalize(); // prevent faster diagonal speed

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);

        controller.Move(move * speed * Time.deltaTime);

        // --- Jump ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // v = sqrt(2gh)  (gravity is negative)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --- Gravity ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
