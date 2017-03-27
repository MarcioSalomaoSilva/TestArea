using UnityEngine;
using System.Collections;

public class test_pickup : MonoBehaviour {

	bool pickup =false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	void OnTriggerStay(Collider other) 
	{
		if (Input.GetButtonDown ("RTrigger")) {
			test.Instance.swordBool = true;
			this.gameObject.SetActive(false);
		}
	}
}
