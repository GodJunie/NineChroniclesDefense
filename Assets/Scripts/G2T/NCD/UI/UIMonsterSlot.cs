using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace G2T.NCD.UI {
    using Table;
    using Game;
    using Management;

    public class UIMonsterSlot : UISlot {
        public Monster Target { get; private set; }

        public void SetUI(Monster monster, Action onClick = null) {
            this.Target = monster;

            var monsterInfo = TableLoader.Instance.MonsterTable.Datas.Find(e => e.Id == monster.Id);

            var icon = ResourcesManager.Instance.Load<Sprite>(monsterInfo.IconPath);

            base.SetUI(icon, string.Format("Lv. {0}", monster.Level + 1), onClick);
        }
    }
}