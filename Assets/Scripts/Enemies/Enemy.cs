using System;
using Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        private PlayerController target;
        //pathfinding
        private NavMeshAgent agent;
        private void Start()
        {
            target = FindObjectOfType<PlayerController>();
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            // If you want to update the destination dynamically
            if (Vector3.Distance(agent.destination, target.transform.position) > 0.1f)
            {
                agent.SetDestination(target.transform.position);
            }
        }
    }
}