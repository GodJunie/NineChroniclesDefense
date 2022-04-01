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
    public class MonsterStatusInfo : ScriptableObject {
        [SerializeField]
        [LabelText("스탯")]
        private List<Status> statuses;

        public void Init(List<Status> statuses) {
            this.statuses = statuses;
        }

        public List<Status> Statuses { get => statuses; }
    }
}