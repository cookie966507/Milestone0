using UnityEngine;
using System.Collections;

namespace CompleteProject
{
	public class PickupWeapon : MonoBehaviour
	{
		private const float INIT_DEPLETE_TIME = 5f;
		public float _depleteTime = 5f;
		public bool canFire = true;
		public int _damage = 10;

		public ParticleSystem _particles;
		public bool firing = false;

		BoxCollider _col;

		PlayerShooting playerShooting; 

		void Awake()
		{
			_col = GetComponent<BoxCollider>();
			_col.enabled = false;

			playerShooting = GetComponentInParent <PlayerShooting> ();
			_particles.enableEmission = false;
		}

		void OnTriggerStay(Collider _col)
		{
			if(_col.tag.Equals("Enemy"))
			{
				EnemyHealth enemyHealth = _col.GetComponent<EnemyHealth>();

				if(enemyHealth != null)
				{
					// ... the enemy should take damage.
					enemyHealth.UpdateDamage (_damage, playerShooting.Type);
				}
			}
		}

		public void Fire()
		{
			if(!firing)
			{
				firing = true;
				_col.enabled = true;
				_particles.enableEmission = true;
			}

			_depleteTime -= Time.deltaTime;
			Debug.Log(_depleteTime);
			if(_depleteTime <= 0)
			{
				canFire = false;
				EndFire();
			}
		}

		public void EndFire()
		{
			firing = false;
			_col.enabled = false;
			//_particles.Stop();
			_particles.enableEmission = false;
		}

		public void NewWeponPickdUp()
		{
			canFire = true;
			_depleteTime = INIT_DEPLETE_TIME;
		}
	}
}
