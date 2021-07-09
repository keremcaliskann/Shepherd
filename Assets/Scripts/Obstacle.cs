using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : LivingEntity
{

    void Start()
    {
        startingHealth = 2;
    }

	public virtual void TakeDamage(float damage)
	{
		health -= damage;

		if (health <= 0 && !dead)
		{
			GameObject.Destroy(gameObject);
		}
	}
}
