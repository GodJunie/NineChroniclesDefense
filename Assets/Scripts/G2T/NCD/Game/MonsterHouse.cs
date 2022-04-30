// System
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.Game {
    using Table;
    using UI;

    public class MonsterHouse : BuildingBase {
        public override List<BuildingStatusInfo> Statuses {
            get {
                return (StatusTable as MonsterHouseStatusTable).Datas.Select(e => e as BuildingStatusInfo).ToList();
            }
        }

        public int MonsterAmount { get; private set; }

        public override async Task Init(BuildingInfo info) {
            await base.Init(info);

            this.MonsterAmount = (Statuses[this.Level] as MonsterHouseStatusInfo).MonsterAmount;
        }

        protected override void ClosePanel() {
            UIManager.Instance.CloseUI("monster-house-info");
        }

        protected override void OpenPanel() {
            UIManager.Instance.OpenUI("monster-house-info", this.uiRoot).GetComponent<UIMonsterHouseInfo>().Open(this);
        }
    }
}