using UnityEngine;
using System.Collections;

public class TEST_Gravity : MonoBehaviour {

	public static TEST_Gravity Instance;

	public GameObject centerOfGravity;
	public GameObject targetOfGravity;
	public Vector3 directionOfGravity;

	void Awake () 
	{
		Instance = this;
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Debug.DrawLine(targetOfGravity.transform.position, centerOfGravity.transform.position, Color.red);
		directionOfGravity = targetOfGravity.transform.position - centerOfGravity.transform.position;
	}
}
