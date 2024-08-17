using System;
using Damage;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        // Movement speed of the player
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private GameObject projectilePrefab;

        [SerializeField] private int hitRange = 2;

        [SerializeField] private int meleeAttackDamage = 30;

        [SerializeField] private LayerMask enemyLayers;  // The layers that should be considered as enemies

        // Rigidbody component for physics-based movement
        private Rigidbody _rigidbody;

        private bool _leftMouseButtonDown;

        private bool _leftShiftButtonDown;

        private void Start()
        {
            // Get the Rigidbody component attached to this GameObject
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                Debug.LogError("Rigidbody component is missing from the player object.");
            }
        }

        private void Update()
        {
            // Get input from the horizontal and vertical axes
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Calculate the movement vector
            Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * movementSpeed;

            // Apply the movement to the Rigidbody
            _rigidbody.MovePosition(_rigidbody.position + movement * Time.deltaTime);
            Attack();
        }

        private void Attack()
        {
            //if the player has attacked need to release the mouse to attack again
            if (Input.GetButtonDown("Fire1") && !_leftMouseButtonDown)
            {
                Fire(GetMouseWorldPosition());
                _leftMouseButtonDown = true;
            }

            if (Input.GetButtonUp("Fire1"))
            {
                _leftMouseButtonDown = false;
            }

            if (Input.GetButtonDown("Fire3") && !_leftShiftButtonDown)
            {
                MeleeAttack(GetMouseWorldPosition());
                Debug.LogError("Melee attack");
                _leftShiftButtonDown = true;
            }

            if (Input.GetButtonUp("Fire3"))
            {
                _leftShiftButtonDown = false;
            }
        }
        

        private void Fire(Vector3 targetPosition)
        {
            //spawn a projectile
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            //get the Projectile component from the projectile object
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            //check if the Projectile component exists
            if (projectileComponent != null)
            {
                //set the initial direction of the projectile
                projectileComponent.SetInitialDirection((targetPosition - transform.position).normalized);
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            // Create a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Create a plane at the player's position with normal facing up
            Plane plane = new Plane(Vector3.up, transform.position);
            // Calculate the distance from the ray origin to the plane
            if (plane.Raycast(ray, out float distance))
            {
                // Get the point on the plane where the ray intersects
                Vector3 hitPoint = ray.GetPoint(distance);
                return hitPoint;
            }
            return Vector3.zero;
        }

        private void MeleeAttack(Vector3 targetPosition)
        {
            RaycastHit hit;
            Vector3 origin = transform.position;
            Vector3 forward = targetPosition - origin;
            
            if (Physics.Raycast(origin, forward, out hit, hitRange, enemyLayers))
            {
                Damageable damageable = hit.transform.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(meleeAttackDamage);
                }
            }
            Debug.DrawRay(origin, forward * hitRange, Color.red);
        }
    }
}