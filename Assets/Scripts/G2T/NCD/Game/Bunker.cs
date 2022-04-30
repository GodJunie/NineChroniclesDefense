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

        public List<Monster> Entries { get; private set; }

        protected override void Start() {
            base.Start();

            this.Entries = new List<Monster>();
        }

        protected override void OpenPanel() {
            UIManager.Instance.OpenUI("bunker-info", this.uiRoot).GetComponent<UIBunkerInfo>().Open(this);
        }

        protected override void ClosePanel() {
            UIManager.Instance.CloseUI("bunker-info");
        }

        public void AddMonster(Monster monster) {
            this.Entries.Add(monster);
            monster.GoToBunker(this);
        }

        public void RemoveMonster(Monster monster) {
            this.Entries.Remove(monster);
            monster.GoOutBunker();
        }
    }
}