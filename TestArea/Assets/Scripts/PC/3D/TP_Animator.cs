using UnityEngine;
using System.Collections;

public class TP_Animator : MonoBehaviour {

	public enum Direction{
		Stationary, Forward, Backward, Left, Right, Run,
		LeftForward, RightForward, LeftBackward, RightBackward, Jump, RunJump
	}

	public static TP_Animator Instance;

	public Direction MoveDirection {get; set;}

	void Awake () 
	{
		Instance = this;
	}
	void Update () 
	{
	
	}
	public void DetermineCurrentMoveDirection()
	{
		var forward = false;
		var backward = false;
		var left = false;
		var right = false;
		var jump = false;
		var run = false;
		//var runjump = false;
		//var jumpDirection = TP_Motor.Instance.MoveVector;
		if (TP_Controller.Instance.zoomed ) 
		{
			if (TP_Motor.Instance.MoveVector.z > 0f) 
			{
				forward = true;
			}
			if (TP_Motor.Instance.MoveVector.z < 0f) 
			{
				backward = true;
			}
			if (TP_Motor.Instance.MoveVector.x < 0f) 
			{
				left = true;
			}
			if (TP_Motor.Instance.MoveVector.x > 0f) 
			{
				right = true;
			}
			if (TP_Controller.characterController.isGrounded == false) 
			{
				jump = true;
			}
		}
		if (TP_Controller.Instance.zoomed==false) 
		{
				run = true;
		}
		//conditions for not running
		if(jump)
		{
			MoveDirection = Direction.Jump;
		}
		if (forward) 
		{
			if (left) 
			{
			MoveDirection = Direction.LeftForward;
			} else if (right) {
			MoveDirection = Direction.RightForward;
			} else {
			MoveDirection = Direction.Forward;
			}
		} else if (backward) {
			if (left) 
			{
				MoveDirection = Direction.LeftBackward;
			} else if (right) {
				MoveDirection = Direction.RightBackward;
			} else {
				MoveDirection = Direction.Backward;
			}
		} else if (left) {
			MoveDirection = Direction.Left;
		} else if (right) {
			MoveDirection = Direction.Right;
		} else {
			MoveDirection = Direction.Stationary;
		}
		//conditions for running
		if (run) 
		{
			MoveDirection = Direction.Run;
		}
	}
}
