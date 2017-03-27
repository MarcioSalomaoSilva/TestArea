using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour {
	
	public Transform target;
	public float angularSpeed;
	
	[SerializeField][HideInInspector] 
	private Vector3 initialOffset;
	
	private Vector3 currentOffset;
	
	[ContextMenu("Set Current Offset")]
	private void SetCurrentOffset () {
		if(target == null) {
			return;
		}
		
		initialOffset = transform.position - target.position;
	}
	
	private void Start () {
		if(target == null) {
			Debug.LogError ("Assign a target for the camera in Unity's inspector");
		}
		
		currentOffset = initialOffset;
	}
	
	private void LateUpdate () {
		transform.position = target.position + currentOffset;
		
		float horizontal = Input.GetAxis ("RHorizontal") * angularSpeed * Time.deltaTime;
		if(!Mathf.Approximately (horizontal, 0f)) {
			transform.RotateAround (target.position, Vector3.up, horizontal);
			currentOffset = transform.position - target.position;
		}
		float vertical = Input.GetAxis ("RVertical") * angularSpeed * Time.deltaTime;
		if(!Mathf.Approximately (vertical, 0f)) {
			transform.RotateAround (target.position, Vector3.right, vertical);
			currentOffset = transform.position - target.position;
		}
		transform.LookAt (target);
	}
}