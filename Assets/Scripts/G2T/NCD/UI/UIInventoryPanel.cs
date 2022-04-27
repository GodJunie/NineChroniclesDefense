using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace G2T.NCD.UI {
    using Game;
    using Table;

    public class UIInventoryPanel : MonoBehaviour {
        [SerializeField]
        private Transform container;
        [SerializeField]
        private UISlot slotPrefab;

        private void OnEnable() {
            var items = GameController.Instance.Items;

            for(int i = 0; i < container.childCount; i++) {
                Destroy(container.GetChild(i).gameObject);
            }
            foreach(var item in items) {
                var slot = Instantiate(slotPrefab, this.container);

                var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == item.Id);

                var path = itemData.IconPath;
                path = path.Replace("Assets/Resources/", "").Replace(Path.GetExtension(path), "");

                var icon = Resources.Load<Sprite>(path);

                //slot.IconImage.sprite = icon;

                var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                //slot.CountText.text = item.Count.ToString();
                slot.SetUI(icon, count.ToString());
            }
        }
    }
}