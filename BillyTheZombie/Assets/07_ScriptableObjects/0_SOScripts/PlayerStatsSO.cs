using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats",
    menuName = "ScriptableObject/Stats/PlayerStats", order = 2)]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Final values")]
    public float currentHealth = 100.0f;
    public float maxHealth = 100.0f;
    
    //Upgrade values
    [Header("Modifications")]
    [Space(20)]
    public float pushPowerPercentage = 0.0f;
    public float armDamagePercentage = 0.0f;
    public float healthPercentage = 0.0f;
    public float speedPercentage = 0.0f;
    //ARMTYPE
    [Header("ArmTypes")]
    public int rightArmType = 0;
    public int leftArmType = 0;
    
    [Header("Basic values")]
    [Space(50)]
    public float basicHealth = 100.0f;
    public float basicArmDamage = 10.0f;
    public float basicSpeed = 4.0f;
    public float basicPushPower = 10.0f;

    public void ResetPlayerStats()
    {
        currentHealth = 100.0f;
        maxHealth = 100.0f;

        pushPowerPercentage = 0.0f;
        armDamagePercentage = 0.0f;
        healthPercentage = 0.0f;
        speedPercentage = 0.0f;

        rightArmType = 0;
        leftArmType = 0;
}
}
