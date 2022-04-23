using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.Game {
    using Table;
    public class Bunker : BuildingBase {
        [SerializeField]
        private BunkerStatusTable table;

        private BunkerStatusInfo data;

        public override void Init() {
            this.level = 0;
            this.data = table.Datas[level];
            this.curStatus = data.Status;
        }

        public override void OnLevelUp() {
            throw new System.NotImplementedException();
        }
    }
}