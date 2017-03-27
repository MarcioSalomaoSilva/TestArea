using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Animator))]
[RequireComponent (typeof (Rigidbody))]
[RequireComponent(typeof(TP2_Animator))]
public class TP2_Controller : MonoBehaviour {

	public static TP2_Controller Instance;

	public float gravity = 10;
	public float moveSpeed = 0.0f;
	public float sprintSpeed = 7f;
	public float deadZone = 0.1f;
	public float directionDampTime = 0.25f;
	private float rotationDegreePerSecond = 120f;
	private float directionSpeed = 0.0f;
	private float direction = 0f;
	private float characterAngle = 0f;
	private float speedDamp = 3f;
	private float jumpDistance = 3f;
	private float jumpMultiplier = 1f;
	private float capsuleHeight;

	public Vector3 moveVector;
	private Vector3 moveSmoothing;
	private Vector3 targetMoveSmoothing;
	private Vector3 rotationAmount;
	private Quaternion deltaRotation;

	public GameObject mainCamera;
	public GameObject thirdPersonTarget;
	public GameObject firstPersonTarget;
	public GameObject headTarget;
	//detect controller bool
	public bool controllerActive = false;

	private AnimatorStateInfo stateInfo;
	private AnimatorTransitionInfo transInfo;

	private CapsuleCollider collider;

	void Awake () 
	{
		Instance = this;
		GetComponent<Rigidbody>().useGravity = true;
		GetComponent<Rigidbody>().isKinematic = false;
		GetComponent<Rigidbody>().detectCollisions = true;
		GetComponent<Rigidbody>().freezeRotation = true;
		//get capsule collider
		collider = GetComponent<CapsuleCollider>();
		capsuleHeight = collider.height;
		//get camera
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		//create target game objects. first person.
		firstPersonTarget = new GameObject("First Person Target");
		firstPersonTarget.transform.parent = this.transform;
		firstPersonTarget.transform.position = new Vector3 (-0.35f, 0.65f, -0.4f);
		//and third person
		thirdPersonTarget = new GameObject("Third Person Target");
		thirdPersonTarget.transform.parent = this.transform;
		thirdPersonTarget.transform.position = new Vector3 (0.0f, 0.4f, 0f);
	}
	void Start () 
	{
		//
		headTarget = TP2_Camera.Instance.headTarget;
	}
	//
	void Update () 
	{
		GetInput ();
	}
	//
	void FixedUpdate ()
	{
		//physics
//		Vector3 directionOfGravity = -Vector3.up;
//		GetComponent<Rigidbody>().MovePosition(directionOfGravity * gravity * Time.deltaTime);
//		GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().transform.position + transform.TransformDirection(moveVector) * Time.deltaTime);
//		GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().transform.position + transform.TransformVector(moveVector) * Time.deltaTime);
		if (TP2_Animator.Instance.IsInLocomotion() && TP2_Camera.Instance.mainCamState != TP2_Camera.mainCamStates.Free && !TP2_Animator.Instance.IsInPivot() && ((direction >= 0 && moveVector.x >= 0) || (direction < 0 && moveVector.x < 0)))
		{
			Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (moveVector.x < 0f ? -1f : 1f), 0f), Mathf.Abs(moveVector.x));
			Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
			this.transform.rotation = (this.transform.rotation * deltaRotation);

		}
		//transform.Rotate(Vector3.up * moveVector.x * directionDampTime);
		//
		if (TP2_Animator.Instance.IsInJump())
		{
			float oldY = transform.position.y;
			transform.Translate(Vector3.up * jumpMultiplier * TP2_Animator.Instance.animator.GetFloat("JumpCurve"));
			if (TP2_Animator.Instance.IsInLocomotionJump())
			{
				transform.Translate(Vector3.forward * Time.deltaTime * jumpDistance);
			}
			collider.height = capsuleHeight + (TP2_Animator.Instance.animator.GetFloat("CapsuleCurve") * 0.5f);
		}
	}
	//
	void LateUpdate () 
	{
		
	}
	//
	public void GetInput ()
	{
		if (TP2_Animator.Instance.animator && TP2_Camera.Instance.mainCamState != TP2_Camera.mainCamStates.FirstPerson) 
		{
			//jump input and feild of view change
			if (Input.GetAxis("XLTrigger") > deadZone )
			{
				TP2_Animator.Instance.animator.SetBool("Jump", true);
			}
			else
			{
				TP2_Animator.Instance.animator.SetBool("Jump", false);
			}
			//
			Debug.Log(Camera.main.fieldOfView);
			//stateinfo
			stateInfo = TP2_Animator.Instance.animator.GetCurrentAnimatorStateInfo(0);
			transInfo = TP2_Animator.Instance.animator.GetAnimatorTransitionInfo(0);
			//movement
			characterAngle = 0f;
			direction = 0f;
			//float charSpeed = 0f;
			moveVector = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical") );
			if (moveVector.magnitude > deadZone) {	
				moveVector += new Vector3 (0, 0, Input.GetAxis ("Vertical"));
				moveVector += new Vector3 (Input.GetAxis ("Horizontal"), 0, 0);
			}
			//Translate movevector into world/cam/character space
			StickToWorldSpace (this.transform, mainCamera.transform, ref direction, ref moveSpeed, ref characterAngle,  TP2_Animator.Instance.IsInPivot());
			//sprint input
			if (Input.GetAxis("XRTrigger") > deadZone * 2f && TP2_Camera.Instance.mainCamState == TP2_Camera.mainCamStates.Focus)
			{
				TP2_Animator.Instance.animator.SetBool("Sprint", true);
				moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, Time.deltaTime);
				Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, TP2_Camera.Instance.sprintFOV, 3f *Time.deltaTime);
				direction = 0f ;
			}
			else 
			{
				moveSpeed = moveSpeed;
				TP2_Animator.Instance.animator.SetBool("Sprint", false);
				Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, TP2_Camera.Instance.targetFOV, 3f *Time.deltaTime);
			}

			//movespeed
			TP2_Animator.Instance.animator.SetFloat("Speed", moveSpeed, speedDamp, Time.deltaTime);
			TP2_Animator.Instance.animator.SetFloat("Direction", direction, 0.22f, Time.deltaTime);
			//speed and pivot conditions
			if(moveSpeed > deadZone)
			{
				if(!TP2_Animator.Instance.IsInPivot())
				{
					TP2_Animator.Instance.animator.SetFloat("Angle", characterAngle);
				}
			}
			if(moveSpeed < deadZone && Mathf.Abs(TP2_Controller.Instance.moveVector.x) < 0.05f)
			{
				TP2_Animator.Instance.animator.SetFloat("Direction", 0f);
				TP2_Animator.Instance.animator.SetFloat("Angle", 0f);
			}
		}
	}
	public void StickToWorldSpace (Transform root, Transform camera, ref float directionOut, ref float speedOut, ref float angleOut, bool isPivoting)
	{
		Vector3 rootDirection = root.forward;
		Vector3 stickDirection = moveVector;
		speedOut = stickDirection.sqrMagnitude  ;
		//get camera rotation
		Vector3 CameraDirection = camera.forward;
		CameraDirection.y = 0.0f;
		Quaternion referentialShift = Quaternion.FromToRotation (Vector3.forward, Vector3.Normalize(CameraDirection));
		//convert movevector to worldspace
		Vector3 moveDirection = referentialShift * stickDirection;
		Vector3 axisSign = Vector3.Cross (moveDirection, rootDirection);
		//debug
		Debug.DrawRay (new Vector3 (root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
		Debug.DrawRay (new Vector3 (root.position.x, root.position.y + 2f, root.position.z), axisSign, Color.red);
		Debug.DrawRay (new Vector3 (root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
		Debug.DrawRay (new Vector3 (root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);
		Debug.DrawRay (new Vector3 (root.position.x, root.position.y + 2f, root.position.z), TP2_Camera.Instance.lookDirection, Color.black);
		//
		float angle = Mathf.Atan2 (moveVector.x,moveVector.z);
		float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
		if(!isPivoting)
		{
			angleOut = angleRootToMove;
		}
		angleRootToMove /= 180f;
		directionOut = angleRootToMove * directionSpeed;
	}
}
