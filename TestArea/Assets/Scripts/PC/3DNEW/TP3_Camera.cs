using UnityEngine;
using System.Collections;

public class TP3_Camera : MonoBehaviour {

	public static TP3_Camera Instance;

	public enum CamStates
	{
		Behind, FirstPerson, Focus, Free, Target, Crouching
	}

	public CamStates camState = CamStates.Free;

	//
	Quaternion targetLook;
	Component dof;
	Component vaca;
	Vector3 targetPosition;
	Vector3 smoothRef;
	Vector3 targetMove;
	float smoothMove = 3f;
	float smoothLook = 4f;
	float lookClamp = 70f;
	float verticalLookRotation;
	float velocity;
	float deadZone = 0.1f;
	//distance of camera
	public float distance = 4f;
	public float distanceMin = 0.5f;
	public float distanceMax = 6f;
	public float distanceUp = 2f;
	//field of view values
	public float sprintFOV = 10f;
	public float targetFOV = 35f;
	public float behindFOV = 60f;
	public float targetingSpeed = 3f;
	//camera damp
	private float freeRotation = -3f;
	//characters move speed
	private float moveSpeed;
	//targets
	public GameObject thirdTargetLeft;
	public GameObject thirdTargetRight;
	public GameObject firstPersonTarget;
	public GameObject thirdPersonTarget;
	//player object
	public GameObject playerCharacter;
	//ik target
	public GameObject ikHeadTarget;
	//input vector
	public Vector3 rotateVector;
	//look at input for targets
	public Vector3 lookAt;

	void Awake ()
	{
		Instance = this;
	}
	void Start () 
	{
		//set initial cam state
		SetVariables ();
	}
	void Update () 
	{

	}
	void LateUpdate () 
	{
		//
		CameraInput ();
		StateConditions ();
		//
		targetMove = thirdPersonTarget.transform.position + (thirdPersonTarget.transform.rotation * new Vector3(0f , distanceUp, -distance));
		//
		distance = Vector3.Distance (transform.position, thirdPersonTarget.transform.position);
		//
		distance = Mathf.Clamp (distance, distanceMin, distanceMax);
		//
		switch (camState) 
		{
		case CamStates.Behind:
			//
			var behindMove = transform.position;
			//
			if (distance >= distanceMax) {
				//transform.position = Vector3.Lerp (transform.position, targetMove, smoothMove * Time.deltaTime);
				transform.position = Vector3.SmoothDamp(transform.position, targetMove, ref smoothRef, smoothMove * Time.deltaTime);
			}
//			else if (distance <= distanceMin)
//			{
//				//transform.position = Vector3.Lerp (transform.position, targetMove, smoothMove * Time.deltaTime);
//				transform.position = Vector3.SmoothDamp(transform.position, targetMove, ref smoothRef, smoothMove * Time.deltaTime);
//			}
			//
			behindMove = transform.position;
			//
			targetLook = Quaternion.LookRotation(thirdPersonTarget.transform.position - transform.position, Vector3.up);
			transform.rotation = Quaternion.Lerp(transform.rotation, targetLook, smoothLook * Time.deltaTime);
			break;
		case CamStates.FirstPerson:
			//field of view change
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, targetFOV, targetingSpeed * Time.deltaTime);
			//clamp
			verticalLookRotation -= rotateVector.y * smoothMove;
			verticalLookRotation = Mathf.Clamp (verticalLookRotation, -lookClamp, lookClamp);
			transform.localEulerAngles = Vector3.left * verticalLookRotation ;
			//
			playerCharacter.transform.Rotate(Vector3.up * rotateVector.x * smoothMove);
			//
			targetPosition = firstPersonTarget.transform.position;
			//
			lookAt = Vector3.Slerp(targetPosition + thirdPersonTarget.transform.forward, this.transform.position + this.transform.forward, smoothMove * Time.deltaTime);
			//choose look at target based on distance
			lookAt = Vector3.Lerp (this.transform.position + this.transform.forward, lookAt, Vector3.Distance(this.transform.position, firstPersonTarget.transform.position));
			//rotate camera
			this.transform.RotateAround(thirdPersonTarget.transform.position, thirdPersonTarget.transform.right, -0.3f * rotateVector.y);
			this.transform.RotateAround(thirdPersonTarget.transform.position, thirdPersonTarget.transform.up, -0.3f * rotateVector.x );
			//
			break;
		case CamStates.Target:
			//camera
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, targetFOV, targetingSpeed * Time.deltaTime);
			//ResetCamera ();
			//lookDirection = thirdPersonTarget.transform.forward; // vector 3 cross between different targets
			//targetPosition = characterOffset  * distanceUp - lookDirection * distance;
			targetMove = thirdPersonTarget.transform.position + (thirdPersonTarget.transform.rotation * new Vector3(0f , distanceUp, -distance));
			//rotate camera
			this.transform.RotateAround(thirdPersonTarget.transform.position, thirdPersonTarget.transform.right, -0.3f * rotateVector.y);
			this.transform.RotateAround(thirdPersonTarget.transform.position, thirdPersonTarget.transform.up, -0.3f * rotateVector.x );
			//
			break;
		case CamStates.Free:
			//camera
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, behindFOV, targetingSpeed * Time.deltaTime);
			//
			transform.LookAt (thirdPersonTarget.transform.position);
			//targetLook = Quaternion.LookRotation(thirdPersonTarget.transform.position - transform.position, thirdPersonTarget.transform.up);
			//center of rotation
			targetMove = thirdPersonTarget.transform.position + (thirdPersonTarget.transform.rotation * new Vector3(0f ,0f, -distance));
			//rotate camera
			this.transform.RotateAround(thirdPersonTarget.transform.position, thirdPersonTarget.transform.right, freeRotation * rotateVector.y);
			this.transform.RotateAround(thirdPersonTarget.transform.position, thirdPersonTarget.transform.up, freeRotation* rotateVector.x );
			//
			break;
		case CamStates.Focus:
			ResetCamera ();
			//set camera direction forward
			targetLook = thirdPersonTarget.transform.rotation;
			//currentLookDirection = thirdPersonTarget.transform.forward;
			// set target position of camera
			//targetPosition = characterOffset + thirdPersonTarget.transform.up * distanceUP - thirdPersonTarget.transform.forward * distanceAway;
			//if statements for targets
			//targetPosition = (characterOffset + thirdPersonTarget.transform.up * distanceUp - lookDirection * distanceAway) + new Vector3 (1f,0f,0f);
			//currentLookDirection = thirdPersonTarget.transform.forward;
			targetLook = Quaternion.LookRotation(thirdPersonTarget.transform.position - transform.position, Vector3.up);
			targetPosition = thirdPersonTarget.transform.position + (thirdPersonTarget.transform.rotation * new Vector3(0f , thirdPersonTarget.transform.position.y, -distance));
			//rotate camera
			this.transform.RotateAround(thirdPersonTarget.transform.position, thirdPersonTarget.transform.right, freeRotation * rotateVector.y);
			this.transform.RotateAround(thirdPersonTarget.transform.position, thirdPersonTarget.transform.up, freeRotation * rotateVector.x );
			//
			break;
		case CamStates.Crouching:

			break;
		}
		//collision
		CompensateForWalls (thirdPersonTarget.transform.position, ref targetPosition);
		//smoothing camera movement
		//smoothPosition(this.transform.position, targetMove);
		//make sure camera is looking correctly
		//transform.LookAt (lookAt);
		//save previous frame
		//rightStickSave = new Vector2 (rotateVector.x, rotateVector.y);
		Debug.Log (camState);
	}
	public void CameraInput ()
	{
		//input stick
		rotateVector = new Vector2 (Input.GetAxis ("RHorizontal"), Input.GetAxis ("RVertical")).normalized;
		//get cases
		CaseCamStates ();
	}
	void StateConditions ()
	{
		//
		if (TP3_Controller.Instance.moveVector.magnitude > deadZone) 
		{
			TP3_Camera.Instance.camState = TP3_Camera.CamStates.Behind;
			if (TP3_Controller.Instance.targetBool) 
			{
				camState = CamStates.Target;
			}
			if (!TP3_Animator.Instance.IsInPivot ()) 
			{
				TP3_Animator.Instance.animator.SetFloat ("Angle", TP3_Controller.Instance.characterAngle);
			}
			if (TP3_Controller.Instance.moveVector.magnitude < deadZone && Mathf.Abs (TP3_Controller.Instance.moveVector.x) < 0.05f) 
			{
				TP3_Animator.Instance.animator.SetFloat ("Direction", 0f);
				TP3_Animator.Instance.animator.SetFloat ("Angle", 0f);
			} 
		}			
		else if (TP3_Controller.Instance.fpsBool) 
		{
			camState = CamStates.FirstPerson;
		} 
		else if (rotateVector.magnitude > deadZone && TP3_Controller.Instance.moveVector.magnitude < deadZone && camState != CamStates.FirstPerson)
		{
			camState = CamStates.Free;
		}
	}
	void CaseCamStates ()
	{

	}
	private void smoothPosition(Vector3 fromPos, Vector3 toPos)
	{
		this.transform.position = Vector3.SmoothDamp (fromPos, toPos, ref smoothRef, smoothMove);
	}
	private void CompensateForWalls (Vector3 fromObject, ref Vector3 toTarget)
	{
		Debug.DrawLine (fromObject, toTarget, Color.black);
		//compensate for walls between camera
		RaycastHit wallHit = new RaycastHit ();
		if (Physics.Linecast (fromObject, toTarget, out wallHit)) 
		{
			Debug.DrawRay(wallHit.point, Vector3.left, Color.red);
			toTarget = new Vector3(wallHit.point.x, toTarget.y, wallHit.point.z);
		}
	}
	private void ResetCamera ()
	{
		transform.localRotation = Quaternion.Lerp (transform.localRotation, Quaternion.identity, Time.deltaTime);
	}
	void SetVariables ()
	{
		//get movespeed from other script
		moveSpeed = TP3_Controller.Instance.moveSpeed;
		//targets
		thirdPersonTarget = TP3_Controller.Instance.thirdPersonTarget;
		thirdTargetLeft = TP3_Controller.Instance.thirdPersonLeft;
		thirdTargetRight = TP3_Controller.Instance.thirdPersonRight;
		firstPersonTarget = TP3_Controller.Instance.firstPersonTarget;
		//and head ik target
		ikHeadTarget = new GameObject("Head Target;");
		ikHeadTarget.transform.parent = this.transform;
		ikHeadTarget.transform.position = new Vector3 (0.0f, 0.7f, 8f);
		//initial cam state
		camState = CamStates.Behind;
		//set player character Object
		playerCharacter = GameObject.FindWithTag("Player");
		//
	}
}
