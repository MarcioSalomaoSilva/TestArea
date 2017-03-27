using UnityEngine;
using System.Collections;

public class G_Projectile : MonoBehaviour {

	public static G_Projectile Instance;

	float speed;

	void Awake ()
	{
		Instance = this;
		speed = G_StartingGun.Instance.projectileVelocity;
	}
	//once per frame
	void Update () {
		transform.Translate (Vector3.forward * Time.deltaTime * speed);
	}
}
