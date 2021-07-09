using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable {

	public float startingHealth;
	public float health { get; protected set; }
	protected bool dead;

	public event System.Action OnDeath;

	protected virtual void Start() {
		health = startingHealth;
	}

	public void GiveHealth(int heal,int maxHealth)
    {
		startingHealth += maxHealth;
		health += heal;
		if (health > startingHealth)
        {
			health = startingHealth;
		}

    }

	public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection) {
		// Do some stuff here with hit var
		TakeDamage(damage);
	}

	public virtual void TakeDamage(float damage) {
		health -= damage;
		if (health <= 0 && !dead) {
			health = 0;
			Die();
		}
	}

	protected void Die() {
		dead = true;
		if (OnDeath != null) {
			OnDeath();
		}
		Destroy(gameObject, 3f);
	}
}
