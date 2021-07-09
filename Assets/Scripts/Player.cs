using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerController))]
public class Player : LivingEntity
{
	public Animator animator;

	public float moveSpeed = 3;

	PlayerController controller;
	GunController gunController;

	Joystick joystick1;

	Vector3 moveDirection;
	Vector3 turnDirection;
	Vector3 moveVelocity;

	List<Enemy> aliveEnemies;
	Enemy target;

	bool isRunning;
	bool isIdle;

	protected override void Start()
	{
		base.Start();
		animator = GetComponent<Animator>();
		controller = GetComponent<PlayerController>();
		gunController = GetComponent<GunController>();
		aliveEnemies = FindObjectOfType<EnemySpawner>().aliveEnemies;
		joystick1 = GameObject.Find("Joystick1").GetComponent<Joystick>();
		isIdle = true;
		OnDeath += OnThisDeath;
		turnDirection = Vector3.forward;
	}

	void Update()
	{
		if (!dead)
		{
			moveDirection = new Vector3(joystick1.Horizontal, 0, joystick1.Vertical);

			if (moveDirection != Vector3.zero)
			{
				turnDirection = moveDirection;
			}
			controller.LookAt(turnDirection);

			moveVelocity = moveDirection.normalized * moveSpeed;
			controller.Move(moveVelocity);

			if (moveDirection != Vector3.zero)
			{
				isRunning = true;
				isIdle = false;
			}
			else
			{
				isRunning = false;
				isIdle = true;
			}
			animator.SetBool("Run", isRunning);
			animator.SetBool("Idle", isIdle);

			if (transform.position.y < -10)
			{
				TakeDamage(health);
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				animator.SetTrigger("Shoot");
				//gunController.Reload();
			}/*
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				gunController.NextGun();
			}
            if (Input.GetKeyDown(KeyCode.Space))
            {
				gunController.OnTriggerHold();
            }
			if (Input.GetKeyUp(KeyCode.Space))
			{
				gunController.OnTriggerRelase();
			}*/
		}
	}

    void OnThisDeath()
    {
		controller.Move(Vector3.zero);
		Destroy(gunController.equippedGun);
		animator.SetTrigger("Death");
    }

	Enemy GetClosestEnemy(List<Enemy> enemies)
	{
		Enemy tMin = null;
		float minDist = 10f;
		Vector3 playerPos = transform.position;
		foreach (Enemy t in enemies)
		{
			float dist = Vector3.Distance(t.transform.position, playerPos);
			if (dist < minDist)
			{
				tMin = t;
				minDist = dist;
			}
		}
		return tMin;

		/*
		aliveEnemies = FindObjectOfType<EnemySpawner>().aliveEnemies;

		if (aliveEnemies.Count > 0)
		{
			target = GetClosestEnemy(aliveEnemies);
			if (target != null)
			{
				isLockedEnemy = true;
				controller.LookAt(target.transform.position);
			}
			else
			{
				isLockedEnemy = false;
			}
		}*/
	}
}
