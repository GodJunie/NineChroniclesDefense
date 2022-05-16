using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace G2T.NCD.UI {
    using Game;
    using Table;
    using Management;

    public class UIInventoryPanel : MonoBehaviour {
        [SerializeField]
        private Transform container;
        [SerializeField]
        private UIInventorySlot slotPrefab;
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private RectTransform tooltipPivot;
        [SerializeField]
        private RectTransform tooltipBackground;
        [SerializeField]
        private Text tooltipText;
        [SerializeField]
        private Vector2 tooltipPadding;
    
        private void Update() {
            if(tooltipPivot.gameObject.activeInHierarchy) {
                var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0;

                this.tooltipPivot.position = worldPos;
            }
        }

        private void OnEnable() {
            var items = GameController.Instance.Items;
            tooltipPivot.gameObject.SetActive(false);

            for(int i = 0; i < container.childCount; i++) {
                Destroy(container.GetChild(i).gameObject);
            }
            foreach(var item in items) {
                var slot = Instantiate(slotPrefab, this.container);

                var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == item.Id);

                var icon = ResourcesManager.Instance.Load<Sprite>(itemData.IconPath);

                var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                //slot.CountText.text = item.Count.ToString();
                slot.SetUI(icon, count.ToString(), 
                    () => {
                        this.tooltipPivot.gameObject.SetActive(true);
                        this.tooltipText.text = itemData.Name;

                        var size = new Vector2(tooltipText.preferredWidth, tooltipText.preferredHeight);
                        tooltipText.rectTransform.sizeDelta = size;

                        this.tooltipBackground.sizeDelta = size + tooltipPadding;
                    },
                    () => {
                        this.tooltipPivot.gameObject.SetActive(false);
                    },
                    scrollRect
                );
            }
        }
    }
}