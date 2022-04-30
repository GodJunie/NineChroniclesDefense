using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;

namespace G2T.NCD.Table {
    [Serializable]
    public class BunkerStatusInfo : BuildingStatusInfo {
        [LabelText("몬스터 마리 수")]
        [SerializeField]
        private int entryCount;

        public override string[] GetProperties() {
            return base.GetProperties().Concat(new string[] {
                "entryCount"
            }).ToArray();
        }

        public override void InitFromJObject(JObject jObject) {
            base.InitFromJObject(jObject);
            this.entryCount = jObject.Value<int>("entryCount");
        }

        public int EntryCount { get => entryCount; }
    }
}