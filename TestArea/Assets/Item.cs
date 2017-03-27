using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public static Item Instance;
	
	public enum H1_Sword_A
	{
		Hand, Equipped, False
	}
	public enum H1_Sword_B 
	{
		Hand, equipped, False
	}
	public enum H1_Undead_Sword
	{
		Hand, Equipped, False
	}
	public enum Bronze_Shield 
	{
		Hand, equipped, False
	}	
	public enum Round_Shield
	{
		Hand, Equipped, False
	}

	public H1_Sword_A h1SwordA = H1_Sword_A.False;
	public H1_Sword_B h1SwordB = H1_Sword_B.False;
	public H1_Undead_Sword h1UndeadSword = H1_Undead_Sword.False;
	public Bronze_Shield bronzeShield = Bronze_Shield.False;
	public Round_Shield roundShield = Round_Shield.False;

	void Awake () 
	{
		Instance = this;
	}
	void Start () 
	{
	
	}
	void Update () 
	{
	
	}
}
