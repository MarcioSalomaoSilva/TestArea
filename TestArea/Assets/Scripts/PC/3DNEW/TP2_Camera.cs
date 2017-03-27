using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(UnityStandardAssets.ImageEffects.DepthOfField))]
public class TP2_Camera : MonoBehaviour {

	public static TP2_Camera Instance;

	public enum mainCamStates
	{
		Behind, FirstPerson, Focus, Free, Target
	}

	TP2_Controller playerCharacter;

	public float distanceAway;
	public float distanceAwayMultiplier = 3f;
	public float distanceUp = 0.05f;
	public float distanceUpMultiplier = 5f;
	public float smooth;
	public float deadZone = 0.1f;
	public float cameraSensitivity = 1.5f;

	private float horizontal;
	private float vertical;
	public float sprintFOV = 10f;
	public float targetFOV = 35f;
	public float behindFOV = 60f;
	public float targetingSpeed = 3f;
	private float xAxisRot = 0.0f;
	private float camSmoothDampTime = 0.1f;
	private float freeRotation = -5f;
	private float freeDistanceUp;
	private float freeDistanceAway;
	private float lastStickMin = float.PositiveInfinity;	// Used to prevent from zooming in when holding back on the right stick/scrollwheel

	public GameObject camera;
	public GameObject thirdPersonTarget;
	public GameObject headTarget;
	public GameObject focusTarget;
	public GameObject focusTargetLeft;
	public GameObject focusTargetRight;
	private GameObject firstPersonTarget;
	private GameObject behindLeftTarget;
	private GameObject behindRightTarget;

	private bool firstPersonBool = false;
	private bool resetBool = false;
	private bool freeBool = false;
	private bool behindBool = false;

	public Vector2 rotateVector;
	public Vector3 lookDirection;
	private Vector3 characterOffset;
	private Vector3 leftCharacterOffset;
	private Vector3 rightCharacterOffset;
	private Vector3 targetPosition;
	private Vector3 currentLookDirection;
	private Vector3 velocityCamSmooth = Vector3.zero;
	private Vector3 savedRigToGoal;
	private Vector2 firstPersonClamp = new Vector2(-70.0f,30.0f);
	private Vector2 camMinDistance = new Vector2(1f,-0.5f);
	private Vector2 rightStickSave = Vector2.zero;

	//test
	float distanceMin = 2f;
	float distanceMax = 5f;

	public UnityStandardAssets.ImageEffects.DepthOfField dof;
	public UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration vaca;
	public mainCamStates mainCamState = mainCamStates.Behind;

	public Vector3 RigToGoalDirection
	{
		get
		{
			// Move height and distance from character in separate parentRig transform since RotateAround has control of both position and rotation
			Vector3 rigToGoalDirection = Vector3.Normalize(characterOffset - this.transform.position);
			// Can't calculate distanceAway from a vector with Y axis rotation in it; zero it out
			rigToGoalDirection.y = 0f;
			
			return rigToGoalDirection;
		}
	}

	//initialize
	void Awake () 
	{
		//instance
		Instance = this;
		//find camera
		camera = GameObject.FindGameObjectWithTag ("MainCamera");
		//and head ik target
		headTarget = new GameObject("Head Target;");
		headTarget.transform.parent = this.transform;
		headTarget.transform.position = new Vector3 (0.0f, 0.7f, 8f);
		// and focus target
		focusTarget = new GameObject("Focus Target");
		focusTarget.transform.parent = this.transform;
		focusTarget.transform.position = new Vector3 (0f, 0.8f, 15f);
		//add two focus targets for each side
		focusTargetRight = new GameObject("Focus Target Right");
		focusTargetRight.transform.parent = this.transform;
		focusTargetRight.transform.position = new Vector3 (-3f, 0.8f, 15f);
		focusTargetLeft = new GameObject("Focus Target Left");
		focusTargetLeft.transform.parent = this.transform;
		focusTargetLeft.transform.position = new Vector3 (3f, 0.8f, 15f);
		//verify is distance is valid
		distanceAway = Mathf.Clamp (distanceAway, distanceMin, distanceMax);

	}

	void Start () 
	{
		//create parent rig object and child camera to it plus the different camera positions

		//makes sure the camera has all the components it needs
		//look for player
		playerCharacter = GameObject.FindWithTag("Player").GetComponent<TP2_Controller>();
		firstPersonTarget = GameObject.Find ("First Person Target");
		thirdPersonTarget = GameObject.Find ("Third Person Target");
		//
		lookDirection = thirdPersonTarget.transform.forward;
		currentLookDirection = thirdPersonTarget.transform.forward;
		//for free case
		savedRigToGoal = RigToGoalDirection;
		//look for camera effect Depth Of Field
		//dof = GameObject.Find("Main Camera").GetComponent("DepthOfField"); 
		dof = GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>();
		if(dof == null)
		{
			Debug.LogWarning("Attach Depth of Field image effect to camera");
		}
		//set dof settings
		dof.focalTransform = thirdPersonTarget.transform;
		dof.focalLength = 4f;
		dof.focalSize = 0.7f;
		dof.aperture = 0.5f;
		dof.highResolution = true;
		dof.nearBlur = true;
		//look for camera effect Vignette And Chromatic Aberration
		//vaca = GameObject.Find("Main Camera").GetComponent("VignetteAndChromaticAberration");
		vaca = GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>();
		if(vaca == null)
		{
			Debug.LogWarning("Attach Vignette And Chromatic Aberration image effect to camera");
		}
		//set vaca settings
		vaca.intensity = 0.11f;
		vaca.blurSpread = 0.2f;
		vaca.blurDistance = 0.4f;
		// Intialize values to avoid having 0s
		savedRigToGoal = RigToGoalDirection;
	}
	void Update () 
	{
		//Debug.Log (rotateVector);
		//Debug.Log (mainCamState);
	}
	void FixedUpdate ()
	{

	}
	void LateUpdate () 
	{
		//input
		rotateVector = new Vector2 (Input.GetAxis ("RHorizontal"), Input.GetAxis ("RVertical"));
		if(rotateVector.magnitude > deadZone)
		{
			horizontal = rotateVector.x ;
			Debug.Log(horizontal);
			vertical = rotateVector.y ;
		}
		//camera position
		characterOffset = thirdPersonTarget.transform.position + new Vector3(0f, distanceUp, 0f);
		leftCharacterOffset = thirdPersonTarget.transform.position + new Vector3(-1.5f, distanceUp, 0f);
		rightCharacterOffset = thirdPersonTarget.transform.position + new Vector3(1.5f, distanceUp, 0f);
		Vector3 lookAt = characterOffset;
		//rest target position every frame
		Vector3 targetPosition = Vector3.zero;
		//testing states comment out the if conditions
		//mainCamState = mainCamStates.Behind;
		//mainCamState = mainCamStates.Focus;
		//mainCamState = mainCamStates.Free;
		//mainCamState = mainCamStates.FirstPerson;
		//set some bools for easy life
		if (Input.GetButtonDown("Y") )
		{
			firstPersonBool = !firstPersonBool;
		} else if( Input.GetButtonDown("B") && firstPersonBool)
		{
			firstPersonBool = false;
		}
		// Determine camera state
		// * focus *
		if (TP2_Controller.Instance.moveVector.magnitude > deadZone )
			//Input.GetAxis ("RXtrigger") >= deadZone)
		{
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, targetFOV, targetingSpeed * Time.deltaTime);
			mainCamState = mainCamStates.Focus;
		}
		else
		{			
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, behindFOV, targetingSpeed * Time.deltaTime);
			// * First Person *
			if (firstPersonBool && TP2_Controller.Instance.moveVector.magnitude < deadZone)
			{
				Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, targetFOV, targetingSpeed * Time.deltaTime);
				mainCamState = mainCamStates.FirstPerson;
			} 
			// * Free *
			if (rotateVector.magnitude > deadZone && TP2_Controller.Instance.moveVector.magnitude < deadZone && mainCamState != mainCamStates.FirstPerson )
			{
				mainCamState = mainCamStates.Free;
			}
			// * Target *
			if (Input.GetButtonDown ("RightPress"))
			{
				mainCamState = mainCamStates.Target;
			}
			// * Behind the back *
			if (mainCamState != mainCamStates.Free && firstPersonBool == false && 
			    TP2_Controller.Instance.moveVector.magnitude < deadZone &&
			    rotateVector.magnitude < deadZone)
			{
				mainCamState = mainCamStates.Behind;
			}
		}
		//execute camera state
		switch (mainCamState) 
		{
		case mainCamStates.Behind:
			ResetCamera ();
			// Only update camera look direction if moving
			if (playerCharacter.moveSpeed > TP2_Animator.Instance.locomotionThreshold && TP2_Animator.Instance.IsInLocomotion() && !TP2_Animator.Instance.IsInPivot())
			{
				lookDirection = Vector3.Lerp(thirdPersonTarget.transform.right * (rotateVector.x < 0 ? 1f : -1f), 
				                             thirdPersonTarget.transform.forward * (rotateVector.y < 0 ? -1f : 1f), 
				                             Mathf.Abs(Vector3.Dot(this.transform.forward, thirdPersonTarget.transform.forward)));
				// Calculate direction from camera to player, kill Y, and normalize to give a valid direction with unit magnitude
				currentLookDirection = Vector3.Normalize(characterOffset - this.transform.position);
				currentLookDirection.y = 0;
				Debug.DrawRay(this.transform.position, currentLookDirection, Color.blue);
				// Damping makes it so we don't update targetPosition while pivoting; camera shouldn't rotate around player
				currentLookDirection = Vector3.SmoothDamp(currentLookDirection, lookDirection, ref velocityCamSmooth, camSmoothDampTime);
			}		
			targetPosition = characterOffset + thirdPersonTarget.transform.up * distanceUp - Vector3.Normalize(currentLookDirection) * distanceAway;
			break;
		case mainCamStates.Focus:
			ResetCamera ();
			//set camera direction forward
			lookDirection = thirdPersonTarget.transform.forward ;
			currentLookDirection = thirdPersonTarget.transform.forward;
			// set target position of camera
			//targetPosition = characterOffset + thirdPersonTarget.transform.up * distanceUP - thirdPersonTarget.transform.forward * distanceAway;
			//if statements for targets
			//targetPosition = (characterOffset + thirdPersonTarget.transform.up * distanceUp - lookDirection * distanceAway) + new Vector3 (1f,0f,0f);
			//
			if(TP2_Controller.Instance.moveVector.x > deadZone)
			{
				currentLookDirection = focusTargetRight.transform.forward;
				targetPosition = (characterOffset + thirdPersonTarget.transform.up * distanceUp - lookDirection * distanceAway) + new Vector3 (1f,0f,0f);
			}
			else if (TP2_Controller.Instance.moveVector.x < -deadZone)
			{
				currentLookDirection = focusTargetLeft.transform.forward;
				targetPosition = (characterOffset + thirdPersonTarget.transform.up * distanceUp - lookDirection * distanceAway) + new Vector3 (-1f,0f,0f);
			}
			else
			{
				currentLookDirection = thirdPersonTarget.transform.forward;
				targetPosition = characterOffset + thirdPersonTarget.transform.up * distanceUp - Vector3.Normalize(currentLookDirection) * distanceAway;
			}
			//rotate camera
			this.transform.RotateAround(characterOffset, thirdPersonTarget.transform.right, freeRotation * rotateVector.y);
			this.transform.RotateAround(characterOffset, thirdPersonTarget.transform.up, freeRotation * rotateVector.x );
			break;
		case mainCamStates.FirstPerson:
			//looking up and down
			//calculate the amount of rotation and apply to the first person target
			xAxisRot += (rotateVector.y * cameraSensitivity);
			xAxisRot = Mathf.Clamp(xAxisRot, firstPersonClamp.x, firstPersonClamp.y);
			firstPersonTarget.transform.Rotate(Vector3.left, rotateVector.y * cameraSensitivity);
			//localRotation = Quaternion.Euler(xAxisRot, 0, 0);
			//superimpose fps target object rotation on camera
			Quaternion rotationShift = Quaternion.FromToRotation(this.transform.forward, firstPersonTarget.transform.forward);
			this.transform.rotation = rotationShift * this.transform.rotation;
			//move left right in the other script
			playerCharacter.transform.Rotate(Vector3.up * rotateVector.x * cameraSensitivity);
			//move camera to fps postition
			targetPosition = firstPersonTarget.transform.position;
			//smoothly transition look direction towards fps target when entering mode
			lookAt = Vector3.Slerp(targetPosition + thirdPersonTarget.transform.forward, this.transform.position + this.transform.forward, camSmoothDampTime * Time.deltaTime);
			//choose look at target based on distance
			lookAt = Vector3.Lerp (this.transform.position + this.transform.forward, lookAt, Vector3.Distance(this.transform.position, firstPersonTarget.transform.position));

			this.transform.RotateAround(characterOffset, thirdPersonTarget.transform.right, -1f * rotateVector.y);
			this.transform.RotateAround(characterOffset, thirdPersonTarget.transform.up, -1f * rotateVector.x );
			break;
		case mainCamStates.Target:
			ResetCamera ();
			lookDirection = thirdPersonTarget.transform.forward;
			targetPosition = characterOffset + thirdPersonTarget.transform.up * distanceUp - lookDirection * distanceAway;
			break;
		case mainCamStates.Free:
			//direction between character and camera
			Vector3 rigToGoal = Vector3.Normalize (characterOffset - this.transform.position);
			rigToGoal.y = 0f;
			float velDistance = 0f;
			Vector3 currentOffset = transform.position;
			float freeDistance = Vector3.Distance(transform.position, thirdPersonTarget.transform.position);
			targetPosition = transform.position;
			//targetPosition = characterOffset + thirdPersonTarget.transform.up * distanceUp - Vector3.Normalize(currentLookDirection) * distanceAway;
			
			float horizontal = rotateVector.x * cameraSensitivity;
			if(Mathf.Abs( rotateVector.x) > deadZone) {
				transform.RotateAround (thirdPersonTarget.transform.position, Vector3.up, -horizontal);
				targetPosition = transform.position ;
			}
			float vertical = rotateVector.y * cameraSensitivity;
			if(Mathf.Abs( rotateVector.y) > deadZone) {
				transform.RotateAround (thirdPersonTarget.transform.position, Vector3.right, vertical);
				targetPosition = transform.position ;
			}
			if(freeDistance != distanceAway ) 
			{
				freeDistance = Mathf.SmoothDamp (freeDistance, distanceAway, ref velDistance, cameraSensitivity);
			}
			break;
		}
		//collision
		CompensateForWalls (characterOffset, ref targetPosition);
		//smoothing camera movement
		smoothPosition(this.transform.position, targetPosition);
		//make sure camera is looking correctly
		transform.LookAt (lookAt);
		//save previous frame
		rightStickSave = new Vector2 (rotateVector.x, rotateVector.y);
	}
	private void smoothPosition(Vector3 fromPos, Vector3 toPos)
	{
		this.transform.position = Vector3.SmoothDamp (fromPos, toPos, ref velocityCamSmooth, camSmoothDampTime);
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
}
