// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// UnityEngine
using UnityEngine;
// Editor
using Sirenix.OdinInspector;
// Etc
using Newtonsoft.Json.Linq;

namespace G2T.NCD.Table {
    [Serializable]
    public class RecipeInfo : ExcelData {
        #region Structs
        [Serializable]
        public struct Material {
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

            public Material(int id, int amount) {
                this.id = id;
                this.amount = amount;
            }

            public int Id { get => id; }
            public int Amount { get => amount; }
        }
        #endregion

        [SerializeField]
        [LabelText("아이디")]
        private int id;

        private List<Material> materials;
        private int resultId;
        private int resultAmount;

        public int Id { get => id; }
        public List<Material> Materials;

        public int ResultId { get => resultId; }
        public int ResultAmount { get => resultAmount; }

        public override string[] GetProperties() {
            var properties = new string[] {
                "id",
                "materialIds",
                "materialAmounts",
                "resultId",
                "resultAmount",
            };
            return properties;
        }

        public override void InitFromJObject(JObject jObject) {
            this.id = jObject.Value<int>("id");
            this.resultId = jObject.Value<int>("resultId");
            this.resultAmount = jObject.Value<int>("resultAmount");

            var materialIds = jObject["materialIds"].Values<int>().ToList();
            var materialAmounts = jObject["materialAmounts"].Values<int>().ToList();

            this.materials = new List<Material>();
            for(int i = 0; i < Mathf.Min(materialIds.Count, materialAmounts.Count); i++) {
                materials.Add(new Material(materialIds[i], materialAmounts[i]));
            }
        }
    }
}