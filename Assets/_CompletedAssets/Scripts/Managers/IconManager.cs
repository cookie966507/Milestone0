using UnityEngine;
using System.Collections;
using CompleteProject;

public class IconManager : MonoBehaviour
{
	public GameObject[] icons;
	public float spawnTime = 3f;
	public float destroyTime = 5f;
	public Transform[] spawnPoints;
	public bool[] spaceFilled;
	
	
	void Start ()
	{
		// Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
		InvokeRepeating ("Spawn", spawnTime, spawnTime);
		spaceFilled = new bool[spawnPoints.Length];
	}
	
	
	void Spawn ()
	{
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);
		if(spaceFilled[spawnPointIndex]) return;
		int icon = Random.Range (0, icons.Length);
		GameObject newIcon = (GameObject)Instantiate (icons[icon], spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
		spaceFilled[spawnPointIndex] = true;
		newIcon.GetComponentInChildren<Pickup>().index = spawnPointIndex;
	}

	public void UpdateFilledSpace(int index)
	{
		spaceFilled[index] = false;
	}
}
