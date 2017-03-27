using UnityEngine;
using System.Collections;

public class TEST_Force : MonoBehaviour {

	public static CharacterController characterController { get; private set;}

	public static TEST_Force Instance;

	public float VerticalVelocity { get; set; }
	public float moveSpeed = 10f;
	public float JumpSpeed = 6f;
	public float Gravity = 21f;
	public float TerminalVelocity = 20f;

	public GameObject target;

	public Vector3 GravityVector { get; set; }
	public Vector3 MoveVector { get; set; }

	//private Vector3 yLookAt;


	void Awake () 
	{
		Instance = this;
	}

	void Start() 
	{
		characterController = GetComponent ("CharacterController") as CharacterController;
	}

	void Update() 
	{
		GetPlayerInput ();

		//transform.LookAt(target.transform);
		//transform.position = new Vector3 (0, TEST_Gravity.Instance.directionOfGravity.y, 0);
		transform.LookAt( target.transform );
		//transform.LookAt(new Vector3(target.transform.position.x , transform.position.y , target.transform.position.z));
		transform.Rotate( 90, 0, 0 );

		GravityVector = TEST_Gravity.Instance.directionOfGravity;
		MoveVector = new Vector3 (MoveVector.x, VerticalVelocity, MoveVector.z);
		//ApplyGravity (); 

		//characterController.Move (GravityVector * Time.deltaTime);
		characterController.Move (GravityVector * Time.deltaTime);
		
		Debug.Log (characterController.isGrounded);

	}

	void GetPlayerInput(){

		var deadZone = 0.1f;
		
		VerticalVelocity = MoveVector.z;
		
		MoveVector = Vector3.zero;
		
		MoveVector = new Vector3 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0);;
		
		//if (characterController.isGrounded == true) {
			
			if (MoveVector.magnitude > deadZone) {
				
				MoveVector += new Vector3 (0, 0, Input.GetAxis ("Vertical"));
				MoveVector += new Vector3 (Input.GetAxis ("Horizontal"), 0, 0);
				
				
				//running = true;
				
			}
		//}
	}

	void ApplyGravity(){
		
		if (MoveVector.y > -TerminalVelocity) {
			
			MoveVector = new Vector3(MoveVector.x,MoveVector.y -Gravity*Time.deltaTime,MoveVector.z);
			
		}
		
		if (TP_Controller.characterController.isGrounded && MoveVector.y < -1f) {
			
			MoveVector = new Vector3 (MoveVector.x, - 1f, MoveVector.z);
			
		}
		
	}
}