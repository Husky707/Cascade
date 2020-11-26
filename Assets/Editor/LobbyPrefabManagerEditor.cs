using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LobbyPrefabManager))]
public class LobbyPrefabManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LobbyPrefabManager prefabs = (LobbyPrefabManager)target;


    }

}
