using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown ("r")) {
			Application.LoadLevel ("test");
		}
	}
	void OnGUI ()
	{
		GUI.Box (new Rect (0, Screen.height-Screen.height/20, Screen.width, Screen.height / 20), 
		         "press r to restart the scene and generate colors, press ESC to exit");
	}
}
