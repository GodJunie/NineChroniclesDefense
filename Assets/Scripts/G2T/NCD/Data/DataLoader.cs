// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
// Etc
using Newtonsoft.Json.Linq;
// Editor
using Sirenix.OdinInspector;
using Newtonsoft.Json;

namespace G2T.NCD.Data {
    public class DataLoader : SingletonBehaviour<DataLoader> {
        // Dictionary 나 List 같은 객체는 어떻게 관리할 것인가
        // Public 으로 빼버리면 생성이랑 할당은 될텐데.....
        
        public Dictionary<string, MonsterData> Monsters { get; private set; }
        public List<object> Stages { get; private set; }
        public Dictionary<string, ItemData> Items { get; private set; }

        //private bool isDirty;

        
        [Button]
        private void Test() {
            var monster = new MonsterData() { Id = "id", Level = 0 };
            Debug.Log(JsonConvert.SerializeObject(monster));
        }

        
        #region Menu
        public void LoadData() {
           
        }

        public void SaveData() {

        }
        #endregion

        #region MonsterData
        [Button]
        public void LoadMonsterData() {
            string json = PlayerPrefs.GetString("monsters", "");

            if(json == "") {
                this.Monsters = new Dictionary<string, MonsterData>();
                return;
            } else {
                this.Monsters = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(json);
            }

            foreach(var pair in this.Monsters) {
                Debug.Log(string.Format("key: {0}, value: (id: {1})", pair.Key, pair.Value.Id));
            }
        }

        [Button]
        public void SaveMonsterData() {
            string json = JsonConvert.SerializeObject(this.Monsters);
            Debug.Log(json);
            PlayerPrefs.SetString("monsters", json);
        }

        [Button]
        public void AddMonster(string id) {
            if(this.Monsters == null) this.Monsters = new Dictionary<string, MonsterData>();
            var monster = new MonsterData() { Id = id };
            this.Monsters.Add(NewGuid(), monster);
            //this.isDirty = true;
        }
        #endregion

        private string NewGuid() {
            return Guid.NewGuid().ToString();
        }
    }
}