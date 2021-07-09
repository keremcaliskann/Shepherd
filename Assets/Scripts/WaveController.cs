using UnityEngine;
using System.Collections;

public class WaveController : MonoBehaviour
{

	public Wave baseWave;
	public Wave currentWave;

	LivingEntity playerEntity;
	Transform playerT;

	int currentWaveNumber;

	public int enemiesRemainingToSpawn;
	public int enemiesRemainingAlive;

	public MapGenerator generatedMap;

	public event System.Action<int> OnNewWave;

	void Start()
	{
		playerEntity = FindObjectOfType<Character>();
		playerT = playerEntity.transform;

		generatedMap = FindObjectOfType<MapGenerator>();

		currentWaveNumber = 0;
		baseWave.enemyCount = 5;
		baseWave.timeBetweenSpawns = 2;

		FindObjectOfType<EnemySpawner>().StartEnemySpawner();
	}

	void ResetPlayerPosition()
	{
		playerT.position = generatedMap.GetRandomOpenTile().position + Vector3.up * 2;
	}

	public void NextWave()
	{
		if (currentWaveNumber < 1)
		{
			currentWave = baseWave;
		}
		else
		{
			currentWave.enemyCount += currentWaveNumber * 3;

			if (currentWave.timeBetweenSpawns > 0.1f)
			{
				currentWave.timeBetweenSpawns -= 0.1f;
			}

			playerEntity.GiveHealth(2, 1);
		}
		enemiesRemainingToSpawn = currentWave.enemyCount;
		enemiesRemainingAlive = enemiesRemainingToSpawn;

		currentWaveNumber++;

		OnNewWave(currentWaveNumber);

		ResetPlayerPosition();

	}

	[System.Serializable]
	public class Wave
	{
		public bool infinite;
		public int enemyCount;
		public float timeBetweenSpawns;
	}

}
