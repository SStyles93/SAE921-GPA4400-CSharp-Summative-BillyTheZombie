using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveProfile", menuName = "ScriptableObject/WaveProfile", order = 3)]
public class WavesSO : ScriptableObject
{
    public bool isSubWaves = false;

    public int NumberOfEnemies = 0; 
    public int EnemyIndex = 0;
    public int PositionIndex = 0;

}
