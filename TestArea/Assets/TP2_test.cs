using UnityEngine;
using System.Collections;

public class TP2_test : MonoBehaviour {

	public static TP2_test Instance;

	public GameObject sword;

	public bool swordBool = false;

	void Awake () 
	{
		Instance = this;
		sword = transform.Find ("1H_Sword_A").gameObject;
		sword.SetActive (false);
	}
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (swordBool) {
			sword.SetActive (true);
		} else {
			sword.SetActive(false);
		}
	}
}
