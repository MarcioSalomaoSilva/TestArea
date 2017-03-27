using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	public static test Instance;

	public GameObject sword;
	GameObject shield;

	public bool swordBool ;
	bool shieldBool;


	void Awake ()
	{
		Instance = this;
	}
	void Start ()
	{
		sword = transform.Find ("1H_Sword_A").gameObject;
//		shield = transform.Find ("Shield").gameObject;
//		if (shield.gameObject.SetActive (false)) {
//			shieldBool = false;
//		} else {
//			shieldBool = true;
//		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		Items ();
		if (Input.GetButtonDown("RTrigger"))
		{
			if(swordBool)
			{
				TP2_test.Instance.swordBool = true;
				swordBool = false;
			}
			else if (TP2_test.Instance.swordBool = true )
			{
				swordBool = true;
				TP2_test.Instance.swordBool = false;
			}

		}
	}
	void Items ()
	{
		if (swordBool == false) 
		{
			sword.gameObject.SetActive (false);
		} 
		else 
		{
			sword.gameObject.SetActive ( true);
		}
	}
}
