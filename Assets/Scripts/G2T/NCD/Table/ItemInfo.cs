// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
// Editor
using Sirenix.OdinInspector;
using Newtonsoft.Json.Linq;

namespace G2T.NCD.Table {
    [System.Serializable]
    public class ItemInfo : ExcelData {
        [SerializeField]
        [HorizontalGroup("group")]
        [BoxGroup("group/정보")]
        [HorizontalGroup("group/정보/group")]
        [BoxGroup("group/정보/group/아이디")]
        [HideLabel]
        private int id;

        [SerializeField]
        [BoxGroup("group/정보/group/이름")]
        [HideLabel]
        private string name;

        [SerializeField]
        [BoxGroup("group/정보/group/설명")]
        [HideLabel]
        private string description;

        [HorizontalGroup("group")]
        [BoxGroup("group/연결")]
        [BoxGroup("group/연결/아이콘 경로")]
        [HideLabel]
        [SerializeField]
        [FilePath(AbsolutePath = false)]
        private string iconPath;

        public string IconPath { get => iconPath; }
        public int Id { get => id; }
        public string Name { get => name; }
        public string Description { get => description; }

        public override string[] GetProperties() {
            var properties = new string[] {
                "id",
                "name",
                "description",
                "iconPath",
            };
            return properties;
        }

        public override void InitFromJObject(JObject jObject) {
            this.id = jObject.Value<int>("id");
            this.name = jObject.Value<string>("name");
            this.description = jObject.Value<string>("description");
            this.iconPath = jObject.Value<string>("iconPath");
        }
    }
}
