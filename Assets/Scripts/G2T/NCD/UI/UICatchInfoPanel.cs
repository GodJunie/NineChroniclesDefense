using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace G2T.NCD.UI {
    using Table;
    using Data;
    using Game;

    public class UICatchInfoPanel : MonoBehaviour {
        [FoldoutGroup("UI Objects")]
        [SerializeField]
        private Button catchButton;
        [FoldoutGroup("UI Objects")]
        [SerializeField]
        private UICatchPanelSlot slotPrefab;
        [FoldoutGroup("UI Objects")]
        [SerializeField]
        private RectTransform slotsContainer;

        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Color iconDisabledColor= new Color(1,1,1,.5f);
        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Color iconEnabledColor = Color.white;

        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Color textDisabledColor = Color.red;
        [FoldoutGroup("UI Settings")]
        [SerializeField]
        private Color textEnabledColor = Color.green;

        private List<UICatchPanelSlot> slots = new List<UICatchPanelSlot>();
        private List<ItemData> needs;

        private Action OnCatch;

        private void Awake() {

        }

        public void Open(List<ItemData> needs, Action onCatch) {
            this.needs = needs;
            this.OnCatch = onCatch;

            var inventory = GameController.Instance.Items;

            foreach(var slot in this.slots) {
                slot.gameObject.SetActive(false);
            }

            for(int i = 0; i < this.needs.Count; i++) {
                var need = this.needs[i];
                var info = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == need.Id);

                UICatchPanelSlot slot;
                if(i < this.slots.Count) {
                    slot = this.slots[i];
                } else {
                    slot = Instantiate(this.slotPrefab, this.slotsContainer);
                    this.slots.Add(slot);
                    slot.transform.localScale = Vector3.one;
                }

                var inventoryItem = inventory.Find(e => e.Id == need.Id);
                int count = inventoryItem == null ? 0 : inventoryItem.Count;

                slot.IconImage.color = count < need.Count ? iconDisabledColor : iconEnabledColor;
                slot.CountText.text = string.Format("<color=#{0}>{1}</color>/{2}", (count < need.Count ? textDisabledColor : textEnabledColor).GetHexString(), count, need.Count);

                slot.gameObject.SetActive(true);
                slot.IconImage.sprite = info.Icon;
            }

            this.catchButton.interactable = this.CanCatch();

            this.catchButton.onClick.RemoveAllListeners();
            this.catchButton.onClick.AddListener(() => {
                foreach(var need in needs) {
                    var item = GameController.Instance.Items.Find(e => e.Id == need.Id);
                    item.Count -= need.Count;
                }
                this.OnCatch?.Invoke();
            });

        }

        private bool CanCatch() {
            foreach(var need in needs) {
                var item = GameController.Instance.Items.Find(e => e.Id == need.Id);
                if(item == null)
                    return false;
                
                if(item.Count < need.Count)
                    return false;
            }
            return true;
        }
    }
}
