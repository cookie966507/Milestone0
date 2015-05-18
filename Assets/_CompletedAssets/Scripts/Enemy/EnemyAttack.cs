using UnityEngine;
using System.Collections;

namespace CompleteProject
{
    public class EnemyAttack : MonoBehaviour
    {
        public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
        public int attackDamage = 10;               // The amount of health taken away per attack.


        Animator anim;                              // Reference to the animator component.
        GameObject player;                          // Reference to the player GameObject.
        PlayerHealth playerHealth;                  // Reference to the player's health.
        EnemyHealth enemyHealth;                    // Reference to this enemy's health.
        bool playerInRange;                         // Whether player is within the trigger collider and can be attacked.
        float timer;                                // Timer for counting up to the next attack.

		EnemyMovement _movement; //added
		bool _enemyInRange; //added


        void Awake ()
        {
            // Setting up the references.
            player = GameObject.FindGameObjectWithTag ("Player");
            playerHealth = player.GetComponent <PlayerHealth> ();
            enemyHealth = GetComponent<EnemyHealth>();
            anim = GetComponent <Animator> ();

			_movement = GetComponent<EnemyMovement>(); //added
        }


        void OnTriggerEnter (Collider other)
        {
            // If the entering collider is the player...
            if(other.gameObject == player)
            {
                // ... the player is in range.
                playerInRange = true;
            }
			#region ADDED
			//confused targets will now find and hur other enemies. no score is given by enemies harming each other
			if(_movement._confusedTarget)
			{
				if(other.transform.Equals(_movement._confusedTarget))
				{
					_enemyInRange = true;
				}
			}
			#endregion
		}


        void OnTriggerExit (Collider other)
        {
            // If the exiting collider is the player...
            if(other.gameObject == player)
            {
                // ... the player is no longer in range.
                playerInRange = false;
            }
			#region ADDED
			if(_movement._confusedTarget)
			{
				if(other.transform.Equals(_movement._confusedTarget))
				{
					_enemyInRange = false;
				}
			}
			else
			{
				_enemyInRange = false;
			}
			#endregion
        }


        void Update ()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

            // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
            if(timer >= timeBetweenAttacks && enemyHealth.currentHealth > 0)
            {
				if(playerInRange)
				{
					// ... attack.
					Attack ();
				}
				#region ADDED
				//checking if enemy is confused and can attack othr enemy
				else if(_enemyInRange)
				{
					if(_movement._confusedTarget)
					{
						timer = 0f;
						
						if(_movement._confusedTarget.GetComponent<EnemyHealth>().currentHealth > 0)
						{
							_movement._confusedTarget.GetComponent<EnemyHealth>().TakeDamage (attackDamage, _movement._confusedTarget.position);
						}
					}
				}
				#endregion
            }

            // If the player has zero or less health...
            if(playerHealth.currentHealth <= 0)
            {
                // ... tell the animator the player is dead.
                anim.SetTrigger ("PlayerDead");
            }
        }


        void Attack ()
        {
            // Reset the timer.
            timer = 0f;

            // If the player has health to lose...
            if(playerHealth.currentHealth > 0)
            {
                // ... damage the player.
                playerHealth.TakeDamage (attackDamage);
            }
        }
    }
}