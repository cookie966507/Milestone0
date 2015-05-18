using UnityEngine;
using CompleteProject;

namespace CompleteProject
{
    public class EnemyHealth : MonoBehaviour
    {
        public int startingHealth = 100;            // The amount of health the enemy starts the game with.
        public int currentHealth;                   // The current health the enemy has.
        public float sinkSpeed = 2.5f;              // The speed at which the enemy sinks through the floor when dead.
        public int scoreValue = 10;                 // The amount added to the player's score when the enemy dies.
        public AudioClip deathClip;                 // The sound to play when the enemy dies.

		public PlayerShooting.AttackType typeAffecting = PlayerShooting.AttackType.None;
		private float timer = 0f;
		private float damageStepTimer = 0f;
		public float stepDamageTime = 1f;
		public float hitTime = 5f;
		private int _currentDamage = 0;
		public bool hit = false;

		public Material[] _materials;
		public ParticleSystem[] _effectParticles;

		private Transform confusedTarget;


        Animator anim;                              // Reference to the animator.
        AudioSource enemyAudio;                     // Reference to the audio source.
        ParticleSystem hitParticles;                // Reference to the particle system that plays when the enemy is damaged.
        CapsuleCollider capsuleCollider;            // Reference to the capsule collider.
        bool isDead;                                // Whether the enemy is dead.
        bool isSinking;                             // Whether the enemy has started sinking through the floor.

		EnemyMovement movement;

        void Awake ()
        {
            // Setting up the references.
            anim = GetComponent <Animator> ();
            enemyAudio = GetComponent <AudioSource> ();
            hitParticles = GetComponentInChildren <ParticleSystem> ();
            capsuleCollider = GetComponent <CapsuleCollider> ();

			movement = GetComponent<EnemyMovement>();

            // Setting the current health when the enemy first spawns.
            currentHealth = startingHealth;

			for(int i = 0; i < _effectParticles.Length; i++)
			{
				_effectParticles[i].enableEmission = false;
			}
        }


        void Update ()
        {
            // If the enemy should be sinking...
            if(isSinking)
            {
                // ... move the enemy down by the sinkSpeed per second.
                transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
            }

			if(typeAffecting != PlayerShooting.AttackType.None)
			{
				timer += Time.deltaTime;
				if(timer > hitTime)
				{
					CeaseBeingAttacked();
				}
				else
				{
					UpdateBeingAttacked();
				}
			}
        }

		void CeaseBeingAttacked()
		{
			switch(typeAffecting)
			{
			case PlayerShooting.AttackType.Fire:
				GetComponent<NavMeshAgent>().speed -= 2;
				_effectParticles[(int)typeAffecting - 1].enableEmission = false;
				break;
			case PlayerShooting.AttackType.Ice:
				GetComponent<NavMeshAgent>().speed += 2;
				_effectParticles[(int)typeAffecting - 1].enableEmission = false;
				break;
			case PlayerShooting.AttackType.Confusion:
				movement.confusedTarget = null;
				_effectParticles[(int)typeAffecting - 1].enableEmission = false;
				break;
			}
			typeAffecting = PlayerShooting.AttackType.None;
			damageStepTimer = 0f;
			timer = 0f;
			hit = false;
			GetComponentInChildren<Renderer>().material = _materials[0];
		}
		void UpdateBeingAttacked()
		{
			if(!hit)
			{
				hit = true;
				switch(typeAffecting)
				{
				case PlayerShooting.AttackType.Fire:
					GetComponent<NavMeshAgent>().speed += 2;
					GetComponentInChildren<Renderer>().material = _materials[(int)typeAffecting];
					_effectParticles[(int)typeAffecting - 1].enableEmission = true;
					break;
				case PlayerShooting.AttackType.Ice:
					GetComponent<NavMeshAgent>().speed -= 2;
					GetComponentInChildren<Renderer>().material = _materials[(int)typeAffecting];
					_effectParticles[(int)typeAffecting - 1].enableEmission = true;
					break;
				case PlayerShooting.AttackType.Confusion:
					movement.confusedTarget = GameObject.FindWithTag("Enemy").transform;
					GetComponentInChildren<Renderer>().material = _materials[(int)typeAffecting];
					_effectParticles[(int)typeAffecting - 1].enableEmission = true;
					break;
				}
			}

			damageStepTimer += Time.deltaTime;
			if(damageStepTimer > stepDamageTime)
			{
				TakeDamage (_currentDamage, transform.position);
				damageStepTimer = 0f;
			}
		}


        public void TakeDamage (int amount, Vector3 hitPoint)
        {
            // If the enemy is dead...
            if(isDead)
                // ... no need to take damage so exit the function.
                return;

            // Play the hurt sound effect.
            enemyAudio.Play ();

            // Reduce the current health by the amount of damage sustained.
            currentHealth -= amount;
            
            // Set the position of the particle system to where the hit was sustained.
            hitParticles.transform.position = hitPoint;

            // And play the particles.
            hitParticles.Play();

            // If the current health is less than or equal to zero...
            if(currentHealth <= 0)
            {
                // ... the enemy is dead.
                Death ();
            }
        }

		public void UpdateDamage (int amount, PlayerShooting.AttackType type)
		{
			_currentDamage = amount;
			if(typeAffecting == PlayerShooting.AttackType.None)
				typeAffecting = type;
		}


        void Death ()
        {
            // The enemy is dead.
            isDead = true;

            // Turn the collider into a trigger so shots can pass through it.
            capsuleCollider.isTrigger = true;

            // Tell the animator that the enemy is dead.
            anim.SetTrigger ("Dead");

            // Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing).
            enemyAudio.clip = deathClip;
            enemyAudio.Play ();
        }


        public void StartSinking ()
        {
            // Find and disable the Nav Mesh Agent.
            GetComponent <NavMeshAgent> ().enabled = false;

            // Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy).
            GetComponent <Rigidbody> ().isKinematic = true;

            // The enemy should no sink.
            isSinking = true;

            // Increase the score by the enemy's score value.
            ScoreManager.score += scoreValue;

            // After 2 seconds destory the enemy.
            Destroy (gameObject, 2f);
        }
    }
}