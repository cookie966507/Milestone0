using UnityEngine;
using System.Collections;

namespace CompleteProject
{
	public class Pickup : MonoBehaviour
	{
		public PlayerShooting.AttackType type = PlayerShooting.AttackType.None;
		public int index = 0;
		public GameObject pickupParticles;

		private IconManager iconManager;

		void Start()
		{
			iconManager = GameObject.Find("IconSpawns").GetComponent<IconManager>();
			Destroy(this.transform.root.gameObject, 10f);
		}

		void OnDisable()
		{
			iconManager.UpdateFilledSpace(index);
		}

		public void Collected()
		{
			GameObject particles = (GameObject)Instantiate(pickupParticles, transform.root.position, transform.rotation);
			Destroy (particles, 1.5f);
		}
	}
}
