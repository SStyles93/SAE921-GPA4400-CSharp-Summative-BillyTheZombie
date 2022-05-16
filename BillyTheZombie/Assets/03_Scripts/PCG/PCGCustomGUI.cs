using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapPCG))]
public class PCGCustomGUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapPCG map = (MapPCG)target;

        if (GUILayout.Button("Generate"))
        {
            map.Generate();
        }
        if (GUILayout.Button("Delete"))
        {
            map.DeleteTiles();
        }
    }
}
