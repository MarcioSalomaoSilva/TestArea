using UnityEngine;
using System.Collections;

public class TP_Camera : MonoBehaviour {

	public static TP_Camera Instance;

	public GameObject targetLookAt;
	public GameObject targetModel;

	public Transform TargetModel;
	public Transform TargetLookAt;
	public Transform TargetLookAtL;
	public Transform TargetLookAtR;
	
	public float Distance;
	public float DistanceMin = 2f;
	public float DistanceMax = 5f;
	public float DistanceResumesmooth = 1f;
	private float distanceSmooth = 0f;
	private float preOccludedDistance = 0f;

	private float X_MouseSensitivity = 1f;
	private float Y_MouseSensitivity = 1f;
	public float Pad_MouseSensitivity = 5f;
	private float MouseWheelSensitivity = 5f;
	private float X_Smooth= 0.05f;
	private float Y_Smooth= 0.1f;
	public float X_MinLimit = -20f;
	public float X_MaxLimit = 40f;
	public float Y_MinLimit = -20f;
	public float Y_MaxLimit = 20f;
	public float DistanceSmooth = 0.1f;
	public float RotateSmooth = 4f; 
	public float Smooth= 0.1f;

	public float OcclusionDistanceStep = 2f;
	public float MaxOcclusionCheck = 2f;

	private float padXY = 5f;
	private float mouseX = 0f;
	private float mouseY = 10f;
	private float velX = 0f;
	private float velY = 0f;
	private float velZ = 0f;
	private float velDistance = 0f;
	//private float startDistance = 0f;
	private float desiredDistance = 0f;

	private Vector3 position = Vector3.zero;
	private Vector3 desiredPosition = Vector3.zero;
	public Vector3 viewPos;

	private bool mouseSmooth = false;
	private bool zoom = false;
	private bool left = false;
	//private bool LeftTrigger=false;
	//private bool rightTrigger=false;

	//accessed by the target script
	

	void Awake() 
	{
		Instance = this;
		Distance = DistanceMax;
		targetLookAt = GameObject.Find ("targetLookAt") as GameObject;
		targetModel = GameObject.Find ("targetModel") as GameObject;
	}
	void Start()
	{
		//verify that distance is valid and return distacne, take start distance and assign a value to it
		Distance = Mathf.Clamp (Distance, DistanceMin, DistanceMax);
		//startDistance = Distance;
		//call reset method
		Reset ();
	}
	void LateUpdate() 
	{
		//check target and if its equal to null the return
		if (TargetLookAt == null) 
		{
			return;
		}
		if (TargetModel == null) 
		{
			return;
		}
		//run
		HandlePlayerinput ();

		var count = 0f;
		do {
			//run
			CalculateDesiredPosition ();
			count++;

		} while (CheckIfOccluded(count));
//		//run
//		CheckCameraPoints (TargetLookAt.position, desiredPosition);
		//run
		UpdatePosition ();
		//creates vectot viewPos and sets it to targets position reltive to the screen
		viewPos = Camera.main.WorldToViewportPoint(TargetModel.position);
	}
	public void HandlePlayerinput(){

		//deadzone variable for scrollwheel and analog stick
		var deadZoneMouse = 0.01f;
		var deadZonePad = 0.9f;
		//deadzone and gamepad input (inputs gamepad: "RHorizontal" "RVertical")
		//new 2d vector that corresponds to axial input to make things simpler (these inputs give two points for a vector)
		Vector2 stickInput = new Vector2 (Input.GetAxis("RHorizontal"), Input.GetAxis("RVertical"));
		//if the magnitude(length) of a vector is below the deadzone stick input equals nothing 
		if(stickInput.magnitude > deadZonePad)
		{
			mouseX += Input.GetAxis ("RHorizontal") * X_MouseSensitivity;
			mouseY -= Input.GetAxis ("RVertical") * Y_MouseSensitivity;
		} 
		mouseY = Helper.ClampAngle (mouseY, Y_MinLimit, Y_MaxLimit);
		if (Input.GetButtonDown ("Y")) 
		{
			zoom = true;
			desiredDistance = Mathf.Clamp (DistanceMin, DistanceMin, DistanceMax);
			padXY = Helper.ClampAngle (padXY, Y_MinLimit, Y_MaxLimit);
			padXY = Helper.ClampAngle (padXY, X_MinLimit, X_MaxLimit);
			//keep the main character on screen while zoomed (axis, and two varibles 0 to 1)
			viewPos = Camera.main.WorldToViewportPoint(TargetModel.position);
			viewPos.x = Mathf.Clamp(viewPos.x, 0.2f, 0.8f);
			viewPos.y = Mathf.Clamp(viewPos.y, 0.3f, 0.7f);
			transform.position = Camera.main.ViewportToWorldPoint(viewPos);
			distanceSmooth = DistanceSmooth;
		} else if (Input.GetButtonUp ("Y")) {
			zoom = false;
			desiredDistance = DistanceMax;
		} 
		//deadzone and mouse input (nputs mouse:"Mouse X" "Mouse Y" )
		//if mouse button is down check the mouse axis input 
		if (Input.GetMouseButton (1)) 
		{
			mouseSmooth = true;
			//check pad deadzone  
			if (Input.GetAxis ("Mouse X") < -deadZoneMouse || Input.GetAxis ("Mouse X") > deadZoneMouse && 
			    Input.GetAxis ("Mouse Y") < -deadZoneMouse || Input.GetAxis ("Mouse Y") > deadZoneMouse) 
			{
				mouseX += Input.GetAxis ("Mouse X") * X_MouseSensitivity;
				mouseY -= Input.GetAxis ("Mouse Y") * Y_MouseSensitivity;
			}
			//this is where were going to clamp or limit mouseY or pad rotation
			mouseY = Helper.ClampAngle (mouseY, Y_MinLimit, Y_MaxLimit);
			//check scrollwheel deadzone
			if (Input.GetAxis ("Mouse ScrollWheel") < -deadZoneMouse || Input.GetAxis ("Mouse ScrollWheel") > deadZoneMouse) 
			{
				//this is where the multiply mousewheel and subtract it from distance
				desiredDistance = Mathf.Clamp (Distance - Input.GetAxis ("Mouse ScrollWheel") * 
											   MouseWheelSensitivity, DistanceMin, DistanceMax);
				preOccludedDistance = desiredDistance;
				distanceSmooth = DistanceSmooth;
			}
		}else{
			mouseSmooth = false;
		}
	}
	void CalculateDesiredPosition()
	{
		ResetDesiredDistance ();
		//evaluate distance. start distance, end distance, current curve and velocity. Distance is a curve that accelarates and decelarates
		Distance = Mathf.SmoothDamp (Distance, desiredDistance, ref velDistance, distanceSmooth);
		//calculate desired position, calculate where the player has moved the camera too
		desiredPosition = CalculatePosition (mouseY, mouseX, Distance);
	}
	Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		//create vector
		Vector3 direction = new Vector3 (0, 0, -distance);
		//create rotation
		Quaternion rotation = Quaternion.Euler (rotationX, rotationY, 0);
		return TargetLookAt.position + rotation * direction;
	}
	bool CheckIfOccluded(float count)
	{
		var isOccluded = false;
		var nearestDistance = CheckCameraPoints(targetLookAt.transform.position, desiredPosition);
		if(nearestDistance != -1)
		{
			if(count < MaxOcclusionCheck)
			{
				isOccluded = true;
				Distance -= OcclusionDistanceStep;
				if(Distance < DistanceMin)
				{
					Distance = DistanceMin;
				}
			}else{
				Distance = nearestDistance - Camera.main.nearClipPlane;
			}
			//Distance = nearestDistance - Camera.main.nearClipPlane;
			desiredDistance = Distance;
			distanceSmooth = DistanceResumesmooth;
		}
		return isOccluded;
	}
	float CheckCameraPoints(Vector3 from, Vector3 to){

		var nearestDistance = -1f;
		RaycastHit hitInfo;
		Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);
//		//Draw lines to tagrets
//		Debug.DrawLine (TargetLookAtL.position, to + (transform.forward * -GetComponent<Camera> ().nearClipPlane), Color.red);
//		Debug.DrawLine (TargetLookAtR.position, to + (transform.forward * -GetComponent<Camera> ().nearClipPlane), Color.red);
		//Draw lines inthe editor to make it easier to visualize
		Debug.DrawLine(from, to + (transform.forward * -GetComponent<Camera>().nearClipPlane), Color.red);
//		Debug.DrawLine(from, clipPlanePoints.UpperLeft, Color.gray);
//		Debug.DrawLine(from, clipPlanePoints.LowerLeft, Color.gray);
//		Debug.DrawLine(from, clipPlanePoints.UpperRight, Color.gray);
//		Debug.DrawLine(from, clipPlanePoints.LowerLeft, Color.gray);
		//box
		Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight, Color.blue);
		Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight, Color.blue);
		Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft, Color.blue);
		Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft, Color.blue);
		//lines for current target look at
		Debug.DrawLine (targetLookAt.transform.position, to + (transform.forward * -GetComponent<Camera> ().nearClipPlane), Color.red);
		Debug.DrawLine(targetLookAt.transform.position, clipPlanePoints.UpperLeft, Color.black);
		Debug.DrawLine(targetLookAt.transform.position, clipPlanePoints.LowerRight, Color.black);
		Debug.DrawLine(targetLookAt.transform.position, clipPlanePoints.UpperRight, Color.black);
		Debug.DrawLine(targetLookAt.transform.position, clipPlanePoints.LowerLeft, Color.black);
		//to mid box
		Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.MidUpperLeft, Color.black);
		Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.MidLowerLeft, Color.black);
		Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.MidUpperRight, Color.black);
		Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.MidLowerRight, Color.black);
		//mid box
		Debug.DrawLine(clipPlanePoints.MidUpperLeft, clipPlanePoints.MidUpperRight, Color.blue);
		Debug.DrawLine(clipPlanePoints.MidUpperRight, clipPlanePoints.MidLowerRight, Color.blue);
		Debug.DrawLine(clipPlanePoints.MidLowerRight, clipPlanePoints.MidLowerLeft, Color.blue);
		Debug.DrawLine(clipPlanePoints.MidLowerLeft, clipPlanePoints.MidUpperLeft, Color.blue);
		//to far box
		Debug.DrawLine(clipPlanePoints.MidUpperLeft, clipPlanePoints.FarUpperLeft, Color.black);
		Debug.DrawLine(clipPlanePoints.MidLowerRight, clipPlanePoints.FarLowerRight, Color.black);
		Debug.DrawLine(clipPlanePoints.MidUpperRight, clipPlanePoints.FarUpperRight, Color.black);
		Debug.DrawLine(clipPlanePoints.MidLowerLeft, clipPlanePoints.FarLowerLeft, Color.black);
		//far box
		Debug.DrawLine(clipPlanePoints.FarUpperLeft, clipPlanePoints.FarUpperRight, Color.blue);
		Debug.DrawLine(clipPlanePoints.FarUpperRight, clipPlanePoints.FarLowerRight, Color.blue);
		Debug.DrawLine(clipPlanePoints.FarLowerRight, clipPlanePoints.FarLowerLeft, Color.blue);
		Debug.DrawLine(clipPlanePoints.FarLowerLeft, clipPlanePoints.FarUpperLeft, Color.blue);
		if (left) 
		{
			//lines to target
			Debug.DrawLine (TargetLookAtL.position, to + (transform.forward * -GetComponent<Camera> ().nearClipPlane), Color.red);
			Debug.DrawLine(TargetLookAtL.position, clipPlanePoints.UpperLeft, Color.gray);
			Debug.DrawLine(TargetLookAtL.position, clipPlanePoints.LowerRight, Color.gray);
			Debug.DrawLine(TargetLookAtL.position, clipPlanePoints.UpperRight, Color.gray);
			Debug.DrawLine(TargetLookAtL.position, clipPlanePoints.LowerLeft, Color.gray);
		} else {
			//lines to target
			Debug.DrawLine (TargetLookAtR.position, to + (transform.forward * -GetComponent<Camera> ().nearClipPlane), Color.red);
			Debug.DrawLine(TargetLookAtR.position, clipPlanePoints.UpperLeft, Color.gray);
			Debug.DrawLine(TargetLookAtR.position, clipPlanePoints.LowerRight, Color.gray);
			Debug.DrawLine(TargetLookAtR.position, clipPlanePoints.UpperRight, Color.gray);
			Debug.DrawLine(TargetLookAtR.position, clipPlanePoints.LowerLeft, Color.gray);
		} 
		//if this hits anything it more important that the above var setting, as soon as detected this is more important
		if (Physics.Linecast (from, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player") 
		{
			nearestDistance = hitInfo.distance;
		}
		if (Physics.Linecast (from, clipPlanePoints.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player") 
		{
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			{
				nearestDistance = hitInfo.distance;
			}
		}
		if (Physics.Linecast (from, clipPlanePoints.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player") 
		{
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			{
				nearestDistance = hitInfo.distance;
			}
		}
		if (Physics.Linecast (from, clipPlanePoints.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player") 
		{
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			{
				nearestDistance = hitInfo.distance;
			}
		}
		if (Physics.Linecast (from, to + (transform.forward * -GetComponent<Camera>().nearClipPlane), out hitInfo) && hitInfo.collider.tag != "Player") 
		{
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
			{
				nearestDistance = hitInfo.distance;
			}
		}
		return nearestDistance;
	}
	void ResetDesiredDistance ()
	{
		if(zoom)
		{
			desiredDistance = DistanceMin;
		}
		else if(desiredDistance < preOccludedDistance)
		{
			var pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);
			var nearestDistance = CheckCameraPoints (TargetLookAt.position, pos);
			if(nearestDistance == -1f || nearestDistance > preOccludedDistance)
			{
				desiredDistance = preOccludedDistance;
			}
		}
	}
	void UpdatePosition()
	{
		//calculate a position with the smoothing, positionX, positionY, positionZ.
		if (mouseSmooth) 
		{
			var posX = Mathf.SmoothDamp (position.x, desiredPosition.x, ref velX, X_Smooth);
			var posY = Mathf.SmoothDamp (position.y, desiredPosition.y, ref velY, Y_Smooth);
			var posZ = Mathf.SmoothDamp (position.z, desiredPosition.z, ref velZ, X_Smooth);
			position = new Vector3 (posX, posY, posZ);
		}else {
			var posX = Mathf.SmoothDamp (position.x, desiredPosition.x, ref velX, Smooth);
			var posY = Mathf.SmoothDamp (position.y, desiredPosition.y, ref velY, Smooth);
			var posZ = Mathf.SmoothDamp (position.z, desiredPosition.z, ref velZ, Smooth);
			position = new Vector3 (posX, posY, posZ);
		}
		//take the cameras current position and set it to the previously calculated position, assign that position(move the camera)
		transform.position = position;
		//look at targetLookAt
		//transform.LookAt (TargetLookAt);
		//run
		UpdateTargetPosition ();
	}
	Vector3 UpdateTargetPosition ()
	{	
//		Debug.DrawLine (transform.position, TargetLookAtR.position, Color.red);
		//var targetLookAtPos = targetLookAt.transform.position;
		var LeftLine = false;
		var RightLine = false;
		//linecast to left target position
		if (Physics.Linecast (transform.position, TargetLookAtL.position))
		{
			LeftLine = true;
		} else {
			LeftLine = false;
		}
		//linecast to right target position
		if (Physics.Linecast (transform.position, TargetLookAtR.position))
		{
			RightLine = true;
		} else {
			RightLine = false;
		}

		if (!zoom) 
		{
			transform.LookAt (targetLookAt.transform.position);
//			if (viewPos.x > 0.5F && LeftLine == false) 
//			{
//					var targetRotation = Quaternion.LookRotation (TargetLookAt.position - transform.position);
//					transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, 5f * Time.deltaTime);
//					targetLookAt.transform.position = Vector3.Slerp (targetLookAt.transform.position, TargetLookAtL.position, 1f * Time.deltaTime);
//					left = false;
//			} else if (viewPos.x > 0.5F && LeftLine == true) {
//					var targetRotation = Quaternion.LookRotation (TargetLookAt.position - transform.position);
//					transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, 5f * Time.deltaTime);
//					targetLookAt.transform.position = Vector3.Slerp (targetLookAt.transform.position, TargetLookAtL.position, 5f * Time.deltaTime);
//					left = true;
//			}
//			if (viewPos.x < 0.5F && RightLine == false)
//			{	
//				var targetRotation = Quaternion.LookRotation (TargetLookAt.position - transform.position);
//				transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, 5f * Time.deltaTime);
//				targetLookAt.transform.position = Vector3.Slerp (targetLookAt.transform.position, TargetLookAtR.position, 1f * Time.deltaTime);
//				left = true;
//			} else if (viewPos.x < 0.5F && RightLine == true) {
//				var targetRotation = Quaternion.LookRotation (TargetLookAt.position - transform.position);
//				transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, 5f * Time.deltaTime);
//				targetLookAt.transform.position = Vector3.Slerp (targetLookAt.transform.position, TargetLookAtR.position, 5f * Time.deltaTime);
//				left = false;
//			}
		}else if (zoom) {
			transform.LookAt (targetLookAt.transform.position);
		}
		return targetLookAt.transform.position;
	} 
	public void Reset()
	{
		mouseX = 0f;
		mouseY = 10f;
		//Distance = startDistance;
		desiredDistance = Distance;
		preOccludedDistance = Distance;
	}
	public static void CreateOrUseCamera()
	{
		GameObject tempCamera;
		GameObject targetLookAt;
		GameObject targetLookAtL;
		GameObject targetLookAtR;
		GameObject targetModel;
		TP_Camera myCamera;
		//if camera exists tempcamera equals the main camera
		if (Camera.main != null) 
		{
			tempCamera = Camera.main.gameObject;
		}
		// if camera doesn't exist create a new camera object and add camera component along with tag
		else {
			tempCamera = new GameObject ("Main Camera");
			tempCamera.AddComponent <Camera>();
			tempCamera.tag = "MainCamera";
		}
		//attach this script to the new camera
		tempCamera.AddComponent <TP_Camera>();
		//find that component and get a reference to it
		myCamera = tempCamera.GetComponent ("TP_Camera") as TP_Camera;
		//find target lookat and assign it to whatver gameobject.find
		targetLookAt = GameObject.Find ("targetLookAt") as GameObject;
		targetLookAtL = GameObject.Find ("targetLookAtL") as GameObject;
		targetLookAtR = GameObject.Find ("targetLookAtR") as GameObject;
		targetModel = GameObject.Find ("targetModel") as GameObject;
		//check to see if the target is found if null nothing was found and it needs to be created and positioned at world origin
		if (targetLookAt == null) 
		{
			targetLookAt = new GameObject ("targetLookAt");
			targetLookAt.transform.position = Vector3.zero;
			Debug.LogWarning("Create game object targetLookAt and its children counterparts then attach to player");
		}
		if (targetModel == null) 
		{
			targetModel = new GameObject ("targetModel");
			targetModel.transform.position = Vector3.zero;
			Debug.LogWarning("Create game object targetModel and its children (L and R) then attach to player");
		}
		//go back to mycamera and assign its target transorm field to be this target
		myCamera.TargetLookAt = targetLookAt.transform;
		myCamera.TargetLookAtR = targetLookAtR.transform;
		myCamera.TargetLookAtL = targetLookAtL.transform;
		myCamera.TargetModel = targetModel.transform;
	}
}
