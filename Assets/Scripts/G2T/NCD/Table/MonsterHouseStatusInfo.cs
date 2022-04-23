using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace G2T.NCD.Table {
    [Serializable]
    public class MonsterHouseStatusInfo : BuildingStatusInfo {
        [LabelText("몬스터 수")]
        [SerializeField]
        private int monsterAmount;

        public override string[] GetProperties() {
            var properties = base.GetProperties();
            return properties.Concat(new string[] {
                "monsterAmount"
            }).ToArray();
        }

        public override void InitFromJObject(JObject jObject) {
            base.InitFromJObject(jObject);

            this.monsterAmount = jObject.Value<int>("monsterAmount");
        }

        public int MonsterAmount { get => monsterAmount; }
    }
}