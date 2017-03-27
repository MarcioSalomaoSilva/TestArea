using UnityEngine;
using System.Collections;

public class TEST_ApplyGravity : MonoBehaviour {

	public float thrust;
	public Rigidbody rb;

	public Transform target;

	void Start() {
		rb = GetComponent<Rigidbody>();
	}
	void Update (){
		transform.LookAt(target);
	}
	void FixedUpdate() {
		rb.AddForce(TEST_Gravity.Instance.directionOfGravity);
	}
}
