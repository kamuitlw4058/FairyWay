using UnityEditor;
using Object = UnityEngine.Object;

namespace FairyWay.Editor
{
    public class AssetDatabaseUtils
    {

        public static T LoadAssetAtPath<T>(string path) where T : Object
        {
#if UNITY_5
            return AssetDatabase.LoadAssetAtPath<T>(path);
#else
            return (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
#endif
        }

    }
}