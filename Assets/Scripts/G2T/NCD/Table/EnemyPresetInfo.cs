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
    public class EnemyPresetInfo : ExcelData {
        #region Structs
        [Serializable]
        public struct DropItem {
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

            public DropItem(int id, int amount) {
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
        [LabelText("적 정보 아이디")]
        [LabelWidth(80f)]
        [SerializeField]
        private int enemyId;

        // 전투
        [BoxGroup("group/전투")]
        [HorizontalGroup("group/전투/group", .5f)]
        [HideLabel]
        [SerializeField]
        private Status status;
        [HorizontalGroup("group/전투/group", .5f)]
        [LabelText("드랍 아이템")]
        [SerializeField]
        private List<DropItem> dropItems;


        public override string[] GetProperties() {
            var properties = new string[] {
                "id",
                "enemyId", 
                "dropItemIds",
                "dropItemAmounts",
            };
            return properties.Concat(Status.Properties).ToArray();
        }

        public override void InitFromJObject(JObject jObject) {
            this.id = jObject.Value<int>("id");
            this.enemyId = jObject.Value<int>("enemyId");

            var dropItemIds = jObject["dropItemIds"].Values<int>().ToList();
            var dropItemAmounts = jObject["dropItemAmounts"].Values<int>().ToList();

            this.dropItems = new List<DropItem>();
            for(int i = 0; i < Mathf.Min(dropItemIds.Count, dropItemAmounts.Count); i++) {
                dropItems.Add(new DropItem(dropItemIds[i], dropItemAmounts[i]));
            }

            this.status = Status.FromJObject(jObject);
        }

        #region Getter
        public int Id { get => id; }
        public int EnemyId { get => enemyId; }
        public Status Status { get => status; }
        public List<DropItem> DropItems { get => dropItems; }
        #endregion
    }
}
