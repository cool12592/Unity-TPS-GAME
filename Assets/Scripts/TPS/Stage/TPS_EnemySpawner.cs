using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_EnemySpawner : MonoBehaviour
{
    [SerializeField]
    TPS_SpawnGate[] spawnGates;

    // Start is called before the first frame update

    public void EnemySpawn(int needEnemyNum)
    {
        if (spawnGates.Length <= 0)
            return;

        if (spawnGates.Length < needEnemyNum)
            needEnemyNum = spawnGates.Length;

        SuffleSpawnGates();

        for (int i=0; i< needEnemyNum; i++)
            spawnGates[i].EnemySpawn();
    }

    void SuffleSpawnGates()
    {
        for(int i=0;i< spawnGates.Length;i++)
        {
            var temp = spawnGates[i];
            int rnd = Random.Range(0, spawnGates.Length);
            spawnGates[i] = spawnGates[rnd];
            spawnGates[rnd] = temp;
        }
    }
}
