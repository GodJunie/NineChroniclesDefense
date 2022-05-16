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
    public class MonsterInfo : ExcelData {
        #region Structs
        [Serializable]
        public struct EvolutionMaterial {
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

            public EvolutionMaterial(int id, int amount) {
                this.id = id;
                this.amount = amount;
            }

            public int Id { get => id; }
            public int Amount { get => amount; }
        }

        [Serializable]
        public struct CatchMaterial {
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

            public CatchMaterial(int id, int amount) {
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

        // 진화
        [BoxGroup("group/진화")]
        [LabelWidth(80f)]
        [LabelText("진화 재료")]
        [SerializeField]
        private List<EvolutionMaterial> evolutionMaterials;
        [BoxGroup("group/진화")]
        [LabelWidth(80f)]
        [LabelText("진화 결과")]
        [SerializeField]
        private int evolutionResult;

        // 포획
        [BoxGroup("group/포획")]
        [LabelText("포획 재료")]
        [SerializeField]
        private List<CatchMaterial> catchMaterials;

        // 오브젝트
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("프리팹")]
        [FilePath(AbsolutePath = false)]
        [SerializeField]
        private string prefabPath;
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("스탯")]
        [FilePath(AbsolutePath = false)]
        [SerializeField]
        private string statusPath;
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("아이콘")]
        [FilePath(AbsolutePath = false)]
        [SerializeField]
        private string iconPath;


        public override string[] GetProperties() {
            var properties = new string[] {
                "id",
                "name",
                "description",
                "evolutionMaterialIds",
                "evolutionMaterialAmounts",
                "evolutionResult",
                "catchMaterialIds",
                "catchMaterialAmounts",
                "prefabPath",
                "statusPath",
                "iconPath"
            };
            return properties;
        }

        public override void InitFromJObject(JObject jObject) {
            this.id = jObject.Value<int>("id");
            this.name = jObject.Value<string>("name");
            this.description = jObject.Value<string>("description");
            this.evolutionResult = jObject.Value<int>("evolutionResult");
            this.prefabPath = jObject.Value<string>("prefabPath");
            this.statusPath = jObject.Value<string>("statusPath");
            this.iconPath = jObject.Value<string>("iconPath");


            if(jObject["evolutionMaterialIds"] != null && jObject["evolutionMaterialAmounts"] != null) {
                var evolutionMaterialIds = jObject["evolutionMaterialIds"].Values<int>().ToList();
                var evolutionMaterialAmounts = jObject["evolutionMaterialAmounts"].Values<int>().ToList();

                this.evolutionMaterials = new List<EvolutionMaterial>();
                for(int i = 0; i < Mathf.Min(evolutionMaterialIds.Count, evolutionMaterialAmounts.Count); i++) {
                    evolutionMaterials.Add(new EvolutionMaterial(evolutionMaterialIds[i], evolutionMaterialAmounts[i]));
                }
            }


            if(jObject["catchMaterialIds"] != null && jObject["catchMaterialAmounts"] != null) {
                var catchMaterialIds = jObject["catchMaterialIds"].Values<int>().ToList();
                var catchMaterialAmounts = jObject["catchMaterialAmounts"].Values<int>().ToList();

                this.catchMaterials = new List<CatchMaterial>();
                for(int i = 0; i < Mathf.Min(catchMaterialIds.Count, catchMaterialAmounts.Count); i++) {
                    catchMaterials.Add(new CatchMaterial(catchMaterialIds[i], catchMaterialAmounts[i]));
                }
            }
        }

        #region Getter
        public int Id { get => id; }
        public string Name { get => name; }
        public string Description { get => description; }
        public List<EvolutionMaterial> EvolutionMaterials { get => evolutionMaterials; }
        public int EvolutionResult { get => evolutionResult; }
        public List<CatchMaterial> CatchMaterials { get => catchMaterials; }
        public string PrefabPath { get => prefabPath; }
        public string IconPath { get => iconPath; }
        public string StatusPath { get => statusPath; }
        #endregion

        public void Temp(string prefabFolderPath, string iconFolderPath, string statusFolderPath) {
            if(!this.prefabPath.StartsWith(prefabFolderPath))
                this.prefabPath = string.Format(prefabFolderPath + prefabPath + ".prefab");
            if(!this.iconPath.StartsWith(iconFolderPath))
                this.iconPath = string.Format(iconFolderPath + iconPath + ".png");
            if(!this.statusPath.StartsWith(statusFolderPath))
                this.statusPath = string.Format(statusFolderPath + statusPath + ".asset");
        }
    }
}
