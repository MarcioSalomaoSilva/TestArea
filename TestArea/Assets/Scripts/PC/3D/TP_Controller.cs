using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class TP_Controller : MonoBehaviour {

	public static CharacterController characterController { get; private set;}
	//Refrence to current instance of itself
	public static TP_Controller Instance;

	//bools that correspond to zooming and being above the walzone
	public bool running=false;
	public bool zoomed=false;

	//Variable that sets deadzone of controller joystck
	public float deadZone = 0.1f;
	
	private Vector3 lastDirection;
			
	void Awake ()
	{
		//allows for tp_controller.something, was put above because too lazy
		Instance = this;
		TP_Camera.CreateOrUseCamera ();
	}
	void Start()
	{
		//assign refrences, cast as a character controller because getcomponent returns object. 
		//was inserted below because of null
		characterController = GetComponent ("CharacterController") as CharacterController;
	}

	void Update () 
	{
		//if there is no camera in the scene don't do anything(return)
		if (Camera.main == null){
			return;
		}
		//run the method below
		GetLocomotionInput ();
		HandleActionInput ();
		//run the methods in the other tp motor class
		TP_Motor.Instance.UpdateMotor ();
	}
	void GetLocomotionInput()
	{
		//save y component of move vector into vertical velocity and feed it into move vectors y
		TP_Motor.Instance.VerticalVelocity = TP_Motor.Instance.MoveVector.y;
		//stops motion from becoming additive recalculates every frame
		TP_Motor.Instance.MoveVector = Vector3.zero;
		TP_Motor.Instance.MoveVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		if (characterController.isGrounded == true) 
		{
			if (TP_Motor.Instance.MoveVector.magnitude > deadZone) 
			{
				TP_Motor.Instance.MoveVector -= new Vector3 (0, 0, Input.GetAxis ("Vertical"));
				TP_Motor.Instance.MoveVector += new Vector3 (Input.GetAxis ("Horizontal"), 0, 0);
				running =true;
			}else{
				running = false;
			}
			lastDirection = TP_Motor.Instance.MoveVector;
		}else{
			TP_Motor.Instance.MoveVector = lastDirection;
		}
		TP_Animator.Instance.DetermineCurrentMoveDirection ();
	}
	void HandleActionInput()
	{
		if (Input.GetButtonDown ("Y")) 
		{
			zoomed = !zoomed;
		} else if(Input.GetButtonUp("Y"))
		{
			zoomed = !zoomed;
		}
		if(Input.GetButtonDown("A"))
		{
			Jump ();
		}
	}
	void Jump()
	{
		TP_Motor.Instance.Jump ();
	}
}