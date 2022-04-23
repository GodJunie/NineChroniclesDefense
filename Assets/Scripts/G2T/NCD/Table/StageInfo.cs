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
    public class StageInfo : ExcelData {
        #region Struct
        [Serializable]
        public struct Reward {
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

            public Reward(int id, int amount) {
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
        [BoxGroup("group/정보")]
        [LabelText("보상 아이템")]
        [LabelWidth(80f)]
        [SerializeField]
        private List<Reward> rewards;

        // 오브젝트
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("타임라인")]
        [SerializeField]
        private string timelinePath;
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("배경 이미지")]
        [FilePath(AbsolutePath = false)]
        [SerializeField]
        private string backgroundPath;
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("인게임 배경 프리팹")]
        [SerializeField]
        private string backgroundPrefabPath;

        public override string[] GetProperties() {
            var properties = new string[] {
                "id",
                "name",
                "description",
                "timelinePath",
                "backgroundPath",
                "rewardIds",
                "rewardAmounts",
            };
            return properties;
        }

        public override void InitFromJObject(JObject jObject) {
            this.id = jObject.Value<int>("id");
            this.name = jObject.Value<string>("name");
            this.description = jObject.Value<string>("description");
            this.timelinePath = jObject.Value<string>("timelinePath");
            this.backgroundPath = jObject.Value<string>("backgroundPath");

            var rewardIds = jObject["rewardIds"].Values<int>().ToList();
            var rewardAmounts = jObject["rewardAmounts"].Values<int>().ToList();

            this.rewards = new List<Reward>();
            for(int i = 0; i < Mathf.Min(rewardIds.Count, rewardAmounts.Count); i++) {
                rewards.Add(new Reward(rewardIds[i], rewardAmounts[i]));
            }
        }

        #region Getter
        public int Id { get => id; }
        public string Name { get => name; }
        public string Description { get => description; }
        public string TimelinePath { get => timelinePath; }
        public string BackgroundPath { get => backgroundPath; }
        public string BackgroundPrefabPath { get => backgroundPrefabPath; }
        public List<Reward> Rewards { get => rewards; }
        #endregion
    }
}
