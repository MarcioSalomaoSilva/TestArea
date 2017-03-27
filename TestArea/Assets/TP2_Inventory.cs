using UnityEngine;
using System.Collections;

public class TP2_Inventory : MonoBehaviour {

	public static TP2_Inventory Instance;
	
	public enum slotStates
	{
		Empty, Used, Shield, Sword, Back
	}

	public enum hasItem
	{
		LeftYes, LeftNo, RightYes, RightNo, LeftSideN0, LeftSideYes, RightSideNO, RightSideYes
	}

	public slotStates slotState;
	public hasItem item;

	float deadZone = 0.1f;

	bool hasItemLeftBool;
	bool hasItemRightBool;
	bool isShield;
	bool isSword;
	bool shieldEquipped;

	GameObject test;
	GameObject leftHand;
	GameObject rightHand;
	GameObject leftSide;
	GameObject rightSide;
	GameObject backLeft;
	GameObject backRight;

	GameObject swordTest;

	Vector3 Dpad;

	void Awake ()
	{
		leftHand = GameObject.Find ("Left Hand");
		rightHand = GameObject.Find ("Right Hand");
		leftSide = GameObject.Find ("Left Side");
		rightSide = GameObject.Find ("Right Side");
		backLeft = GameObject.Find ("Back Left");
		backRight = GameObject.Find ("Back Right");

	}
	void Start () 
	{
	
	}
	void Update () 
	{
		//check if objects have children
		detectItem ();
		//get player input
		getInput ();
		// hasItem cases
		hasItemCheck ();
		//slot cases
		slotCasesCheck ();
	}
	void FixedUpdate ()
	{

	}
	void LateUpdate()
	{

	}
	// hasItem cases
	void hasItemCheck ()
	{
		switch (item) 
		{
		case hasItem.LeftNo:
			break;
		case hasItem.LeftYes:
				




			break;
		case hasItem.RightNo:
			break;
		case hasItem.RightYes:
			break;
		}
	}
	//slot cases
	void slotCasesCheck ()
	{
		switch (slotState) 
		{
		case slotStates.Empty:
			break;
		case slotStates.Used:
			break;
		case slotStates.Shield:
			break;
		case slotStates.Sword:
			break;
		}
	}
	void detectItem ()
	{
		//detect if player has item, check if the gameobject has a child
		if (leftHand.transform.childCount > 0f)
		{
			item = hasItem.LeftYes;
		}
		else 
		{
			item = hasItem.LeftNo;
		}
		//right hand
		if (rightHand.transform.childCount > 0f) 
		{
			item = hasItem.RightYes;
		} 
		else 
		{
			item = hasItem.RightNo;
		}
		if (leftSide.transform.childCount >0f)
		{

		}
		else
		{

		}
	}
	void getInput()
	{
		Dpad = new Vector2 (Input.GetAxis ("DHorizontal"), Input.GetAxis ("DVertical"));

		//get left trigger and dpad input but check if item in hand first
		if (Input.GetButtonDown ("LTrigger"))
		{

			if (item == hasItem.LeftYes) 
			{
				//right
				if(Dpad.x > deadZone)
				{

					if(slotState == slotStates.Empty)
					{

						//make an item active
						//deactivate current item
					}
					if(slotState == slotStates.Used)
					{
						// do nothing or play animation
					}
				}
				//left
				if(Dpad.x < -deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						if(slotState == slotStates.Shield)
						{
							//do nothing
						}
						if(slotState == slotStates.Sword)
						{
							//store
						}
					}
					if(slotState == slotStates.Used)
					{
						// do nothing or play animation
					}
				}
				//up
				if(Dpad.y > deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						//store
						if(slotState == slotStates.Shield)
						{
							//do nothing
						}
						if(slotState == slotStates.Sword)
						{
							//store
						}
					}
					if(slotState == slotStates.Used)
					{
						//do nothing
					}
				}
				//down
				if(Dpad.y < -deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						//chck to see if its a sword or shield
						if(slotState == slotStates.Shield)
						{
							//store
						}
						if(slotState == slotStates.Sword)
						{
							//store
						}
					}
					if(slotState == slotStates.Used)
					{
						//do nothing or animation
					}
				}
			} 
			//get right trigger and dpad input
			else if (item == hasItem.LeftNo) 
			{
				//right
				if(Dpad.x > deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						//do nothing
					}
					if(slotState == slotStates.Used)
					{
						//equip item
					}
				}
				//left
				if(Dpad.x < -deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						//do nothing
					}
					if(slotState == slotStates.Used)
					{
						//equip item
					}
				}
				//up
				if(Dpad.y > deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						//do nothing
					}
					if(slotState == slotStates.Used)
					{
						//equip item
					}

				}
				//down
				if(Dpad.y < -deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						//do nothing
					}
					if(slotState == slotStates.Used)
					{
						//equip
					}
				}
			}
		}
		//--------------------------------------------------------------------------------------------------------------------------------------------
		if (Input.GetButtonDown ("RTrigger"))
		{
			
			if (item == hasItem.RightYes) 
			{
				//right
				if(Dpad.x > deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						
					}
					if(slotState == slotStates.Used)
					{
						
					}
					if(slotState == slotStates.Back)
					{
						
					}
					if(slotState == slotStates.Shield)
					{
						
					}
				}
				//left
				if(Dpad.x < -deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						
					}
					if(slotState == slotStates.Used)
					{
						
					}
					if(slotState == slotStates.Back)
					{
						
					}
					if(slotState == slotStates.Shield)
					{
						
					}
				}
				//up
				if(Dpad.y > deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						
					}
					if(slotState == slotStates.Used)
					{
						
					}
					if(slotState == slotStates.Back)
					{
						
					}
					if(slotState == slotStates.Shield)
					{
						
					}
				}
				//down
				if(Dpad.y < -deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						
					}
					if(slotState == slotStates.Used)
					{
						
					}
					if(slotState == slotStates.Back)
					{
						
					}
					if(slotState == slotStates.Shield)
					{
						
					}
				}
			} 
			//get right trigger and dpad input
			else if (item == hasItem.RightNo) 
			{
				//right
				if(Dpad.x > deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						
					}
					if(slotState == slotStates.Used)
					{
						
					}
					if(slotState == slotStates.Back)
					{
						
					}
					if(slotState == slotStates.Shield)
					{
						
					}
				}
				//left
				if(Dpad.x < -deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						
					}
					if(slotState == slotStates.Used)
					{
						
					}
					if(slotState == slotStates.Back)
					{
						
					}
					if(slotState == slotStates.Shield)
					{
						
					}
				}
				//up
				if(Dpad.y > deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						
					}
					if(slotState == slotStates.Used)
					{
						
					}
					if(slotState == slotStates.Back)
					{
						
					}
					if(slotState == slotStates.Shield)
					{
						
					}
				}
				//down
				if(Dpad.y < -deadZone)
				{
					if(slotState == slotStates.Empty)
					{
						
					}
					if(slotState == slotStates.Used)
					{
						
					}
					if(slotState == slotStates.Back)
					{
						
					}
					if(slotState == slotStates.Shield)
					{
						
					}
				}
			}
		}
	}
}
