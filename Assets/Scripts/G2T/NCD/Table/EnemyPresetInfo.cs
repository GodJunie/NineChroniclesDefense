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
    public class EnemyPresetInfo : ExcelData {
        #region Structs
        [Serializable]
        public struct DropItem {
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

            public DropItem(int id, int amount) {
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
        [LabelText("적 정보 아이디")]
        [LabelWidth(80f)]
        [SerializeField]
        private int enemyId;

        // 전투
        [BoxGroup("group/전투")]
        [HorizontalGroup("group/전투/group", .5f)]
        [HideLabel]
        [SerializeField]
        private Status status;
        [HorizontalGroup("group/전투/group", .5f)]
        [LabelText("드랍 아이템")]
        [SerializeField]
        private List<DropItem> dropItems;

        public EnemyPresetInfo(int id, int enemyId, Status status, List<DropItem> dropItems) {
            this.id = id;
            this.enemyId = enemyId;
            this.status = status;
            this.dropItems = dropItems;
        }

        public int Id { get => id; }
    }
}
