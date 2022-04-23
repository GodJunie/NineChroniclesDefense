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

    [Serializable]
    public class EnemyInfo : ExcelData {
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

        // 오브젝트
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("프리팹")]
        [FilePath(AbsolutePath = false)]
        [SerializeField]
        private string prefabPath;
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("아이콘")]
        [FilePath(AbsolutePath = false)]
        [SerializeField]
        private string iconPath;

        public int Id { get => id; }
        public string Name { get => name; }
        public string Description { get => description; }
        public string IconPath { get => iconPath; }
        public string PrefabPath { get => prefabPath; }

        public override string[] GetProperties() {
            var properties = new string[] {
                "id",
                "name",
                "description",
                "prefabPath",
                "iconPath",
            };
            return properties;
        }

        public override void InitFromJObject(JObject jObject) {
            this.id = jObject.Value<int>("id");
            this.name = jObject.Value<string>("name");
            this.description = jObject.Value<string>("description");
            this.iconPath = jObject.Value<string>("iconPath");
            this.prefabPath = jObject.Value<string>("prefabPath");
        }

        public void Temp() {
            var p = "Assets/Resources/Prefabs/Game/Enemies/";
            if(!this.prefabPath.StartsWith(p))
                this.prefabPath = string.Format(p + prefabPath + ".prefab");
            var p2 = "Assets/Resources/UI/Icons/Character/";
            if(!this.iconPath.StartsWith(p2))
                this.iconPath = string.Format(p2 + iconPath + ".png");
        }
    }
}
