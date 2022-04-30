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

namespace G2T.NCD.UI {
    using Table;
    using Game;

    public class UIBunkerInfo : UIBuildingBaseInfo {
        [TabGroup("group", "Info")]
        [SerializeField]
        private UIStatus uiStatus;
        [TabGroup("group", "Info")]
        [SerializeField]
        private Transform monsterSlotContainer;

        private Bunker bunker;

        public void Open(Bunker bunker) {
            this.bunker = bunker;

            base.Open(bunker);
        }

        protected override void OpenInfo() {
            base.OpenInfo();

            uiStatus.SetUI(bunker.Statuses[bunker.Level].Status);


        }
    }
}