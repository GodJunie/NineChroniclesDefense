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

    [Serializable]
    public class MonsterInfo {
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
        [SerializeField]
        private Monster prefab;
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("스탯")]
        [SerializeField]
        private MonsterStatusInfo status;
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("아이콘")]
        [SerializeField]
        private Sprite icon;


        public MonsterInfo(int id, string name, string description, List<CatchMaterial> catchMaterials, List<EvolutionMaterial> evolutionMaterials, int evolutionResult) {
            this.id = id;
            this.name = name;
            this.description = description;
            this.catchMaterials = catchMaterials;
            this.evolutionMaterials = evolutionMaterials;
            this.evolutionResult = evolutionResult;
        }

        public Sprite Icon { get => icon; }
        public int Id { get => id; }
        public string Name { get => name; }
        public string Description { get => description; }
        public List<EvolutionMaterial> EvolutionMaterials { get => evolutionMaterials; }
        public int EvolutionResult { get => evolutionResult; }
    }
}
