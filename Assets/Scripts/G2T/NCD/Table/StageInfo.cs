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
    public class StageInfo {
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
        [LabelText("타임라인")]
        [SerializeField]
        private StageTimelineInfo timeline;
        [BoxGroup("group/연결")]
        [LabelWidth(80f)]
        [LabelText("배경 이미지")]
        [SerializeField]
        private Sprite background;

        public StageInfo(int id, string name, string description) {
            this.id = id;
            this.name = name;
            this.description = description;
        }

        public int Id { get => id; }
        public string Name { get => name; }
        public string Description { get => description; }
    }
}
