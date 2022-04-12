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
    public class MonsterInfo : ExcelData {
        #region Structs
        [Serializable]
        public struct EvolutionMaterial {
            [SerializeField]
            [HorizontalGroup("group")]
            [BoxGroup("group/아이디")]
            [HideLabel]
            public int id;

            [SerializeField]
            [HorizontalGroup("group")]
            [BoxGroup("group/개수")]
            [HideLabel]
            public int amount;
        }

        [Serializable]
        public struct CatchMaterial {
            [SerializeField]
            [HorizontalGroup("group")]
            [BoxGroup("group/아이디")]
            [HideLabel]
            public int id;

            [SerializeField]
            [HorizontalGroup("group")]
            [BoxGroup("group/개수")]
            [HideLabel]
            public int amount;
        }
        #endregion

        [HorizontalGroup("group")]
        // 정보
        [BoxGroup("group/정보")]
        [LabelText("아이디")]
        [LabelWidth(80f)]
        [SerializeField]
        public int id;
        [BoxGroup("group/정보")]
        [LabelText("이름")]
        [LabelWidth(80f)]
        [SerializeField]
        public string name;
        [BoxGroup("group/정보")]
        [LabelText("설명")]
        [LabelWidth(80f)]
        [SerializeField]
        public string description;

        // 진화
        [BoxGroup("group/진화")]
        [LabelWidth(80f)]
        [LabelText("진화 재료")]
        [SerializeField]
        public List<EvolutionMaterial> evolutionMaterials;
        [BoxGroup("group/진화")]
        [LabelWidth(80f)]
        [LabelText("진화 결과")]
        [SerializeField]
        public int evolutionResult;

        // 포획
        [BoxGroup("group/포획")]
        [LabelText("포획 재료")]
        [SerializeField]
        public List<CatchMaterial> catchMaterials;

        // 오브젝트
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("프리팹")]
        [SerializeField]
        public Monster prefab;
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("스탯")]
        [SerializeField]
        public MonsterStatusInfo status;
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("아이콘")]
        [SerializeField]
        public Sprite icon;
    }
}
