using UnityEngine;
using UnityEditor;

public class ObjectDeplicate
{
    [MenuItem("Edit/DummyDeplicate %d", false, -1)]
    static void CreateEmptyObjec2t()
    {
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if (path == string.Empty)
            {
                var gameObject = obj as GameObject;
                var copy = GameObject.Instantiate(gameObject, gameObject.transform.parent);
                copy.name = obj.name;
                copy.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex());
                Undo.RegisterCreatedObjectUndo(copy, "deplicate");
            }
            else
            {
                var newPath = AssetDatabase.GenerateUniqueAssetPath(path);
                AssetDatabase.CopyAsset(path, newPath);
            }
        }
    }
}