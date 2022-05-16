using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonPrompt", menuName = "ScriptableObject/UI/UIButtons", order = 4)]
public class UIButtonsSO : ScriptableObject
{
    [Tooltip("UI Sprites for Keyboard:\n[0]-Right\n[1]-Left\n[2]-Head")]
    public Sprite[] keyboardSprites = new Sprite[3];
    [Tooltip("UI Sprites for Gamepad:\n[0]-Right\n[1]-Left\n[2]-Head")]
    public Sprite[] gamepadSprites = new Sprite[3];
}
