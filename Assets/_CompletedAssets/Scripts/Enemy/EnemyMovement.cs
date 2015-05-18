using UnityEngine;
using System.Collections;

namespace CompleteProject
{
    public class EnemyMovement : MonoBehaviour
    {
        Transform player;               // Reference to the player's position.
        PlayerHealth playerHealth;      // Reference to the player's health.
        EnemyHealth enemyHealth;        // Reference to this enemy's health.
        NavMeshAgent nav;               // Reference to the nav mesh agent.

		public Transform _confusedTarget; //added


        void Awake ()
        {
            // Set up the references.
            player = GameObject.FindGameObjectWithTag ("Player").transform;
            playerHealth = player.GetComponent <PlayerHealth> ();
            enemyHealth = GetComponent <EnemyHealth> ();
            nav = GetComponent <NavMeshAgent> ();
        }


        void Update ()
        {
            // If the enemy and the player have health left...
            if(enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
            {
				#region ADDED
				//find an enemy if confused
				if(_confusedTarget)
					nav.SetDestination(_confusedTarget.position);
				else
	                // ... set the destination of the nav mesh agent to the player.
	                nav.SetDestination (player.position);
				#endregion
            }
            // Otherwise...
            else
            {
                // ... disable the nav mesh agent.
                nav.enabled = false;
            }
        }
    }
}