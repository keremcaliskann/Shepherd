using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character : LivingEntity
{
	Animator animator;
	CharacterController characterController;
	public Transform hand;
	public GameObject weapon;
	Collider weaponCollider;

	public float moveSpeed = 3;

	Joystick joystick1;

	Vector3 moveDirection;
	Vector3 turnDirection;
	Vector3 moveVelocity;

	bool isRunning;
	bool isIdle;

	protected override void Start()
	{
		weapon = Instantiate(weapon, hand.position, hand.rotation);
		weapon.transform.parent = hand.transform;
		weaponCollider = weapon.transform.Find("Axe").GetComponent<Collider>();
		weaponCollider.enabled = false;
		base.Start();
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();
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
			characterController.LookAt(turnDirection);

			moveVelocity = moveDirection.normalized * moveSpeed;
			characterController.Move(moveVelocity);

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
				Invoke("EnableWeaponCollider", 0.2f);
				animator.SetTrigger("Shoot");
				Invoke("DisableWeaponCollider", 0.5f);
			}
		}
	}
	void EnableWeaponCollider()
	{
		weaponCollider.enabled = true;
	}
	void DisableWeaponCollider()
    {
		weaponCollider.enabled = false;
    }

	void OnThisDeath()
	{
		characterController.Move(Vector3.zero);
		animator.SetTrigger("Death");
	}
}
