using UnityEngine;
using BaseClasses;

namespace Implementation
{
    public class PlayerScript : CharacterSheet
    {
        // Public fields for movement parameters
        public float moveSpeed = 5f;
        public float jumpForce = 7f;
        private bool isGrounded;

        // Reference to Rigidbody
        private Rigidbody rb;

        // Camera reference for player direction
        public Transform cameraTransform;

        protected override void StartWrapper()
        {
            base.StartWrapper();
            // Get the Rigidbody component attached to the player
            rb = GetComponent<Rigidbody>();
        }

        protected override void UpdateWrapper()
        {
            base.UpdateWrapper();
            WalkCycle();
            Jump();
        }

        // Main Walk Cycle for 3D movement
        private void WalkCycle()
        {
            // Get horizontal and vertical inputs (W/A/S/D or arrow keys)
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            // Get the forward direction relative to the camera's rotation
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // Project forward and right vectors onto the horizontal plane (y=0)
            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            // Move the player along the camera's forward and right directions
            Vector3 moveDirection = forward * moveZ + right * moveX;
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

            // Move the player using Rigidbody
            rb.MovePosition(transform.position + movement);
        }

        // Handle Jumping
        private void Jump()
        {
            // Check if the player is grounded
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                // Apply an upward force for jumping
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        }

        // Collision detection to check if player is grounded
        private void OnCollisionEnter(Collision collision)
        {
            // If the player collides with an object tagged as "Ground", consider the player grounded
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }
    }
}