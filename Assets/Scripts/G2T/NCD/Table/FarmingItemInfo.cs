// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
// Editor
using Sirenix.OdinInspector;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace G2T.NCD.Table {
    [Serializable]
    public class FarmingItemInfo : ExcelData {
        #region Structs
        [Serializable]
        public struct DropItem {
            [SerializeField]
            [HorizontalGroup("group")]
            [BoxGroup("group/���̵�")]
            [HideLabel]
            private int id;

            [SerializeField]
            [HorizontalGroup("group")]
            [BoxGroup("group/����")]
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

        [SerializeField]
        [LabelText("���̵�")]
        private int id;

        [SerializeField]
        [LabelText("��� ������")]
        private List<DropItem> dropItems;

        [SerializeField]
        [LabelText("������ ���")]
        [FilePath(AbsolutePath = false)]
        private string prefabPath;

        public int Id { get => id; }
        public List<DropItem> DropItems;
        public string PrefabPath { get => prefabPath; }

        public override string[] GetProperties() {
            var properties = new string[] {
                "id",
                "dropItemIds",
                "dropItemAmounts",
                "prefabPath",
            };
            return properties;
        }

        public override void InitFromJObject(JObject jObject) {
            this.id = jObject.Value<int>("id");
            this.prefabPath = jObject.Value<string>("prefabPath");

            var dropItemIds = jObject["dropItemIds"].Values<int>().ToList();
            var dropItemAmounts = jObject["dropItemAmounts"].Values<int>().ToList();

            this.dropItems = new List<DropItem>();

            for(int i = 0; i < Mathf.Min(dropItemIds.Count, dropItemAmounts.Count); i++) {
                dropItems.Add(new DropItem(dropItemIds[i], dropItemAmounts[i]));
            }
        }
    }
}