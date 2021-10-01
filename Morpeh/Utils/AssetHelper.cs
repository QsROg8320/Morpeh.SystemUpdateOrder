using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Morpeh.Utils
{
    public class AssetHelper
    {

        public static T CreateAsset<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(asset, Path.Combine(path, $"{nameof(T)}.asset"));
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
            return asset;
        }
        public static UnityEngine.Object CreateAsset(Type t, string path)
        {
            UnityEngine.Object asset = ScriptableObject.CreateInstance(t);

            AssetDatabase.CreateAsset(asset, Path.Combine(path, $"{t.Name}.asset"));
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
            return asset;
        }
        public static List<Type> FindClasses<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(assembly => assembly.GetTypes())
                     .Where(type => type.IsSubclassOf(typeof(T)))
                     .ToList();
        }
        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }
    }
}
