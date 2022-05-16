using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonPrompt", menuName = "ScriptableObject/UI/UIActions", order = 4)]
public class UIActionsSO : ScriptableObject
{
    public Sprite[] actionSprites = new Sprite[4];
}
