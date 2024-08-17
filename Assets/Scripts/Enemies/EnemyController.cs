using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float shootRate = 1f;

    private Rigidbody _rigidbody;

    private float _nextShoot;

    private void Start()
    {
        // Get the Rigidbody component attached to this GameObject
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody component is missing from the player object.");
        }
        _nextShoot = Time.time + shootRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _nextShoot) {
            // Update the time when the enemy can shoot next
            _nextShoot = Time.time + shootRate;

            Shoot();
        }
        
    }

    private void Shoot()
    {
        
        //spawn a projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        //get the Projectile component from the projectile object
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        //check if the Projectile component exists
        if (projectileComponent != null)
        {
            //set the initial direction of the projectile
            projectileComponent.SetInitialDirection((targetObject.transform.position - transform.position).normalized);
        }
    }
}
