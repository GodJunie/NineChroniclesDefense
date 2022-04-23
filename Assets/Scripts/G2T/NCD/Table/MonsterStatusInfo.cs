// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.Table {
    using Game;
    using Newtonsoft.Json.Linq;
    using System.Linq;

    [Serializable]
    public class MonsterStatusInfo : ExcelData {
        #region Struct
        [Serializable]
        public struct LevelUpItem {
            [SerializeField]
            [HorizontalGroup("group")]
            [BoxGroup("group/아이디")]
            [HideLabel]
            private int id;

            [SerializeField]
            [HorizontalGroup("group")]
            [BoxGroup("group/개수")]
            [HideLabel]
            private int amount;

            public LevelUpItem(int id, int amount) {
                this.id = id;
                this.amount = amount;
            }

            public int Id { get => id; }
            public int Amount { get => amount; }
        }
        #endregion

        [LabelText("스탯")]
        [SerializeField]
        private Status status;
        [LabelText("레벨업 아이템")]
        [SerializeField]
        private List<LevelUpItem> levelUpItems;

        public override string[] GetProperties() {
            var properties = new string[] {
                "levelUpItemIds",
                "levelUpItemAmounts",
            };
            return properties.Concat(Status.Properties).ToArray();
        }

        public override void InitFromJObject(JObject jObject) {
            var levelUpItemIds = jObject["levelUpItemIds"].Values<int>().ToList();
            var levelUpItemAmounts = jObject["levelUpItemAmounts"].Values<int>().ToList();

            this.levelUpItems = new List<LevelUpItem>();
            for(int i = 0; i < Mathf.Min(levelUpItemIds.Count, levelUpItemAmounts.Count); i++) {
                levelUpItems.Add(new LevelUpItem(levelUpItemIds[i], levelUpItemAmounts[i]));
            }

            this.status = Status.FromJObject(jObject);
        }

        public Status Status { get => status; }
        public List<LevelUpItem> LevelUpItems { get => levelUpItems; }
    }
}