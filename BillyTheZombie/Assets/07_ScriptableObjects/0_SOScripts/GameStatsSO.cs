using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStats", menuName = "ScriptableObject/Stats/GameStats", order = 1)]
public class GameStatsSO : ScriptableObject
{
    public float mutagenPoints;
    public int currentWaveCount;
    public int maxReachedWaveCount;

    public int indexOfWaveToSpawn;

    public void ResetGameStats()
    {
        mutagenPoints = 0.0f;

        currentWaveCount = 0;
        maxReachedWaveCount = 0;
        
        indexOfWaveToSpawn = 0;
    }
}
