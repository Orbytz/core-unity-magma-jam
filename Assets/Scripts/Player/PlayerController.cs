using System;
using Damage;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        // Movement speed of the player
        [Header("Movement Setup")]
        [SerializeField] private float movementSpeed = 15f;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private GameObject MeleeAttackPrefab;
        [SerializeField] private float hitRange = 0.5f;
        [SerializeField] private float recoverTime = 0.7f;

        [Space(10)]

        [Header("Attack Setup")]
        [SerializeField] private int meleeAttackDamage = 30;
        [SerializeField] private LayerMask enemyLayers;  // The layers that should be considered as enemies
        [SerializeField] private float rotationSpeed = 10f;  // Speed at which the player rotates

        // Rigidbody component for physics-based movement
        private Rigidbody _rigidbody;

        private bool _leftMouseButtonDown;

        private bool _leftShiftButtonDown;

        private Camera _mainCamera;  // Reference to the main camera

        private MeleeAttackBox _meleeAttackBox = null;

        private float _previousMeleeAttack;

        private void Start()
        {
            // Get the Rigidbody component attached to this GameObject
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                Debug.LogError("Rigidbody component is missing from the player object.");
            }
            // Get the main camera
            _mainCamera = Camera.main;

            _previousMeleeAttack = Time.time;
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

            RotateTowardsMouse();

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
                var canAttackList = new List<string> { "Enemy" };
                projectileComponent.SendMessage("EditCanAttack", canAttackList);

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
            Vector3 forward = (transform.forward * hitRange) + origin;

            if (_meleeAttackBox == null && Time.time > _previousMeleeAttack + recoverTime) {
                _previousMeleeAttack = Time.time;
                GameObject attackBox = Instantiate(MeleeAttackPrefab, forward, transform.rotation);
                _meleeAttackBox = attackBox.GetComponent<MeleeAttackBox>();
                _meleeAttackBox.transform.parent = gameObject.transform;
                if (_meleeAttackBox != null)
                {
                    Debug.LogError("create");
                    var canAttackList = new List<string> { "Enemy" };
                    _meleeAttackBox.SendMessage("EditCanAttack", canAttackList);
                }
            }
        }

        void RotateTowardsMouse()
        {
            // Get the mouse position in the world space
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);  // Define a plane at the ground level

            // Check where the ray intersects the plane
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 targetPoint = ray.GetPoint(distance);  // Get the point on the plane
                Vector3 direction = (targetPoint - transform.position).normalized;  // Calculate the direction

                // Create a target rotation
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

                // Smoothly rotate towards the target rotation
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}