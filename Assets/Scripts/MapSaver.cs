using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapSaver : MonoBehaviour
{
    public GameObject tileMap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

// Only allow in editor
#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            CreatePrefab(tileMap);
        }
    }

    // https://docs.unity3d.com/ScriptReference/PrefabUtility.html
    static void CreatePrefab(GameObject map)
    {
        string localPath = "Assets/Prefab/" + map.name + ".prefab";

        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        PrefabUtility.SaveAsPrefabAsset(map, localPath);
    }
#endif
}
