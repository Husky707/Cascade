using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataTester : MonoBehaviour
{
    [SerializeField] bool runOnInit = false;
    [SerializeField] bool runTest = false;
    private bool test = true;

    [SerializeField] GameRule rules = null;

    //[SerializeField] GameSettings testGameSettings = null;
    //[SerializeField] BoardLayout testBoardLayout = null;

    [SerializeField] CascadeGame testGame = null;

    private void RunTest()
    {
        if (!testGame)
            return;

        Debug.Log("Beginning GameData test creation");
        GameData test = new GameData(testGame.Board.Layout, testGame.GameSettings);



        if (test == null)
            Debug.Log("Test failed. GameData is null");
        else
        {
            Debug.Log("Created GameData succesfully");
        }
    }

    private void OnValidate()
    {
        if (test == runTest)
        {
            test = !runTest;
            RunTest();
        }
    }
}
