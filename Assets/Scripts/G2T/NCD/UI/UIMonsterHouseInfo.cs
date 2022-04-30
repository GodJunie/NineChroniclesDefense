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

    public class UIMonsterHouseInfo : UIBuildingBaseInfo {
        [TabGroup("group", "Info")]
        [SerializeField]
        private Text textDescription;
        [TabGroup("group", "Info")]
        [SerializeField]
        private Text textMonsterAmount;

        private MonsterHouse monsterHouse;

        protected override void OpenInfo() {
            base.OpenInfo();

            textDescription.text = monsterHouse.Info.Description;
            textMonsterAmount.text = string.Format("X {0}", monsterHouse.MonsterAmount);
        }

        public void Open(MonsterHouse monsterHouse) {
            this.monsterHouse = monsterHouse;

            base.Open(monsterHouse);
        }
    }
}