using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyStatsProfile",
    menuName = "ScriptableObject/Stats/EnemyStats", order = 3)]
public class EnemyStatsSO : ScriptableObject
{
    public float _mutagenValue = 0.0f;

    public float _health = 0.0f;
    public float _speed = 0.0f;
    public float _damage = 0.0f;
    public float _attackFrequency = 0.0f;
}
