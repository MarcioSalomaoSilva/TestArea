using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (G_Controller))]

public class FPS_Controller : MonoBehaviour {

	public static FPS_Controller Instance;

	G_Controller gunController;

	public enum Direction{
		Stationary, Forward, Backward, Left, Right, Run,
		LeftForward, RightForward, LeftBackward, RightBackward, Jump, RunJump,
		Crouch
	}
	public Direction moveDirection {get; set;}

	public GameObject centerOfGravity;
	public GameObject mainCamera;
	
	bool isGrounded = false;
	bool running = false;
	bool crouching =false;
	public bool controllerActive = false;
	
	public float deadZone = 0.1f;
	public float gravity = 10f;
	public float jumpForce = 10f;
	float moveSpeed;
	public float forwardSpeed = 20f;
	public float runSpeed = 30f;
	public float backwardSpeed = 10f;
	public float strafingSpeed = 15f;
	public float crouchSpeed = 5f;
	float sensitivity;
	public float controllerSensitivity = 1f;
	public float mouseSensitivity = 3f;
	public float lookClamp = 70f;
	float velocity;
	float cameraY = 0f;
	float verticalLookRotation;
	
	Vector3 lastDirection;
	Vector3 directionOfGravity;
	Vector3 moveVector;
	Vector3 moveSmoothing;
	Vector3 targetMoveSmoothing;
	Vector2 rotateVector;

	
	void Awake ()
	{
		Instance = this;
		GetComponent<Rigidbody>().useGravity = false;
		GetComponent<Rigidbody>().isKinematic = false;
		GetComponent<Rigidbody>().detectCollisions = true;
		GetComponent<Rigidbody>().freezeRotation = true;
		//
		if (centerOfGravity == null) {
			CreateOrUseCenterofGravity ();
			Debug.LogWarning("Create a game object at origin or vector3.zero and assign it in the inspector");
		} 
		if (mainCamera == null) 
		{
			CreateOrUseCamera();
			Debug.LogWarning("Create and child a Camera to the to this game object, also assign it in the Inspector");
		}
	}
	//
	void Start () 
	{
	}
	//
	void Update () 
	{
		Debug.LogWarning (isGrounded);
		Grounded ();
		CameraInput ();
		ApplyGravity ();
		RotateToCenter ();
		CalculateDirection ();
		GetInput ();
	}
	//
	void FixedUpdate ()
	{
		GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().transform.position + transform.TransformDirection(moveVector) * Time.deltaTime);
	}
	//
	void LateUpdate () 
	{
		
	}
	//
	public void CameraInput ()
	{
		//controller detection
		if ( controllerActive == true) 
		{
			rotateVector = new Vector2 (Input.GetAxis ("RVertical"), Input.GetAxis ("RHorizontal")).normalized;
			sensitivity = controllerSensitivity;
		} else {
			rotateVector = new Vector2 (Input.GetAxis ("Mouse Y"),Input.GetAxis("Mouse X")).normalized;
			sensitivity = mouseSensitivity;
		}
		//crouching transform change
		if (crouching == true) 
		{
			cameraY = 0.3f;
		} else if (crouching == false)
		{
			cameraY = 0.9f;
		}
		float newY = Mathf.SmoothDamp(Camera.main.transform.localPosition.y, cameraY, ref velocity, 0.1f);
		mainCamera.transform.localPosition = new Vector3 (0, newY, 0);
		//clamp
		verticalLookRotation += rotateVector.x * sensitivity;
		verticalLookRotation = Mathf.Clamp (verticalLookRotation, -lookClamp, lookClamp);
		mainCamera.transform.localEulerAngles = Vector3.left * verticalLookRotation ;
	}
	//
	public void ApplyGravity ()
	{
		Debug.DrawLine(transform.position, centerOfGravity.transform.position, Color.red);
		directionOfGravity = (transform.position - centerOfGravity.transform.position);
		GetComponent<Rigidbody>().AddForce(directionOfGravity * gravity * Time.deltaTime);
	}
	//
	public void RotateToCenter ()
	{
		Vector3 direction = directionOfGravity.normalized;
		transform.rotation = Quaternion.FromToRotation (-transform.up, direction) * transform.rotation;
	}
	//
	public void Grounded ()
	{
		Ray ray = new Ray (transform.position, -transform.up);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 2f)) 
		{
			isGrounded = true;
		} else {
			isGrounded = false;
		}
		Debug.LogWarning (hit.distance);
	}
	//
	public void GetInput ()
	{
		//rotation
		transform.Rotate (Vector3.up * rotateVector.y * sensitivity);
		//movement
		if (controllerActive == true) 
		{
			moveVector = new Vector3 (Input.GetAxis ("LHorizontal"), 0, Input.GetAxis ("LVertical")).normalized;
			if (moveVector.magnitude > deadZone) 
			{	
				moveVector += new Vector3 (0, 0, Input.GetAxis ("LVertical"));
				moveVector += new Vector3 (Input.GetAxis ("LHorizontal"), 0, 0);
			}
		} else {
			moveVector = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")).normalized;	
			moveVector += new Vector3 (0, 0, Input.GetAxis ("Vertical"));
			moveVector += new Vector3 (Input.GetAxis ("Horizontal"), 0, 0);
		}
		//jump
		if (isGrounded == true) {
			if (Input.GetButtonDown ("Jump")) {
				GetComponent<Rigidbody> ().AddForce (directionOfGravity * -jumpForce);
				lastDirection = moveVector;
			}
		} else {
			moveVector = lastDirection;
		}
		//run
		if (Input.GetButtonDown ("Run")) {
			running = true;
		} else if (Input.GetButtonUp("Run")){
			running = false;
		}
		//crouch
		if (Input.GetButtonDown ("Crouch")) {
			crouching = true;
			GetComponent<CapsuleCollider>().height = 1f; 
		} else if (Input.GetButtonUp ("Crouch")) 
		{
			crouching = false;
			GetComponent<CapsuleCollider>().height = 2.5f; 
		}
		//fire
		if(Input.GetButtonDown ("Fire"))
		{
			G_Controller.Instance.Shoot();
		}
		//speed calc and smoothing
		switch (moveDirection) 
		{	
		case Direction.Stationary:
			moveSpeed = forwardSpeed;
			break;
		case Direction.Forward:
			moveSpeed = forwardSpeed;
			break;	
		case Direction.Backward:
			moveSpeed = backwardSpeed;
			break;	
		case Direction.Left:
			moveSpeed = strafingSpeed;
			break;	
		case Direction.Right:
			moveSpeed = strafingSpeed;
			break;	
		case Direction.LeftForward:
			moveSpeed = forwardSpeed;
			break;	
		case Direction.RightForward:
			moveSpeed = forwardSpeed;
			break;	
		case Direction.LeftBackward:
			moveSpeed = backwardSpeed;
			break;	
		case Direction.RightBackward:
			moveSpeed = backwardSpeed;
			break;	
			//running
		case Direction.Run:
			moveSpeed = runSpeed;
			break;
		case Direction.Crouch:
			moveSpeed = crouchSpeed;
			break;
		}
		Debug.LogWarning (moveSpeed);
		moveVector = moveVector * moveSpeed;
		moveSmoothing = Vector3.SmoothDamp (moveSmoothing, moveVector, ref targetMoveSmoothing, .15f);
	}
	//calculates the direction of the player 
	public void CalculateDirection ()
	{
		var forward = false;
		var backward = false;
		var left = false;
		var right = false;
		var jump = false;
		var run = false;
		var crouch = false;
		if (moveVector.z > 0f) 
		{
			forward = true;
		}
		if (moveVector.z < 0f) 
		{
			backward = true;
		}
		if (moveVector.x < 0f) 
		{
			left = true;
		}
		if (moveVector.x > 0f) 
		{
			right = true;
		}
		if (isGrounded == false) 
		{
			jump = true;
		}
		if (running == true) 
		{
			run = true;
		}
		if(crouching == true)
		{
			crouch = true;
		}
		//
		if (forward) 
		{
			if (left) 
			{
				moveDirection = Direction.LeftForward;
				if(crouch)
				{
					moveDirection = Direction.Crouch;
				}
				if (run) {
					moveDirection = Direction.Run;
					if(crouch)
					{
						moveDirection = Direction.Crouch;
					}
				}
			} else if (right) 
			{
				moveDirection = Direction.RightForward;
				if(crouch)
				{
					moveDirection = Direction.Crouch;
				}
				if(crouch)
				{
					moveDirection = Direction.Crouch;
				}
				if (run) {
					moveDirection = Direction.Run;
					if(crouch)
					{
						moveDirection = Direction.Crouch;
					}
				}
			} else if (run) {
				moveDirection = Direction.Run;
				if(crouch)
				{
					moveDirection = Direction.Crouch;
				}
			}else {
				moveDirection = Direction.Forward;
				if(crouch)
				{
					moveDirection = Direction.Crouch;
				}
			}
		} else if (backward) 
		{
			if (left) 
			{
				moveDirection = Direction.LeftBackward;
				if(crouch)
				{
					moveDirection = Direction.Crouch;
				}
			} else if (right) 
			{
				moveDirection = Direction.RightBackward;
				if(crouch)
				{
					moveDirection = Direction.Crouch;
				}
			} else 
			{
				moveDirection = Direction.Backward;
				if(crouch)
				{
					moveDirection = Direction.Crouch;
				}
			}
		} else if (left) 
		{
			moveDirection = Direction.Left;
			if(crouch)
			{
				moveDirection = Direction.Crouch;
			}
		} else if (right) 
		{
			moveDirection = Direction.Right;
			if(crouch)
			{
				moveDirection = Direction.Crouch;
			}
		} else 
		{
			moveDirection = Direction.Stationary;
			if(crouch)
			{
				moveDirection = Direction.Crouch;
			}
		}
	}
	//
	public void CreateOrUseCamera()
	{
		mainCamera = new GameObject ("Main Camera");
		mainCamera.AddComponent <Camera>();
		mainCamera.tag = "MainCamera";
		//		mainCamera.transform.parent = transform;
		//		mainCamera.transform.position = transform.position + Vector3.up;
	}
	//
	public void CreateOrUseCenterofGravity ()
	{
		centerOfGravity = new GameObject ("Center");
		centerOfGravity.transform.position = Vector3.zero;
	}
}
