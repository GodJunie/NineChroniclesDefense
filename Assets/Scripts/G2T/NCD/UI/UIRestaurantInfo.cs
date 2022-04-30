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

    public class UIRestaurantInfo : UIBuildingBaseInfo {
        [TabGroup("group", "Settings")]
        [SerializeField]
        private UIWorkShopSlot workShopSlotPrefab;

        [TabGroup("group", "Info")]
        [SerializeField]
        private Transform recipeSlotContainer;

        [TabGroup("group", "Cook")]
        [SerializeField]
        private GameObject panelCook;
        [TabGroup("group", "Cook")]
        [SerializeField]
        private Transform cookSlotContainer;
        [TabGroup("group", "Cook")]
        [SerializeField]
        private UISlot cookResultSlot;

        private RecipeInfo info;
        private Restaurant restaurant;

        public void Open(Restaurant restaurant) {
            this.restaurant = restaurant;
            this.panelCook.SetActive(false);

            base.Open(restaurant);
        }

        protected override async void OpenInfo() {
            base.OpenInfo();

            for(int i = 0; i < recipeSlotContainer.childCount; i++) {
                Destroy(recipeSlotContainer.GetChild(i).gameObject);
            }

            //for(int i = 0; i < restaurant.Statuses.Count; i++) {
            //    bool isLock = i > workshop.Level;

            //    foreach(var recipeId in (workshop.Statuses[i] as RestaurantStatusInfo).RecipeIds) {
            //        var recipeData = TableLoader.Instance.RecipeTable.Datas.Find(e => e.Id == recipeId);

            //        var slot = Instantiate(workShopSlotPrefab, this.recipeSlotContainer);

            //        var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == recipeData.ResultId);

            //        var icon = await ResourcesManager.Instance.LoadAsync<Sprite>(itemData.IconPath);

            //        slot.SetUI(icon, recipeData.ResultAmount, isLock, () => OpenCraft(recipeData));
            //    }
            //}
        }

        private async void OpenCraft(RecipeInfo info) {
            this.panelInfo.SetActive(false);
            //this.panelCraft.SetActive(true);

            //this.info = info;

            //for(int i = 0; i < craftSlotContainer.childCount; i++) {
            //    Destroy(craftSlotContainer.GetChild(i).gameObject);
            //}
            //foreach(var material in info.Materials) {
            //    var slot = Instantiate(itemSlotPrefab, this.craftSlotContainer);

            //    var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == material.Id);

            //    var icon = await ResourcesManager.Instance.LoadAsync<Sprite>(itemData.IconPath);

            //    var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
            //    int count = ownedItem == null ? 0 : ownedItem.Count;

            //    slot.SetUI(icon, count, material.Amount);
            //}

            //var resultItemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == info.ResultId);
            //var resultIcon = await ResourcesManager.Instance.LoadAsync<Sprite>(resultItemData.IconPath);

            //this.craftResultSlot.SetUI(resultIcon, info.ResultAmount.ToString());
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