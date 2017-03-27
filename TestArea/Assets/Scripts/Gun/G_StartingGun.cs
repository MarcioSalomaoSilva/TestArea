using UnityEngine;
using System.Collections;

public class G_StartingGun : MonoBehaviour {

	public static G_StartingGun Instance;

	public Transform muzzle;

	public GameObject projectile;

	public float rateOfFire = 100f;
	public float projectileVelocity = 35;

	float nextShotTime;

	void Awake()
	{
		Instance = this;
	}
	void Start ()
	{
		if(projectile == null)
		{
			Debug.LogError ("Assign a projectile prefab in the inspector, make sure it has the G_Projectile script attached to it");
		}
	}
	public void Shoot()
	{
		if (Time.time > nextShotTime) 
		{
			nextShotTime = Time.time + rateOfFire / 1000f;
			GameObject newProjectile = Instantiate (projectile, muzzle.position, muzzle.rotation) as GameObject;
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
