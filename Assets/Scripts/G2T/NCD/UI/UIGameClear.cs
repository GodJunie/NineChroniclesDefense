using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace G2T.NCD.UI {
    using Game;
    using Management;
    using Table;

    public class UIGameClear : MonoBehaviour {
        [SerializeField]
        private Text textContext;
        [SerializeField]
        private UIMonsterSlot slotPrefab;
        [SerializeField]
        private Transform monsterSlotContainer;

        public void Open() {
            this.gameObject.SetActive(true);

            var monsters = GameController.Instance.Monsters;

            this.textContext.text = string.Format("{0}\n\nKill: {1}", GameController.Instance.Stage.Name, GameController.Instance.KillCount);

            for(int i = 0; i < monsterSlotContainer.childCount; i++) {
                Destroy(monsterSlotContainer.GetChild(i).gameObject);
            }

            foreach(var monster in monsters) {
                var slot = Instantiate(slotPrefab, monsterSlotContainer);

                slot.SetUI(monster);
            }

            foreach(var fitter in this.gameObject.GetComponentsInChildren<ContentSizeFitter>()) {
                LayoutRebuilder.ForceRebuildLayoutImmediate(fitter.GetComponent<RectTransform>());
            }
        }

        public void Main() {
            GameManager.Instance.GoToMain();
        }
    }
}