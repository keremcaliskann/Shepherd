using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	Player player;

	public enum FireMode { Auto, Burst, Single};
	public FireMode fireMode;

	public Transform[] projectTileSpawn;
	public Projectile projectile;
	public float msBetweenShots = 100;
	public float muzzleVelocity = 35;
	public int burstCount;
	public int magSize;
	public float reloadTime = .5f;
	public float damage;

	public Transform shell;
	public Transform shellEjection;
	MuzzleFlash muzzleFlash;
	float nextShotTime;

	bool triggerRelasedSinceLastShot;
	int shotsRemainingInBurst;
	public int remainingBulletCount;
	public bool isReloading;

	public float spray;
	public float maxspray;
	public float minspray;

	Vector3 recoilSmoothDampVelocity;

	private void Start()
    {
		player = FindObjectOfType<Player>();
		triggerRelasedSinceLastShot = true;
		switch (fireMode)
        {
			case FireMode.Auto:
				maxspray = 10;
				minspray = 3;
				break;
			case FireMode.Burst:
				maxspray = 7;
				minspray = 2;
				break;
			case FireMode.Single:
				maxspray = 3;
				minspray = 1;
				break;
			default:
				break;
        }
		spray = minspray;
        muzzleFlash = GetComponent<MuzzleFlash>();
		shotsRemainingInBurst = burstCount;

		remainingBulletCount = magSize;
    }

	void LateUpdate()
	{
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, 0.1f);
	}

	void Shoot()
	{
		if (!isReloading && Time.time > nextShotTime && remainingBulletCount > 0)
		{
			if (fireMode == FireMode.Burst)
			{
				if (shotsRemainingInBurst == 0)
				{
					return;
				}
				shotsRemainingInBurst--;
			}
			else if (fireMode == FireMode.Single)
			{
				if (!triggerRelasedSinceLastShot)
				{
					return;
				}
			}
			player.animator.SetTrigger("Shoot");
			muzzleFlash.Activate();

			for (int i = 0; i < projectTileSpawn.Length; i++)
			{
                if (remainingBulletCount == 0)
                {
					break;
                }
				remainingBulletCount--;
				nextShotTime = Time.time + msBetweenShots / 1000;
				Projectile newProjectile = Instantiate(projectile, projectTileSpawn[i].position, projectTileSpawn[i].rotation * Quaternion.Euler(0, Random.Range(-spray, spray), 0)) as Projectile;
				newProjectile.SetSpeed(muzzleVelocity);
			}

			Instantiate(shell, shellEjection.position, shellEjection.rotation);
			
			transform.localPosition -= Vector3.forward * 0.2f;
		}
	}

	public void Reload()
    {
		if (!isReloading && remainingBulletCount != magSize)
		{
			spray = minspray;
			StartCoroutine(AnimateReload());
		}
    }

	IEnumerator AnimateReload()
	{
		isReloading = true;
		yield return new WaitForSeconds(0.2f);
		/*
		float startRotation = transform.eulerAngles.x;
		float endRotation = startRotation - 720.0f;
		float t = 0.0f;

		while (t < reloadTime)
		{
			t += Time.deltaTime;
			float xRotation = Mathf.Lerp(startRotation, endRotation, t / reloadTime) % 360.0f;
			transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, transform.eulerAngles.z);
			yield return null;
		}
		*/
		yield return new WaitForSeconds(0.3f);

		isReloading = false;
		remainingBulletCount = magSize;

	}

	public void OnTriggerHold()
    {
		Shoot();
		if (spray <= maxspray && !isReloading)
		{
			spray += Time.deltaTime * 10f;
        }
		triggerRelasedSinceLastShot = false;
    }

	public void OnTriggerRelase()
	{
		spray = minspray;
		triggerRelasedSinceLastShot = true;
		shotsRemainingInBurst = burstCount;
	}
}
