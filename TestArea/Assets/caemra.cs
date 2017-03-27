using UnityEngine;
using System.Collections;

public class caemra : MonoBehaviour {

	Transform player;
	Quaternion targetLook;
	Vector3 targetMove;
	public float smoothLook= 0.1f;
	public float smoothMove = 0.1f;
	private Vector3 smoothRef;
	public float distanceUp = 2f;
	public float distance = 2f;
	public float distanceMin = 1f;
	public float distanceMax = 5f;

	void Start () 
	{
		player = GameObject.Find ("PC").transform;

	}

	void Update () 
	{
		targetMove = player.position + (player.rotation * new Vector3(0f , 0f, -distance));

		if (distance >= distanceMax) {
//			transform.position = Vector3.Lerp (transform.position, targetMove, smoothMove * Time.deltaTime);
			transform.position = Vector3.SmoothDamp(transform.position, targetMove, ref smoothRef, smoothMove * Time.deltaTime);
		}else if (distance <= distanceMin)
		{
//			transform.position = Vector3.Lerp (transform.position, targetMove, smoothMove * Time.deltaTime);
			transform.position = Vector3.SmoothDamp(transform.position, targetMove, ref smoothRef, smoothMove * Time.deltaTime);
		}

		targetLook = Quaternion.LookRotation(player.position - transform.position,transform.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetLook, smoothLook * Time.deltaTime);
	}
}
