using UnityEngine;
using UnityEditor;
using Boo.Lang;
using Mirror;

[CustomEditor(typeof(BoardLayout))]
public class BoardLayoutEditor : Editor
{

    bool isInit = false;
    int initXX;
    int inityy;

    private bool[][] data;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BoardLayout layout =  (BoardLayout)target;

        if(isInit)
        {
            if (initXX != (int)layout.Rows || inityy != (int)layout.Columns)
                isInit = false;
        }

        if(!isInit)
        {
            isInit = true;
            initXX = (int)layout.Columns;
            inityy = (int)layout.Rows;
            data = new bool[inityy][];

            for (int yy = 0; yy < (int)layout.Rows; yy++)
            {
                bool[] xList = new bool[initXX];
                for(int xx = 0; xx < (int)layout.Columns; xx++)
                {
                    xList[xx]= false;
                }
                data[yy] = xList;
            }
            layout.SetRowSpecials(data);
        }

        GUILayout.BeginVertical();
        for(int yy = 0 ; yy < (int)layout.Rows; yy++)
        {

            GUILayout.BeginHorizontal();
            for(int xx = 0; xx < (int)layout.Columns; xx++)
            {
                Texture texture;
                bool yes = data[yy][xx];
                if (yes) texture = layout.OnTexture;
                else texture = layout.baseTexture;

                if(GUILayout.Button(texture))
                {
                    data[yy][xx] = !data[yy][xx];
                    layout.SetRowSpecials(data);
                }
            }
            GUILayout.EndHorizontal();

        }
        GUILayout.EndVertical();
    }
}
