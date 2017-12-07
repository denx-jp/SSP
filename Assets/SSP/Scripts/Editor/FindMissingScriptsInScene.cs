using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindMissingScriptsInScene : EditorWindow
{
    public bool LimitResultCount = false;
    public int MaxResults = 1;

    public List<GameObject> Results;
    private Vector2 ResultScrollPos;

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            {
                if (GUILayout.Button("Find Missing Scripts In The Current Scene"))
                    Find();

                if (LimitResultCount = EditorGUILayout.Foldout(LimitResultCount, "Limit Result Count (Limit:"
                        + (LimitResultCount ? MaxResults.ToString() : "None") + ")"))
                    MaxResults = EditorGUILayout.IntField("Result Max:", MaxResults);
            }

            EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);
            {
                if (Results != null)
                {
                    EditorGUILayout.LabelField("Scene objects found:", Results.Count.ToString(), EditorStyles.boldLabel);

                    ResultScrollPos = EditorGUILayout.BeginScrollView(ResultScrollPos);
                    {
                        if (LimitResultCount)
                        {
                            for (int i = 0; i < Mathf.Min(MaxResults, Results.Count); i++)
                                EditorGUILayout.ObjectField(Results[i], typeof(GameObject), false);
                        }
                        else
                        {
                            foreach (GameObject go in Results)
                                EditorGUILayout.ObjectField(go, typeof(GameObject), false);
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    void Find()
    {
        Results = new List<GameObject>();

        GameObject[] objs = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject go in objs)
        {
            if (go.transform.parent == null)
                FindMissingScriptsRecursively(go);
        }
    }
    void FindMissingScriptsRecursively(GameObject go)
    {
        Component[] components = go.GetComponents<Component>();

        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                if (!Results.Contains(go))
                    Results.Add(go);
            }
        }

        // Now recurse through each child GO (if there are any):  
        foreach (Transform child in go.transform)
        {
            FindMissingScriptsRecursively(child.gameObject);
        }
    }

    [MenuItem("Tools/Find Missing Scripts In Scene...")]
    static void Init()
    {
        FindMissingScriptsInScene window = EditorWindow.GetWindow<FindMissingScriptsInScene>("Find Missing Scripts");
        window.ShowPopup();
        //window.ShowAuxWindow();  
    }
}
