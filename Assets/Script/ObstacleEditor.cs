using UnityEditor;
using UnityEngine;

public class ObstacleEditor : EditorWindow
{
    private ObstacleData obstacleData;

    [MenuItem("Tools/Obstacle Editor")]
    public static void OpenEditor()
    {
        GetWindow<ObstacleEditor>("Obstacle Editor"); // Opens custom window
    }

    private void OnGUI()
    {
        if (obstacleData == null)
        {
            GUILayout.Label("Choose Obstacle Data:"); // User selects obstacle file
            obstacleData = (ObstacleData)EditorGUILayout.ObjectField(obstacleData, typeof(ObstacleData), false);
            return; // Stops if no file choosen
        }

        GUILayout.Label("Obstacle Grid (10x10):", EditorStyles.boldLabel); // Grid settings

        for (int row = 0; row < 10; row++)
        {
            GUILayout.BeginHorizontal();
            for (int col = 0; col < 10; col++)
            {
                int index = row * 10 + col;
                obstacleData.blockedCells[index] = GUILayout.Toggle(obstacleData.blockedCells[index], ""); // Toggle blocked cells
            }
            GUILayout.EndHorizontal(); // Finish row of toggles
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(obstacleData); // Saves file changes
            AssetDatabase.SaveAssets(); // Updates asset database
        }
    }
}