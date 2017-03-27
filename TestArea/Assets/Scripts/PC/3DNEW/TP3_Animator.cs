using UnityEngine;
using System.Collections;

//[RequireComponent (typeof(Animator))]
//[RequireComponent (typeof(TP3_Controller))]
public class TP3_Animator : MonoBehaviour {

	public static TP3_Animator Instance;

	public Animator animator;
	public bool ikActive = false;
	//ik targets
	GameObject ikHeadTarget;
	GameObject thirdPersonTarget;
	GameObject thirdPersonLeft;
	GameObject thirdPersonRight;
	//
	private AnimatorStateInfo stateInfo;
	private AnimatorTransitionInfo transInfo;
	// Hashes
	private int m_LocomotionId = 0;
	private int m_IdleId = 0;
	private int m_RunningId = 0;
	private int m_LocomotionPivotLId = 0;
	private int m_LocomotionPivotRId = 0;	
	private int m_LocomotionPivotLTransId = 0;	
	private int m_LocomotionPivotRTransId = 0;
	//
	public float locomotionThreshold { get { return 0.2f; } }
	private float deadZone = 0.1f;
	//initialize
	void Awake ()
	{
		Instance = this;
		//
		animator = transform.Find ("Y_Bot").GetComponent<Animator> ();
		//
		if (animator.layerCount >= 2) 
		{
			animator.SetLayerWeight(1,1);
		}
		//hash all animation names for performance
		m_LocomotionId = Animator.StringToHash("Base Layer.Locomotion");
		m_IdleId = Animator.StringToHash("Base Layer.Idle");
		m_RunningId = Animator.StringToHash("Base Layer.Running");
		m_LocomotionPivotLId = Animator.StringToHash("Base Layer.Pivot Left");
		m_LocomotionPivotRId = Animator.StringToHash("Base Layer.Pivot Right");
		m_LocomotionPivotLTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.Pivot Left");
		m_LocomotionPivotRTransId = Animator.StringToHash ("Base Layer.Locomotion -> Base Layer.Pivot Right");
	}
	void Start () 
	{
		//
		thirdPersonTarget = TP3_Controller.Instance.thirdPersonTarget;
		thirdPersonLeft = TP3_Camera.Instance.thirdTargetLeft;
		thirdPersonRight = TP3_Camera.Instance.thirdTargetRight;
		//
		ikHeadTarget = TP3_Camera.Instance.ikHeadTarget;

	}
	void Update () 
	{
		
	}
	void FixedUpdate ()
	{
		
	}
	void LateUpdate ()
	{
		//animator.SetFloat ("Stick", TP2_Controller.Instance.moveVector.z);
		animator.SetFloat ("Speed", TP3_Controller.Instance.moveSpeed);
		animator.SetFloat ("Direction", TP3_Controller.Instance.moveVector.x, 0.3f, Time.deltaTime);
		animator.SetBool ("Walk", TP3_Controller.Instance.walkBool);
	}
	public bool IsInJump()
	{
		return (IsInIdleJump() || IsInLocomotionJump());
	}
	
	public bool IsInIdleJump()
	{
		return animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle Jump");
	}
	
	public bool IsInLocomotionJump()
	{
		return animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Run Jump");
	}
	public bool IsInPivot()
	{
		return stateInfo.nameHash == m_LocomotionPivotLId || 
			stateInfo.nameHash == m_LocomotionPivotRId || 
			transInfo.nameHash == m_LocomotionPivotLTransId || 
			transInfo.nameHash == m_LocomotionPivotRTransId;
		return (animator.GetCurrentAnimatorStateInfo(0).IsName("Pivot Left") 
		        || animator.GetCurrentAnimatorStateInfo(0).IsName("Pivot Right") 
		        || animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Locomotion -> Base Layer.Pivot Left") 
		        || animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Locomotion -> Base Layer.Pivot Right"));
		
	}
	public bool IsInLocomotion()
	{
		return stateInfo.fullPathHash == m_LocomotionId;
	}
	void OnAnimatorIK(int layerIndex) 
	{
		if (animator) {
			if(ikActive && TP3_Camera.Instance.camState == TP3_Camera.CamStates.FirstPerson )
			{
				animator.SetLookAtWeight (1);
				animator.SetLookAtPosition (ikHeadTarget.transform.position);
			} 
			else if (ikActive && TP3_Camera.Instance.camState == TP3_Camera.CamStates.Focus)
			{
				//rearamge target positions with cases in mind
				
				if (TP3_Camera.Instance.rotateVector.x > deadZone )
				{
					animator.SetLookAtWeight (1);
					animator.SetLookAtPosition (thirdPersonRight.transform.position);	
				}
				else if(TP3_Camera.Instance.rotateVector.x < -deadZone )
				{
					animator.SetLookAtWeight (1);
					animator.SetLookAtPosition (thirdPersonLeft.transform.position);	
				} 
				else 
				{
					animator.SetLookAtWeight (1);
					animator.SetLookAtPosition (thirdPersonTarget.transform.position);	
				}
			}
		}
	}
}
