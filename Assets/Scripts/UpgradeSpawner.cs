using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSpawner : MonoBehaviour
{
    public Upgrade[] upgrade;
    
    Transform randomTile;

    WaveController waveController;

    float timeBetweenSpawns;
    float nextSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        waveController = FindObjectOfType<WaveController>();

        timeBetweenSpawns = 10f;
        nextSpawnTime = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawnTime && waveController != null)
        {

            randomTile = waveController.generatedMap.GetRandomOpenTile();

            nextSpawnTime = Time.time + timeBetweenSpawns;

            Upgrade spawnedUpgrade = Instantiate(upgrade[Random.Range(0, upgrade.Length)], randomTile.position + Vector3.up * 1, Quaternion.identity) as Upgrade;
        }
    }
}
