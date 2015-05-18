using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

		Image energyBar; 

		void Awake()
		{
			_col = GetComponent<BoxCollider>();
			_col.enabled = false;

			playerShooting = GetComponentInParent <PlayerShooting> ();
			_particles.enableEmission = false;
			energyBar = GameObject.Find("Bar").GetComponent<Image>();
			energyBar.transform.localScale = new Vector3(0, 1, 1);
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
			float _scaleX = _depleteTime/INIT_DEPLETE_TIME * (-1);
			energyBar.transform.localScale = new Vector3(_scaleX, 1, 1);

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
			_particles.enableEmission = false;
		}

		public void NewWeponPickdUp()
		{
			canFire = true;
			_depleteTime = INIT_DEPLETE_TIME;
			energyBar.transform.localScale = new Vector3(-1, 1, 1);
		}
	}
}
