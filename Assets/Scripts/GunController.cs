using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{

	public Transform weaponHold;
	public Gun[] allGuns;
	public Gun equippedGun;
	public int equippedGunIndex;

	void Start()
	{
		if (allGuns[0] != null)
		{
			EquipGun(allGuns[equippedGunIndex]);
		}
	}

	public void EquipGun(Gun gunToEquip)
	{
		if (equippedGun != null)
		{
			Destroy(equippedGun.gameObject);
		}
		equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
		equippedGun.transform.parent = weaponHold;
	}

	public void OnTriggerHold()
	{
		if (equippedGun != null)
		{
			equippedGun.OnTriggerHold();
		}
	}

	public void OnTriggerRelase()
	{
		if (equippedGun != null)
		{
			equippedGun.OnTriggerRelase();
		}
	}

	public float GunHeight
	{
		get
		{
			return weaponHold.position.y;
		}
	}

	public void Reload()
	{
		if (equippedGun != null)
		{
			equippedGun.Reload();
		}
	}

	public Gun EquippedGun
	{
		get
		{
			return equippedGun;
		}
	}

	public void NextGun()
	{
		if (equippedGunIndex < allGuns.Length - 1)
		{
			equippedGunIndex++;
		}
		else
		{
			equippedGunIndex = 0;
		}
		EquipGun(allGuns[equippedGunIndex]);
	}
}