using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{

	public Map map;

	public Transform[] tilePrefabs;
	public Transform[] grassPrefabs;
	public Transform[] enviromentPrefabs;
	public Transform enviromentPrefab;
	public Transform navmeshFloor;
	public Transform navmeshMaskPrefab;
	Coord enviromentCoord;
	public Vector2 maxMapSize;

	float outlinePercent = -0.1f;

	public float tileSize;
	List<Coord> allTileCoords;
	Queue<Coord> shuffledTileCoords;
	Queue<Coord> shuffledOpenTileCoords;
	Transform[,] tileMap;

	public Map currentMap;
	public bool lockCurrentMap;

	void Awake()
	{
		map.mapSize.x = 10;
		map.mapSize.y = 10;
		map.obstaclePercent = 0f;
		map.seed = 1;
		map.minObstacleHeight = 1f;
		map.maxObstacleHeight = 1f;

		FindObjectOfType<WaveController>().OnNewWave += OnNewWave;
	}

	void OnNewWave(int waveIndex)
	{
		if (!lockCurrentMap)
		{
			if (waveIndex <= 1)
			{
				currentMap = map;
			}
			else
			{
				currentMap.mapSize.x += 2;
				currentMap.mapSize.y += 2;
				currentMap.obstaclePercent += 0.01f;
				currentMap.seed += 1;
			}
			GenerateMap();
		}
	}

	public void GenerateMap()
	{
		currentMap = map;
		//currentMap.seed = Random.Range(1, 1000);
		tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
		System.Random prng = new System.Random(currentMap.seed);
		GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, .05f, currentMap.mapSize.y * tileSize);

		// Generating coords
		allTileCoords = new List<Coord>();
		for (int x = 0; x < currentMap.mapSize.x; x++)
		{
			for (int y = 0; y < currentMap.mapSize.y; y++)
			{
				allTileCoords.Add(new Coord(x, y));
			}
		}
		shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

		// Create map holder object
		string holderName = "Generated Map";
		if (transform.Find(holderName))
		{
			DestroyImmediate(transform.Find(holderName).gameObject);
		}

		Transform mapHolder = new GameObject(holderName).transform;
		mapHolder.parent = transform;

		// Spawning tiles
		for (int x = 0; x < currentMap.mapSize.x; x++)
		{
			for (int y = 0; y < currentMap.mapSize.y; y++)
			{
				Vector3 tilePosition = CoordToPosition(x, y);
				//Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as Transform;
				Transform newTile = Instantiate(tilePrefabs[Random.Range(0,tilePrefabs.Length)], tilePosition, Quaternion.identity) as Transform;
				//Instantiate(grassPrefabs[Random.Range(0, grassPrefabs.Length)], tilePosition, Quaternion.identity);
				newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
				newTile.parent = mapHolder;
				tileMap[x, y] = newTile;
			}
		}

		// Spawning obstacles
		bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

		int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
		int currentObstacleCount = 0;
		List<Coord> allOpenCoords = new List<Coord>(allTileCoords);

		for (int i = 0; i < obstacleCount; i++)
		{
			enviromentPrefab = GetRandomEnviromentPrefab();
			enviromentCoord = GetRandomCoord();

			obstacleMap[enviromentCoord.x, enviromentCoord.y] = true;
			currentObstacleCount++;

			if (enviromentCoord != currentMap.mapCentre)
			{
				//float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
				//Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
				Vector3 obstaclePosition = CoordToPosition(enviromentCoord.x, enviromentCoord.y);

				//Transform newObstacle = Instantiate(enviromentPrefabs[Random.Range(0,enviromentPrefabs.Length)], obstaclePosition, Quaternion.Euler(0, Random.Range(0, 360), 0)) as Transform;
				Transform newObstacle = Instantiate(enviromentPrefab, obstaclePosition, Quaternion.Euler(0, Random.Range(0, 360), 0)) as Transform;

				newObstacle.parent = mapHolder;
				//newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);
				
				/*
				Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
				Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
				obstacleRenderer.sharedMaterial = obstacleMaterial;*/

				allOpenCoords.Remove(enviromentCoord);
			}
			else
			{
				obstacleMap[enviromentCoord.x, enviromentCoord.y] = false;
				currentObstacleCount--;
			}
		}

		shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), Random.Range(1, 1000)));

		// Creating navmesh mask
		Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
		maskLeft.parent = mapHolder;
		maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

		Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
		maskRight.parent = mapHolder;
		maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

		Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
		maskTop.parent = mapHolder;
		maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

		Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
		maskBottom.parent = mapHolder;
		maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

		navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;

	}

	Vector3 CoordToPosition(int x, int y)
	{
		return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
	}

	public Coord GetRandomCoord()
	{
		Coord randomCoord = shuffledTileCoords.Dequeue();
		shuffledTileCoords.Enqueue(randomCoord);
		return randomCoord;
	}

	public Transform GetRandomOpenTile()
	{
		Coord randomCoord = shuffledOpenTileCoords.Dequeue();
		shuffledOpenTileCoords.Enqueue(randomCoord);
		return tileMap[randomCoord.x, randomCoord.y];
	}

	[System.Serializable]
	public struct Coord
	{
		public int x;
		public int y;

		public Coord(int _x, int _y)
		{
			x = _x;
			y = _y;
		}

		public static bool operator ==(Coord c1, Coord c2)
		{
			return c1.x == c2.x && c1.y == c2.y;
		}

		public static bool operator !=(Coord c1, Coord c2)
		{
			return !(c1 == c2);
		}
	}

	[System.Serializable]
	public class Map
	{

		public Coord mapSize;
		[Range(0, 1)]
		public float obstaclePercent;
		public int seed;
		public float minObstacleHeight;
		public float maxObstacleHeight;

		public Coord mapCentre
		{
			get
			{
				return new Coord(mapSize.x / 2, mapSize.y / 2);
			}
		}

	}

	Transform GetRandomEnviromentPrefab()
    {
		int random = Random.Range(0, 100);
		if (random > 0 && random < 5)
		{
			//Tree
			return enviromentPrefabs[Random.Range(0, 3)];
		}
		if (random > 5 && random < 60)
		{
			//Grass
			return enviromentPrefabs[Random.Range(3, 6)];
		}
		if (random > 60 && random < 70)
		{
			//Rock
			return enviromentPrefabs[Random.Range(6, 10)];
		}
		if (random > 70 && random < 80)
		{
			//Other
			return enviromentPrefabs[Random.Range(10, 12)];
		}
		if (random > 80 && random < 90)
		{
			//Grass2
			return enviromentPrefabs[Random.Range(12, 14)];
		}
		if (random > 90 && random < 100)
		{
			//Grass2
			return enviromentPrefabs[Random.Range(14, 17)];
		}
		return enviromentPrefabs[Random.Range(0, enviromentPrefabs.Length)];
	}
}
