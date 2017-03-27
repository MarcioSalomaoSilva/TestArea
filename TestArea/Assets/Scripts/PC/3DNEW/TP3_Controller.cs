using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
[RequireComponent (typeof (TP3_Animator))]
[RequireComponent (typeof (Animator))]
public class TP3_Controller : MonoBehaviour {
	
	public static TP3_Controller Instance;
	
	
	public float backwardSpeed = 10f;
	public float strafingSpeed = 15f;
	public float crouchSpeed = 5f;
	float sensitivity;
	public float controllerSensitivity = 1f;
	public float mouseSensitivity = 3f;

	float verticalLookRotation;

	//new
	//
	private CharacterController controller;

	//
	public float deadZone = 0.1f;
	public float gravity = 21f;
	public float moveSpeed;
	public float runspeed = 3f;
	public float sprintSpeed = 5f;
	public float moveSpeedSmooth = 0.3f;
	public float jumpSpeed = 30f;
	public float lookClamp = 70f;
	//player direction
	private float direction = 0f;
	private float verticalSpeed;
	private float terminalVelocity = 20f;
	private float verticalVelocity { get; set; }
	private float directionSpeed = 0.0f;
	public float characterAngle = 0f;
	private float rotationAngle;
	private float targetRotation;
	private float acceleration;
	//camera
	public GameObject mainCamera;
	//targets
	public GameObject thirdPersonTarget;
	public GameObject thirdPersonLeft;
	public GameObject thirdPersonRight;
	public GameObject firstPersonTarget;

	//
	private Transform childModel;

	//
	public Vector3 moveVector;
	public Vector3 currentMoveVector;
	public Vector3 moveDirection;
	private Vector3 lastDirection;
	private Vector3 moveSmoothing;

	//

	
	//bools for camera
	public bool walkBool = false;
	public bool focusBool = false;
	public bool fpsBool = false;
	public bool crouchBool = false;
	public bool targetBool = false;
	//bools for input
	private bool isGrounded = false;
	public bool running = false;
	
	void Awake ()
	{
		Instance = this;
		//
		CreateCameraTargets ();
		CreateOrUseCamera ();
		//
		childModel = transform.FindChild ("Y_Bot");
		//
		controller = GetComponent<CharacterController> ();
		//
		terminalVelocity = gravity + 1f;
	}
	//
	void Start () 
	{
	}
	//
	void Update () 
	{
		GetInput ();
		//
		//childModel.localRotation = this.transform.rotation;
	}
	//
	void FixedUpdate ()
	{

	}
	//
	void LateUpdate () 
	{
		
	}
	//
	public void GetInput ()
	{
		//movement
		moveVector = new Vector3 (Input.GetAxisRaw ("LHorizontal"), 0, Input.GetAxisRaw ("LVertical"));
		if (moveVector.magnitude > 1f) 
		{
			moveVector.Normalize();
		}
		Vector3 targetMoveVector = moveVector;
		targetMoveVector = mainCamera.transform.rotation * targetMoveVector;
		targetMoveVector.y = 0.0f;
		targetMoveVector.Normalize ();
		targetMoveVector *= moveVector.sqrMagnitude;
		//move speed
		moveSpeed = moveVector.sqrMagnitude * runspeed;
//		acceleration += Time.deltaTime * moveSpeedSmooth;
//		moveSpeed = Mathf.Lerp(0f, moveVector.sqrMagnitude * 1.5f, Time.deltaTime * moveSpeedSmooth);
		sprintSpeed = moveVector.sqrMagnitude * sprintSpeed;
		//smooth
		currentMoveVector = Vector3.SmoothDamp(currentMoveVector, targetMoveVector * moveSpeed, ref moveSmoothing, moveSpeedSmooth);
		Debug.Log (moveSpeed);
		//
		//angles for animator
//		Vector3 CameraDirection = mainCamera.transform.forward;
//		CameraDirection.y = 0.0f;
//		Quaternion referentialShift = Quaternion.FromToRotation (Vector3.forward, Vector3.Normalize(CameraDirection));
//		//convert movevector to worldspace
//		Vector3 moveDirection = referentialShift * currentMoveVector;
//		Vector3 rootDirection = transform.forward;
//		Vector3 axisSign = Vector3.Cross (moveDirection, rootDirection);
//		float angle = Mathf.Atan2 (moveVector.x,moveVector.z);
//		float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
//		//
//		if(!TP3_Animator.Instance.IsInPivot())
//		{
//			characterAngle = angleRootToMove;
//		}
//		angleRootToMove /= 2f;
//		direction = angleRootToMove * directionSpeed;
		//

		//character rotation
		//Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
		if (new Vector3(currentMoveVector.x, 0f, currentMoveVector.y).magnitude > 0.8f) 
		{
			targetRotation = Mathf.Atan2(currentMoveVector.x, currentMoveVector.z) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0f, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, targetRotation, ref rotationAngle, 30f * Time.deltaTime), 0f); 
			//transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, moveSpeedSmooth * Time.deltaTime); 
		}
		//other inputs
		if (controller.isGrounded == true) {

			//
			if (Input.GetButtonDown ("A")) {
				lastDirection = moveVector;
				TP3_Animator.Instance.animator.SetBool ("Jump", true);
				Jump();
			}
			//run
			if (Input.GetAxis ("XRTrigger") > deadZone) {
				focusBool = true;
				moveSpeed = Mathf.Lerp (moveSpeed, sprintSpeed, Time.deltaTime);
				TP3_Animator.Instance.animator.SetBool ("Sprint", true);
			} 
			//crouch
			if (Input.GetButtonDown ("X")) {

			} else {
				crouchBool = false;
			}
			//fire
			if (Input.GetButtonDown ("B")) {
				fpsBool = false;
			}
			//
			if (Input.GetButtonDown ("Y")) {
				fpsBool = true;
			}
			//
			if (Input.GetButtonDown ("RightPress")) {
				targetBool = false;
			}
			//roll input and feild of view change
			if (Input.GetAxis ("XLTrigger") > deadZone) {
				TP3_Animator.Instance.animator.SetBool ("crouch", true);
				crouchBool = true;
				controller.height = 1f; 
			}
		}

		//currentMoveVector.y = verticalSpeed;
		ApplyGravity ();
		//move
		controller.Move(currentMoveVector * moveSpeed * Time.deltaTime);
		//apply gravity
		Debug.Log (controller.isGrounded);
	}
	public void Jump()
	{
		if (controller.isGrounded) 
		{
			verticalSpeed = jumpSpeed;
		}
	}
	void ApplyGravity()
	{
		if (currentMoveVector.y > -terminalVelocity) 
		{
			currentMoveVector = new Vector3 (currentMoveVector.x,  - gravity , currentMoveVector.z);
		}
		if (controller.isGrounded  && currentMoveVector.y < -1f) 
		{
			currentMoveVector = new Vector3 (currentMoveVector.x, - 1f, currentMoveVector.z);
		}
	}
	//
	public void CreateOrUseCamera()
	{
		mainCamera = new GameObject ("Main Camera");
		mainCamera.AddComponent <Camera>();
		mainCamera.AddComponent <TP3_Camera> ( );
		mainCamera.tag = "MainCamera";
		//mainCamera.transform.parent = this.transform;
		mainCamera.GetComponent<Camera> ().nearClipPlane = 0.2f;
		mainCamera.transform.localPosition = new Vector3 (0f, TP3_Camera.Instance.distanceUp, - TP3_Camera.Instance.distance);
	}
	//
	public void CreateCameraTargets ()
	{
		thirdPersonTarget = new GameObject("Focus Target");
		thirdPersonTarget.transform.parent = this.transform;
		thirdPersonTarget.transform.localPosition = new Vector3 (0.0f, 1.2f, 0f);
		//add two focus targets for each side
		thirdPersonRight = new GameObject("Focus Target Right");
		thirdPersonRight.transform.parent = this.transform;
		thirdPersonRight.transform.localPosition = new Vector3 (-3f, 0.8f, 10f);
		thirdPersonLeft = new GameObject("Focus Target Left");
		thirdPersonLeft.transform.parent = this.transform;
		thirdPersonLeft.transform.position = new Vector3 (3f, 0.8f, 10f);
		//
		firstPersonTarget = new GameObject("FPS Target");
		firstPersonTarget.transform.parent = this.transform;
		firstPersonTarget.transform.localPosition = new Vector3 (-0.3f, 1.8f, -0.4f);
	}
}
