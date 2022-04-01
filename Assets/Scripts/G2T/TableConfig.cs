using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

namespace G2T {
    public abstract class TableConfig<T> : ScriptableObject where T : TableConfig<T>, new() {
#if UNITY_EDITOR
        private bool isLoadedFromAsset = true;
        public bool IsLoadedFromAsset { get { return isLoadedFromAsset; } }

        private static T _instance = null;
        public static T Instance {
            get {
                if(_instance == null) {
                    var guid = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).Name)).FirstOrDefault();
                    Debug.Log(guid);
                    if(guid == default) {
                        _instance = new T();
                        _instance.isLoadedFromAsset = false;
                    } else {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                        Debug.Log(path);
                        _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                    }
                }
                return _instance;
            }
        }

        [InfoBox("최초 한 번만 생성합니다. 저장 이후엔 자동으로 생성된 테이블을 불러옵니다.")]
        [Button("생성하기", ButtonHeight = 50), HideIf("isLoadedFromAsset")]
        private void CreateTable() {
            string path = UnityEditor.EditorUtility.SaveFilePanel("생성하기", "Assets", "", "asset");
            if(!path.StartsWith(Application.dataPath))
                return;
            path = path.Replace(Application.dataPath, "Assets");
            Debug.Log(path);
            try {
                _instance.isLoadedFromAsset = true;
                UnityEditor.AssetDatabase.CreateAsset(_instance, path);
            }
            catch(Exception e) {
                Debug.LogError(e.Message);
                return;
            }
        }
#endif
    }
}