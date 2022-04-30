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
    using Management;

    public class UIWorkShopInfo : UIBuildingBaseInfo {
        [TabGroup("group", "Settings")]
        [SerializeField]
        private UIWorkShopSlot workShopSlotPrefab;

        [TabGroup("group", "Info")]
        [SerializeField]
        private Transform recipeSlotContainer;

        [TabGroup("group", "Craft")]
        [SerializeField]
        private GameObject panelCraft;
        [TabGroup("group", "Craft")]
        [SerializeField]
        private Transform craftSlotContainer;
        [TabGroup("group", "Craft")]
        [SerializeField]
        private UISlot craftResultSlot;

        private RecipeInfo info;
        private WorkShop workshop;

        public void Open(WorkShop workshop) {
            this.workshop = workshop;
            this.panelCraft.SetActive(false);

            base.Open(workshop);
        }

        protected override void OpenInfo() {
            base.OpenInfo();

            for(int i = 0; i < recipeSlotContainer.childCount; i++) {
                Destroy(recipeSlotContainer.GetChild(i).gameObject);
            }

            for(int i = 0; i < workshop.Statuses.Count; i++) {
                bool isLock = i > workshop.Level;
                
                foreach(var recipeId in (workshop.Statuses[i] as RestaurantStatusInfo).RecipeIds) {
                    var recipeData = TableLoader.Instance.RecipeTable.Datas.Find(e => e.Id == recipeId);

                    var slot = Instantiate(workShopSlotPrefab, this.recipeSlotContainer);

                    var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == recipeData.ResultId);

                    var icon =  ResourcesManager.Instance.Load<Sprite>(itemData.IconPath);

                    slot.SetUI(icon, recipeData.ResultAmount, isLock, () => OpenCraft(recipeData));
                }
            }
        }

        private void OpenCraft(RecipeInfo info) {
            this.panelInfo.SetActive(false);
            this.panelCraft.SetActive(true);

            this.info = info;

            for(int i = 0; i < craftSlotContainer.childCount; i++) {
                Destroy(craftSlotContainer.GetChild(i).gameObject);
            }
            foreach(var material in info.Materials) {
                var slot = Instantiate(itemSlotPrefab, this.craftSlotContainer);

                var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == material.Id);

                var icon = ResourcesManager.Instance.Load<Sprite>(itemData.IconPath);

                var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                slot.SetUI(icon, count, material.Amount);
            }

            var resultItemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == info.ResultId);
            var resultIcon = ResourcesManager.Instance.Load<Sprite>(resultItemData.IconPath);

            this.craftResultSlot.SetUI(resultIcon, info.ResultAmount.ToString());
        }

        public void OnCraft() {
            foreach(var item in info.Materials) {
                var ownedItem = GameController.Instance.Items.Find(e => e.Id == item.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                if(count < item.Amount) return;
            }

            foreach(var item in info.Materials) {
                GameController.Instance.UseItem(item.Id, item.Amount);
            }

            GameController.Instance.AddItem(info.ResultId, info.ResultAmount);

            building.OnInteract();
        }
    }
}