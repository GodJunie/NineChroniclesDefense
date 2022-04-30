// System
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.Game {
    using Table;
    using UI;

    public class Bunker : BuildingBase {
        public override List<BuildingStatusInfo> Statuses {
            get {
                return (StatusTable as BunkerStatusTable).Datas.Select(e => e as BuildingStatusInfo).ToList();
            }
        }

        public List<Monster> Monsters { get; private set; }

        protected override void OpenPanel() {
            UIManager.Instance.OpenUI("bunker-info").GetComponent<UIBunkerInfo>().Open(this);
        }

        protected override void ClosePanel() {
            UIManager.Instance.CloseUI("bunker-info");
        }
    }
}