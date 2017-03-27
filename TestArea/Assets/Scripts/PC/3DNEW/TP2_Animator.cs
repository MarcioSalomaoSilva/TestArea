using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof(TP2_Controller))]
public class TP2_Animator : MonoBehaviour {

	public static TP2_Animator Instance;

	public Animator animator;
	public bool ikActive = false;
	public GameObject headTarget;
	public GameObject focusTarget;
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
	float speedDamp = 0.05f;

	void Awake ()
	{
		Instance = this;
		animator = GetComponent<Animator> ();
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
		headTarget = TP2_Camera.Instance.headTarget;
		focusTarget = TP2_Camera.Instance.focusTarget;
	}
	void Update () 
	{

	}
	void FixedUpdate ()
	{

	}
	void LateUpdate ()
	{
		animator.SetFloat ("Stick", TP2_Controller.Instance.moveVector.z);
		animator.SetFloat ("Speed", TP2_Controller.Instance.moveSpeed, speedDamp, Time.deltaTime);
		animator.SetFloat ("Direction", TP2_Controller.Instance.moveVector.x, TP2_Controller.Instance.directionDampTime, Time.deltaTime);
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
//		return (animator.GetCurrentAnimatorStateInfo(0).IsName("Pivot Left") 
//		        || animator.GetCurrentAnimatorStateInfo(0).IsName("Pivot Right") 
//		        || animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Locomotion -> Base Layer.Pivot Left") 
//		        || animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Locomotion -> Base Layer.Pivot Right"));

	}
	public bool IsInLocomotion()
	{
		return stateInfo.fullPathHash == m_LocomotionId;
	}
	void OnAnimatorIK(int layerIndex) 
	{
		if (animator) {
			if(ikActive && TP2_Camera.Instance.mainCamState == TP2_Camera.mainCamStates.FirstPerson )
			{
				animator.SetLookAtWeight (1);
				animator.SetLookAtPosition (headTarget.transform.position);
			} 
			else if (ikActive && TP2_Camera.Instance.mainCamState == TP2_Camera.mainCamStates.Focus)
			{
				//rearamge target positions with cases in mind

				if (TP2_Camera.Instance.rotateVector.x > TP2_Camera.Instance.deadZone )
				{
					animator.SetLookAtWeight (1);
					animator.SetLookAtPosition (TP2_Camera.Instance.focusTargetRight.transform.position);	
				}
				else if(TP2_Camera.Instance.rotateVector.x < -TP2_Camera.Instance.deadZone )
				{
					animator.SetLookAtWeight (1);
					animator.SetLookAtPosition (TP2_Camera.Instance.focusTargetLeft.transform.position);	
				} 
				else 
				{
					animator.SetLookAtWeight (1);
					animator.SetLookAtPosition (TP2_Camera.Instance.focusTarget.transform.position);	
				}
			}
		}
	}
}
