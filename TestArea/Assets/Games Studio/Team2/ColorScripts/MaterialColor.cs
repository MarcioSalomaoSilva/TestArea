using UnityEngine;
using System.Collections;

public class MaterialColor : MonoBehaviour {


	public Renderer rend;
	// Use this for initialization
	void Start () {


		rend = GetComponent<Renderer>();
		rend.material.SetColor("_TopColor", new Color(Random.value,Random.value,Random.value));
		rend.material.SetColor("_BottomColor", new Color(Random.value,Random.value,Random.value));
	}
	
	// Update is called once per frame
	void Update () {

		//rend.material.SetColor("_TopColor", new Color(Random.value,Random.value,Random.value));
	}
}
