using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        // Movement speed of the player
        [SerializeField] private float movementSpeed = 5f;

        // Rigidbody component for physics-based movement
        private Rigidbody _rigidbody;

        private void Start()
        {
            // Get the Rigidbody component attached to this GameObject
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                Debug.LogError("Rigidbody component is missing from the player object.");
            }
        }

        private void FixedUpdate()
        {
            // Get input from the horizontal and vertical axes
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Calculate the movement vector
            Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * movementSpeed;

            // Apply the movement to the Rigidbody
            _rigidbody.MovePosition(_rigidbody.position + movement * Time.fixedDeltaTime);
        }
    }
}