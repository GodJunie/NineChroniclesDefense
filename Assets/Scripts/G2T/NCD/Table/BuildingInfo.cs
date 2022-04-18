using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace G2T.NCD.Table {
    [Serializable]
    public class BuildingInfo : ExcelData {
        #region Struct
        [Serializable]
        public struct ConstructItem {
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

            public ConstructItem(int id, int amount) {
                this.id = id;
                this.amount = amount;
            }

            public int Id { get => id; }
            public int Amount { get => amount; }
        }
        #endregion

        [HorizontalGroup("group")]
        // 정보
        [BoxGroup("group/정보")]
        [LabelText("아이디")]
        [LabelWidth(80f)]
        [SerializeField]
        private int id;
        [BoxGroup("group/정보")]
        [LabelText("이름")]
        [LabelWidth(80f)]
        [SerializeField]
        private string name;
        [BoxGroup("group/정보")]
        [LabelText("설명")]
        [LabelWidth(80f)]
        [SerializeField]
        private string description;

        [BoxGroup("group/게임")]
        [LabelText("건설 아이템")]
        [LabelWidth(80f)]
        [SerializeField]
        private List<ConstructItem> constructItems;
        [BoxGroup("group/게임")]
        [LabelText("건물 범위")]
        [LabelWidth(80f)]
        [SerializeField]
        private float range;

        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("스탯")]
        [SerializeField]
        private string statusPath;
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("프리팹")]
        [SerializeField]
        private string prefabPath;

        public override string[] GetProperties() {
            var properties = new string[] {
                "id",
                "name",
                "description",
                "constructItemIds",
                "constructItemAmounts",
                "range",
                "statusPath",
                "prefabPath",
            };
            return properties;
        }

        public override void InitFromJObject(JObject jObject) {
            this.id = jObject.Value<int>("id");
            this.name = jObject.Value<string>("name");
            this.description = jObject.Value<string>("description");

            var constructItemIds = jObject["constructItemIds"].Values<int>().ToList();
            var constructItemAmounts = jObject["constructItemAmounts"].Values<int>().ToList();

            this.constructItems = new List<ConstructItem>();
            for(int i = 0; i < Mathf.Min(constructItemIds.Count, constructItemAmounts.Count); i++) {
                constructItems.Add(new ConstructItem(constructItemIds[i], constructItemAmounts[i]));
            }

            this.range = jObject.Value<float>("range");

            this.statusPath = jObject.Value<string>("statusPath");
            this.prefabPath = jObject.Value<string>("prefabPath");
        }

        #region Getter
        public int Id { get => id; }
        public string Name { get => name; }
        public string Description { get => description; }
        public List<ConstructItem> ConstructItems { get => constructItems; }
        public float Range { get => range; }
        public string StatusPath { get => statusPath; }
        public string PrefabPath { get => prefabPath; }
        #endregion
    }
}