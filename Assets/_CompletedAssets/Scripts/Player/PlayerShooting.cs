using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace CompleteProject
{
    public class PlayerShooting : MonoBehaviour
    {
		#region ADDED
		public enum AttackType
		{
			None,
			Fire,
			Ice,
			Confusion,

			NumTypes
		}

		private AttackType _currentAttack = AttackType.None;

		private PickupWeapon _currentWeapon = null;
		public PickupWeapon[] _weapons;
		#endregion

        public int damagePerShot = 20;                  // The damage inflicted by each bullet.
        public float timeBetweenBullets = 0.15f;        // The time between each shot.
        public float range = 100f;                      // The distance the gun can fire.


        float timer;                                    // A timer to determine when to fire.
        Ray shootRay;                                   // A ray from the gun end forwards.
        RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
        int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
        public ParticleSystem gunParticles;                    // Reference to the particle system.
        LineRenderer gunLine;                           // Reference to the line renderer.
        AudioSource gunAudio;                           // Reference to the audio source.
        Light gunLight;                                 // Reference to the light component.
        float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.


        void Awake ()
        {
            // Create a layer mask for the Shootable layer.
            shootableMask = LayerMask.GetMask ("Shootable");

            // Set up the references.
            //gunParticles = GetComponent<ParticleSystem> ();
            gunLine = GetComponent <LineRenderer> ();
            gunAudio = GetComponent<AudioSource> ();
            gunLight = GetComponent<Light> ();
        }


        void Update ()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

#if !MOBILE_INPUT
            // If the Fire1 button is being press and it's time to fire...
			if(Input.GetButton ("Fire1"))
            {
                // ... shoot the gun.
                Shoot ();
            }
			#region ADDED
			//stop shooting
			if(Input.GetButtonUp("Fire1"))
			{
				CeaseFire();
			}
			#endregion
#else
            // If there is input on the shoot direction stick and it's time to fire...
            if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
            {
                // ... shoot the gun
                Shoot();
            }
#endif
            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if(timer >= timeBetweenBullets * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects ();
            }
        }


        public void DisableEffects ()
        {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
            gunLight.enabled = false;
        }

		#region ADDED
		//tells the weapon to stop firing
		void CeaseFire()
		{
			if(_currentWeapon != null)
				_currentWeapon.EndFire();
		}

		//shoots based on the current pickup
        void Shoot ()
        {
			if(_currentAttack == AttackType.None)
				ShootNormal();
			else
			{
				if(_currentWeapon == null)
					ChooseWeapon();

				if(_currentWeapon.canFire)
					_currentWeapon.Fire();
				else
					_currentAttack = AttackType.None;
			}
		}

		//gets the weapon in association with the pickup aquired
		void ChooseWeapon()
		{
			_currentWeapon = _weapons[(int)_currentAttack - 1];
			_currentWeapon.gameObject.SetActive(true);
		}
		#endregion

		void ShootNormal()
		{
			if(timer >= timeBetweenBullets && Time.timeScale != 0)
			{
				// Reset the timer.
				timer = 0f;
				
				// Play the gun shot audioclip.
				gunAudio.Play ();
				
				// Enable the light.
				gunLight.enabled = true;
				
				// Stop the particles from playing if they were, then start the particles.
				gunParticles.Stop ();
				gunParticles.Play ();
				
				// Enable the line renderer and set it's first position to be the end of the gun.
				gunLine.enabled = true;
				gunLine.SetPosition (0, transform.position);
				
				// Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
				shootRay.origin = transform.position;
				shootRay.direction = transform.forward;
				
				// Perform the raycast against gameobjects on the shootable layer and if it hits something...
				if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
				{
					// Try and find an EnemyHealth script on the gameobject hit.
					EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
					
					// If the EnemyHealth component exist...
					if(enemyHealth != null)
					{
						// ... the enemy should take damage.
						enemyHealth.TakeDamage (damagePerShot, shootHit.point);
					}
					
					// Set the second position of the line renderer to the point the raycast hit.
					gunLine.SetPosition (1, shootHit.point);
				}
				// If the raycast didn't hit anything on the shootable layer...
				else
				{
					// ... set the second position of the line renderer to the fullest extent of the gun's range.
					gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
				}
			}
		}

		#region ADDED
		//getters and setters for the Type of attack
		public AttackType Type
		{
			get { return _currentAttack; }
			set
			{
				if(_currentWeapon != null)
				{
					_currentWeapon.EndFire();
					_currentWeapon = null;
				}
				_currentAttack = value;
				ChooseWeapon();
				_currentWeapon.NewWeponPickdUp();
			}
		}
		#endregion
	}
}