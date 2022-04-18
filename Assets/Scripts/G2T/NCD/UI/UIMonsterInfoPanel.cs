// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Other
using Sirenix.OdinInspector;


namespace G2T.NCD.UI {
    using Game;
    using Table;
    using Data;

    public class UIMonsterInfoPanel : MonoBehaviour {
        [FoldoutGroup("UI Objects")]
        [SerializeField]
        private Button levelUpButton;
        [FoldoutGroup("UI Objects")]
        [SerializeField]
        private UICatchPanelSlot slotPrefab;
        [FoldoutGroup("UI Objects")]
        [SerializeField]
        private RectTransform slotsContainer;
        [FoldoutGroup("UI Objects")]
        [SerializeField]
        private GameObject panelStatus;
        [FoldoutGroup("UI Objects")]
        [SerializeField]
        private GameObject panelLevelUp;
        [FoldoutGroup("UI Objects")]
        [SerializeField]
        private Button btnOpenLevelUp;

      
        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Color iconDisabledColor = new Color(1, 1, 1, .5f);
        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Color iconEnabledColor = Color.white;

        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Color textDisabledColor = Color.red;
        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Color textEnabledColor = Color.green;

        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Text textHp;
        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Text textAtk;
        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Text textMoveSpeed;
        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Text textAtkSpeed;
        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Text textLevel;
        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Text textName;

        private List<UICatchPanelSlot> itemSlots = new List<UICatchPanelSlot>();
        private List<ItemData> items;

        public Action OnLevelUp;

        public void Open(Monster monster, Action onLevelUp) {
            this.OnLevelUp = onLevelUp;

            this.textName.text = monster.Name;

            if(monster.Level == monster.LevelInfos.Count) {
                // Max Level
                textLevel.text = "Lv. MAX";
                this.btnOpenLevelUp.interactable = false;
            } else {
                var inventory = GameController.Instance.Items;
                this.items = monster.LevelInfos[monster.Level].Needs;

                foreach(var slot in this.itemSlots) {
                    slot.gameObject.SetActive(false);
                }
                for(int i = 0; i < this.items.Count; i++) {
                    var item = this.items[i];
                    var info = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == item.Id);

                    UICatchPanelSlot slot;
                    if(i < this.itemSlots.Count) {
                        slot = this.itemSlots[i];
                    } else {
                        slot = Instantiate(this.slotPrefab, this.slotsContainer);
                        this.itemSlots.Add(slot);
                        slot.transform.localScale = Vector3.one;
                    }

                    var inventoryItem = inventory.Find(e => e.Id == item.Id);
                    int count = inventoryItem == null ? 0 : inventoryItem.Count;

                    slot.IconImage.color = count < item.Count ? iconDisabledColor : iconEnabledColor;
                    slot.CountText.text = string.Format("<color=#{0}>{1}</color>/{2}", (count < item.Count ? textDisabledColor : textEnabledColor).GetHexString(), count, item.Count);

                    slot.gameObject.SetActive(true);

                    //slot.IconImage.sprite = info.Icon;
                }

                this.items = monster.LevelInfos[monster.Level].Needs;
                this.levelUpButton.interactable = this.CanLevelUp();
                this.textLevel.text = string.Format("Lv. {0}", monster.Level);
                this.btnOpenLevelUp.interactable = true;
            }

            this.textHp.text = monster.CurStatus.Hp.ToString("0");
            this.textAtk.text = monster.CurStatus.Atk.ToString("0");
            this.textMoveSpeed.text = monster.CurStatus.MoveSpeed.ToString("0");
            this.textAtkSpeed.text = monster.CurStatus.AttackSpeed.ToString("0");

            this.panelStatus.SetActive(true);
            this.panelLevelUp.SetActive(false);
        }

        private bool CanLevelUp() {
            foreach(var item in items) {
                var inventoryItem = GameController.Instance.Items.Find(e => e.Id == item.Id);
                if(inventoryItem == null)
                    return false;

                if(inventoryItem.Count < item.Count)
                    return false;
            }
            return true;
        }

        public void LevelUp() {
            foreach(var item in items) {
                var inventoryItem = GameController.Instance.Items.Find(e => e.Id == item.Id);
                inventoryItem.Count -= item.Count;
            }
            this.OnLevelUp?.Invoke();
        }
    }
}