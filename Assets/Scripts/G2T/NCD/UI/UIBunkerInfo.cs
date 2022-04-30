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
        [TabGroup("group", "Info")]
        [SerializeField]
        private Text textEntryCount;

        private Bunker bunker;

        public void Open(Bunker bunker) {
            this.bunker = bunker;

            base.Open(bunker);
        }

        protected override void OpenInfo() {
            base.OpenInfo();

            uiStatus.SetUI(bunker.Statuses[bunker.Level].Status);

            for(int i = 0; i < monsterSlotContainer.childCount - 1; i++) {
                Destroy(monsterSlotContainer.GetChild(i).gameObject);
            }

            foreach(var monster in bunker.Entries) {
                var slot = Instantiate(monsterSlotPrefab, monsterSlotContainer);

                slot.transform.SetSiblingIndex(0);

                slot.SetUI(monster, () => {
                    bunker.RemoveMonster(monster);
                    this.OpenInfo();
                });
            }

            this.textEntryCount.text = string.Format("{0}/{1}", bunker.Entries.Count, (bunker.Statuses[bunker.Level] as BunkerStatusInfo).EntryCount);
        }

        public void OnSelectMonster() {
            GameController.Instance.OpenMonsterPanel(GameController.Instance.Monsters.Where(e => e.gameObject.activeInHierarchy && e.CurState != State.Dead && e.MonsterType == MonsterType.Friendly).ToList(), (monster) => {
                bunker.AddMonster(monster);
                this.OpenInfo();
            });
        }
    }
}