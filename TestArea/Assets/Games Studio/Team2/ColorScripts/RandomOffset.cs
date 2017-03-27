using UnityEngine;
using System.Collections;

public class RandomOffset : MonoBehaviour 
{
	public Color colorTop;
	public Color colorBottom;
	public Renderer rend;
	public float offSetTop = 0.3f;
	public float offSetBottom = 0.1f;
	// Use this for initialization
	void Start () 
	{
		//average value top
		float valuetop = (colorTop.r + colorTop.g + colorTop.b)/3;
		//average value + random value time 2 * offset float - offset float
		float newValuetop = valuetop + 2*Random.value * offSetTop ;
		float valueRatiotop = newValuetop / valuetop;

		//average value bottom
		float valuebottom = (colorBottom.r + colorBottom.g + colorBottom.b)/3;
		//average value + random value time 2 * offset float - offset float
		float newValuebottom = valuebottom + 2*Random.value * offSetBottom;
		float valueRatiobottom = newValuebottom / valuebottom;

		//do random
		Color newColortop = colorTop * valueRatiotop;
		Color newColorBottom = colorBottom * valueRatiobottom;

		//assign new color
		rend = GetComponent<Renderer>();
		rend.material.SetColor("_TopColor", newColortop);
		rend.material.SetColor("_BottomColor", newColorBottom);
	}
	void Update () 
	{

	}
}
