using UnityEngine;
using System.Collections;

public class G_Controller : MonoBehaviour {

	public static G_Controller Instance;
	public Transform weaponHold;
	public GameObject startingGun;

	GameObject equippedGun;

	void Awake ()
	{
		Instance = this;
	}
	void Start ()
	{
		if (startingGun != null) 
		{
			EquipGun (startingGun);
		} else {
			Debug.LogError("Create a gun prefab with the G_Starting Gun Script attached to it and assign it in the inspector");
		}
		if (weaponHold == null) 
		{
			Debug.LogError("Create a game object and child it to the pc. Assign it in the editor");
		}
	}

	public void EquipGun(GameObject gunToEquip)
	{
		if (equippedGun != null)
		{
			Destroy(equippedGun.gameObject);
		}
		equippedGun = Instantiate (gunToEquip, weaponHold.position, weaponHold.rotation) as GameObject;
		equippedGun.transform.parent = weaponHold;
	}

	public void Shoot()
	{
		if (equippedGun != null) 
		{
			G_StartingGun.Instance.Shoot();
		}
	}
}
