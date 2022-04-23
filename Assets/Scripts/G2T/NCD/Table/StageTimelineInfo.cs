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
    public class StageTimelineInfo : ExcelData {
        #region Struct
        [Serializable]
        public struct MonsterSpawnInfo {
            [HorizontalGroup("group")]
            [BoxGroup("group/아이디")]
            [HideLabel]
            [SerializeField]
            private int id;
            [HorizontalGroup("group")]
            [BoxGroup("group/확률")]
            [HideLabel]
            [SerializeField]
            private float prob;
           
            public MonsterSpawnInfo(int id, float prob) {
                this.id = id;
                this.prob = prob;
            }

            public int Id { get => id; }
            public float Prob { get => prob; }
        }
        #endregion

        [FoldoutGroup("@GroupName")]
        [BoxGroup("@GroupName/시간 정보")]
        [HorizontalGroup("@GroupName/시간 정보/group", .5f)]
        [BoxGroup("@GroupName/시간 정보/group/시간대")]
        [HideLabel]
        [SerializeField]
        private DayTimePart timePart;
        [BoxGroup("@GroupName/시간 정보/group/시간 (초)")]
        [HideLabel]
        [SerializeField]
        private float time;
        [FoldoutGroup("@GroupName")]
        [SerializeField]
        [LabelText("왼쪽 적군")]
        private List<int> leftEnemies;
        [FoldoutGroup("@GroupName")]
        [LabelText("오른쪽 적군")]
        [SerializeField]
        private List<int> rightEnemies;
        [FoldoutGroup("@GroupName")]
        [SerializeField]
        [LabelText("몬스터 정보")]
        private List<MonsterSpawnInfo> monsters;
        [FoldoutGroup("@GroupName")]
        [SerializeField]
        [LabelText("몬스터 마리수")]
        private int monsterAmount;

        public DayTimePart TimePart { get => timePart; }
        public float Time { get => time; }
        public List<int> LeftEnemies { get => leftEnemies; }
        public List<int> RightEnemies { get => rightEnemies; }
        public List<MonsterSpawnInfo> Monsters { get => monsters; }
        public int MonsterAmount { get => monsterAmount; }

#if UNITY_EDITOR
        private string GroupName { get => string.Format("{0}/{1}초/왼쪽 적 {2}명/오른쪽 적 {3}명/몬스터 {4}마리", this.timePart, this.time, this.leftEnemies.Count, this.rightEnemies.Count, this.monsterAmount); }
#endif

        public override string[] GetProperties() {
            var properties = new string[] {
                "timePart",
                "time",
                "waveLeftIds",
                "waveRightIds",
                "monsterIds",
                "monsterProbs",
                "monsterAmount"
            };
            return properties;
        }

        public override void InitFromJObject(JObject jObject) {
            this.timePart = (DayTimePart)Enum.Parse(typeof(DayTimePart), jObject.Value<string>("timePart"));
            this.time = jObject.Value<float>("time");

            Debug.Log(jObject["waveLeftIds"]);

            if(jObject["waveLeftIds"] != null)
                this.leftEnemies = jObject["waveLeftIds"].Values<int>().ToList();
            else
                this.leftEnemies = new List<int>();
            if(jObject["waveRightIds"] != null)
                this.rightEnemies = jObject["waveRightIds"].Values<int>().ToList();
            else
                this.rightEnemies = new List<int>();

            this.monsters = new List<MonsterSpawnInfo>();

            if(jObject["monsterIds"] != null && jObject["monsterProbs"] != null) {
                var monsterIds = jObject["monsterIds"].Values<int>().ToList();
                var monsterProbs = jObject["monsterProbs"].Values<float>().ToList();

                for(int i = 0; i < Mathf.Min(monsterIds.Count, monsterProbs.Count); i++) {
                    monsters.Add(new MonsterSpawnInfo(monsterIds[i], monsterProbs[i]));
                }
            }

            this.monsterAmount = jObject.Value<int>("monsterAmount");
        }
    }
}