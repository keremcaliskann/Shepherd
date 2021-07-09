using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
	WaveController waveController;

	public Enemy enemy;

	public List<Enemy> aliveEnemies;
	int spawnedEnemyNumber;

	LivingEntity playerEntity;

	float nextSpawnTime;

	public bool isDisabled;
	public event System.Action<int> OnNewWave;

	public void StartEnemySpawner()
	{
		spawnedEnemyNumber = 0;

		playerEntity = FindObjectOfType<Character>();

		playerEntity.OnDeath += OnPlayerDeath;

		waveController = FindObjectOfType<WaveController>();

		waveController.NextWave();
	}

	void Update()
	{
		if (!isDisabled)
		{
			if ((waveController.enemiesRemainingToSpawn > 0 || waveController.currentWave.infinite) && Time.time > nextSpawnTime)
			{
				waveController.enemiesRemainingToSpawn--;
				nextSpawnTime = Time.time + waveController.currentWave.timeBetweenSpawns;

				StartCoroutine(SpawnEnemy());
			}
		}
	}

	public IEnumerator SpawnEnemies()
	{
		float spawnDelay = 1;
		float tileFlashSpeed = 4;

		Transform randomTile = waveController.generatedMap.GetRandomOpenTile();
		Material tileMat = randomTile.GetComponent<Renderer>().material;
		Color initialColour = tileMat.color;
		Color flashColour = Color.red;
		float spawnTimer = 0;

		while (spawnTimer < spawnDelay)
		{
			tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

			spawnTimer += Time.deltaTime;
			yield return null;
		}


		Enemy spawnedEnemy = Instantiate(enemy, randomTile.position + Vector3.up, Quaternion.identity) as Enemy;
		//Enemy spawnedEnemy = Instantiate(enemy[Random.Range(0, enemy.Length)], randomTile.position + Vector3.up, Quaternion.identity) as Enemy;
		spawnedEnemy.OnDeath += OnEnemyDeath;
		spawnedEnemy.myNumber = spawnedEnemyNumber;
		aliveEnemies.Add(spawnedEnemy);
		spawnedEnemyNumber++;
	}

	void OnPlayerDeath()
	{
		isDisabled = true;
	}

	void OnEnemyDeath()
	{
		waveController.enemiesRemainingAlive--;

		if (waveController.enemiesRemainingAlive == 0)
		{
			waveController.NextWave();
			waveController.enemiesRemainingAlive = waveController.enemiesRemainingToSpawn;
			spawnedEnemyNumber = 0;
		}
	}
}
