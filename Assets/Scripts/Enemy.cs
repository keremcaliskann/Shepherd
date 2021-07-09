using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
	GameObject canvas;
	public enum State { Idle, Chasing, Attacking };
	State currentState;

	Animator animator;
	public ParticleSystem deathEffect;
	public ParticleSystem hurthEffect;

	public static event System.Action OnDeathStatic;

	NavMeshAgent pathfinder;
	Transform target;
	LivingEntity targetEntity;

	float attackDistanceThreshold = .5f;
	float timeBetweenAttacks = 2;
	float damage = 1;

	float nextAttackTime;
	float myCollisionRadius;
	float targetCollisionRadius;

	public int myNumber;

	bool hasTarget;

	public float wanderRadius;
	public float wanderTimer;
	private float timer;

	private void Awake()
	{
		pathfinder = GetComponent<NavMeshAgent>();
		
		if (GameObject.FindGameObjectWithTag("Player") != null)
		{
			hasTarget = true;

			target = GameObject.FindGameObjectWithTag("Player").transform;
			targetEntity = target.GetComponent<LivingEntity>();

			this.OnDeath += OnThisDeath;

			myCollisionRadius = GetComponent<CapsuleCollider>().radius;
			targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
		}
	}

	protected override void Start()
	{
		base.Start();
		animator = GetComponent<Animator>();

		targetEntity.OnDeath += OnTargetDeath;
		wanderRadius = 20f;
		wanderTimer = 2f;
		timer = wanderTimer;
		canvas = transform.Find("Canvas").gameObject;
	}

	public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		Destroy(Instantiate(hurthEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, hurthEffect.main.startLifetime.constant);

		if (damage >= health)
		{
			if (OnDeathStatic != null)
			{
				OnDeathStatic();
				FindObjectOfType<EnemySpawner>().aliveEnemies.Remove(this);
			}
			Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);

		}
		base.TakeHit(damage, hitPoint, hitDirection);

		canvas.SetActive(true);

		Invoke("DeactiveHealtBar", 3.0f);
	}

	void DeactiveHealtBar()
	{
		canvas.SetActive(false);
	}
	void OnTargetDeath()
	{
		StopAllCoroutines();
		//StopCoroutine("Attack");
		hasTarget = false;
		currentState = State.Idle;
		animator.SetBool("Walking", false);
		animator.SetBool("Idle", true);
	}

	void OnThisDeath()
	{
		Destroy(transform.GetComponent<Collider>());
		hasTarget = false;
		pathfinder.enabled = false;
		currentState = State.Idle;
		animator.SetBool("Walking", false);
		animator.SetBool("Idle", true);
		animator.SetTrigger("Death");
	}
	void Update()
	{
		if (HasNearTarget())
		{
			StartCoroutine(UpdatePath());

			if (Time.time > nextAttackTime)
			{
				float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
				if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
				{
					nextAttackTime = Time.time + timeBetweenAttacks;
					StartCoroutine(Attack());
				}

			}
		}
		if (!HasNearTarget())
		{
			StopCoroutine("UpdatePath");
			//StartCoroutine(Wander());
		}

	}

	IEnumerator Attack()
	{

		currentState = State.Attacking;
		pathfinder.enabled = false;

		float attackSpeed = 3;
		float percent = 0;

		bool hasAppliedDamage = false;

		while (percent <= 1)
		{
			if (percent >= .5f && !hasAppliedDamage)
			{
				animator.SetTrigger("Attack");
				yield return new WaitForSeconds(0.4f);
				hasAppliedDamage = true;
				targetEntity.TakeDamage(damage);
			}
			percent += Time.deltaTime * attackSpeed;
			yield return null;
		}

		currentState = State.Chasing;
		pathfinder.enabled = true;
	}

	IEnumerator UpdatePath()
	{
		float refreshRate = .25f;
		if (currentState == State.Chasing)
		{
			animator.SetBool("Walking", true);
			animator.SetBool("Idle", false);
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
			if (!dead)
			{
				pathfinder.SetDestination(targetPosition);
			}
		}
		yield return new WaitForSeconds(refreshRate);
		animator.SetBool("Walking", false);
		animator.SetBool("Idle", true);
	}

	IEnumerator Wander()
	{
		yield return new WaitForSeconds(Random.Range(1, 5));
		if (currentState == State.Idle)
		{
			animator.SetBool("Walking", true);
			animator.SetBool("Idle", false);
			currentState = State.Idle;
			timer += Time.deltaTime;

			if (timer >= wanderTimer)
			{
				Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
				pathfinder.SetDestination(newPos);
				timer = 0;
			}
		}
	}
	public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
	{
		Vector3 randDirection = Random.insideUnitSphere * dist;
		randDirection += origin;
		NavMeshHit navHit;
		NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
		return navHit.position;
	}

	bool HasNearTarget()
	{
		if (target != null && !dead)
		{
			float dist = Vector3.Distance(transform.position, target.position);
			if (dist < 8f && hasTarget)
			{
				pathfinder.enabled = true;
				currentState = State.Chasing;
				return true;
			}
		}
		pathfinder.enabled = false;
		currentState = State.Idle;
		return false;
	}
}