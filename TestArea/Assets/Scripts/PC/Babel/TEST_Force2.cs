using UnityEngine;
using System.Collections;

public class TEST_Force2 : MonoBehaviour {

	public static TEST_Force2 Instance;

	public Rigidbody rb;

	public float deadZone = 0.1f;
	public float gravity = 10f;
	public float jumpSpeed = -20f;
	public float moveSpeed = 10f;

	public GameObject centerOfGravity;
	public Vector3 directionOfGravity;
	public Vector3 moveVector;

	public bool isGrounded = false;

	void Awake ()
	{
		Instance = this;
		rb = GetComponent<Rigidbody>();
	}

	void Start () 
	{

	}

	void Update () 
	{
		transform.LookAt( centerOfGravity.transform );
		transform.Rotate( 90, 0, 0 );
		CalculateGravity ();
		ApplyGravity ();
		Debug.LogWarning (isGrounded);
	}

	void FixedUpdate ()
	{
		GetInput ();
		//rb.MovePosition(rb.position + transform.TransformDirection(moveVector) * moveSpeed * Time.deltaTime);
	}

	public void ApplyGravity ()
	{
		rb.AddForce(directionOfGravity * gravity * Time.deltaTime);
	}

	public void CalculateGravity ()
	{
		Debug.DrawLine(transform.position, centerOfGravity.transform.position, Color.red);
		directionOfGravity = transform.position - centerOfGravity.transform.position;
	}

	public void GetInput ()
	{
		moveVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
		if (moveVector.magnitude > deadZone) 
		{	
			moveVector += new Vector3 (0, 0, Input.GetAxis ("Vertical"));
			moveVector += new Vector3 (Input.GetAxis ("Horizontal"), 0, 0);
		}
		if (isGrounded == true) {
			if (Input.GetButtonDown ("Jump")) {
				gravity = -50f;
			}
		} else {
			gravity = 10f;
		}
		rb.MovePosition(rb.position + transform.TransformDirection(moveVector) * moveSpeed * Time.deltaTime);
	}

	public void Jump ()
	{

	}

	void OnCollisionEnter (Collision col)
	{
		isGrounded = true;
	}
	void OnCollisionExit (Collision col)
	{
		isGrounded = false;
	}
}
