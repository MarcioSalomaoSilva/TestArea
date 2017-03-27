using UnityEngine;
using System.Collections;

public class TP_Motor : MonoBehaviour {

	//Refrence to current instance of itself
	public static TP_Motor Instance;

	public float BackwardSpeed = 2f;
	public float StrafingSpeed = 5f;
	public float WalkSpeed = 7f;
	public float RunSpeed = 7f;
	public float SlideSpeed = 10f;
	public float MaxSpeed = 20f;
	public float Acceleration=1f;
	public float Decceleration = 15f;
	public float Gravity = 21f;
	public float TerminalVelocity = 20f;
	public float JumpSpeed = 6f;
	public float SlideThreshold = 0.6f;
	public float MaxControllableSlideMagnitude = 0.4f;

	private Vector3 slideDirection;
	public float VerticalVelocity { get; set; }
	public Vector3 MoveVector { get; set; }


	void Awake () 
	{
		Instance = this;
	}
	void Start()
	{

	}
	void Update()
	{
		Debug.Log (MoveVector.magnitude);
	}
	public void UpdateMotor () 
	{
		//when update motor is called it calls these methods.
		SnapAlignCharacterWithCamera ();
		ProcessMotion ();
	}
	void ProcessMotion()
	{
		//Transform moveVector into world space relative to our characters rotation
		MoveVector = transform.TransformDirection (MoveVector);
		//normalize our move vector if magnitude is greater > 1 (restrict it to 1 so that the character doesn't move faster diagonally)
		if (MoveVector.magnitude > 1) 
		{
			MoveVector = Vector3.Normalize (MoveVector);
		}
		//apply sliding if applicable
		ApplySlide ();
		//Multiply MoveVector by Speed, add acceleration
		MoveVector *= MoveSpeed (); 
		//Reapply Vertical Velocity movevector.y
		MoveVector = new Vector3 (MoveVector.x, VerticalVelocity, MoveVector.z);
		//apply vertical Velocity
		ApplyGravity ();
		//Move the Character in world space. Multiply MoveVector by DeltaTime so that changes to value per second instead of value per frame
		TP_Controller.characterController.Move (MoveVector*Time.deltaTime);
	}
	void ApplyGravity()
	{
		if (MoveVector.y > -TerminalVelocity) 
		{
			MoveVector = new Vector3 (MoveVector.x, MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);
		}
		if (GetComponent< CharacterController>().isGrounded  && MoveVector.y < -1f) 
		{
			MoveVector = new Vector3 (MoveVector.x, - 1f, MoveVector.z);
		}
	}

	void ApplySlide(){

		if (!TP_Controller.characterController.isGrounded) {
			return;
		}
		slideDirection = Vector3.zero;
		RaycastHit hitInfo;
		//if we hit something return it
		if (Physics.Raycast (transform.position + Vector3.up, Vector3.down, out hitInfo)) 
		{
			if (hitInfo.normal.y < SlideThreshold) 
			{
				slideDirection = new Vector3 (hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z);
			}
			if (slideDirection.magnitude < MaxControllableSlideMagnitude) 
			{
				MoveVector += slideDirection;
			} else {
				MoveVector = slideDirection;
			}
		}
	}
	public void Jump()
	{
		if (TP_Controller.characterController.isGrounded) 
		{
			VerticalVelocity = JumpSpeed;
		}
	}
	void SnapAlignCharacterWithCamera()
	{
		//if move vector.x or .y don't equal zero
		if (MoveVector.x != 0 || MoveVector.y != 0) 
		{
			//take x and z from caracters controller(whatever they happens to be right now) and take y rotation of camera to change character direction.
			//set characters orientation to that of the cameras
			transform.rotation = Quaternion.Euler(transform.eulerAngles.x, 
			                                      Camera.main.transform.eulerAngles.y,
			                                      transform.eulerAngles.z);
		}
	}
	float MoveSpeed()
	{
		//--
		var moveSpeed =  0f;
		//--
		if (TP_Controller.Instance.running) 
		{
			RunSpeed += Acceleration * Time.deltaTime;
			if (RunSpeed > MaxSpeed) 
			{
				RunSpeed = MaxSpeed;
			}
		}  else {
			RunSpeed = WalkSpeed;
		}
		
		if (TP_Controller.characterController.isGrounded == false) 
		{
			RunSpeed -= Decceleration * Time.deltaTime;
			if (RunSpeed < WalkSpeed) 
			{
				RunSpeed = WalkSpeed;
			}
		}
		if (TP_Controller.Instance.zoomed) 
		{	
			RunSpeed=WalkSpeed;
		}
		//--
		switch (TP_Animator.Instance.MoveDirection) 
		{
		case TP_Animator.Direction.Stationary:
				moveSpeed = 0f;
			break;
		case TP_Animator.Direction.Forward:
				moveSpeed = WalkSpeed;
			break;
		case TP_Animator.Direction.Backward:
				moveSpeed = BackwardSpeed;
			break;
		case TP_Animator.Direction.Left:
				moveSpeed = StrafingSpeed;
			break;
		case TP_Animator.Direction.Right:
				moveSpeed = StrafingSpeed;
			break;
		case TP_Animator.Direction.LeftForward:
				moveSpeed = WalkSpeed;
			break;
		case TP_Animator.Direction.RightForward:
				moveSpeed = WalkSpeed;
			break;
		case TP_Animator.Direction.LeftBackward:
				moveSpeed = BackwardSpeed;
			break;
		case TP_Animator.Direction.RightBackward:
				moveSpeed = BackwardSpeed;
			break;
		//running
		case TP_Animator.Direction.Run:
			moveSpeed = RunSpeed;
			break;
		}
		if(slideDirection.magnitude >0f)
		{
			moveSpeed = SlideSpeed;
		}
		return moveSpeed;
	}
}