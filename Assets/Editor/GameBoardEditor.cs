using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameBoard))]
public class GameBoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameBoard board = (GameBoard)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Preveiw"))
        {
            board.Preveiw();
        }
        if (GUILayout.Button("Clear"))
        {
            board.ClearPreveiw();
        }

        GUILayout.EndHorizontal();
    }
}
