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
        [TabGroup("group", "Settings")]
        [SerializeField]
        private UIRestaurantSlot restaurantSlotPrefab;

        [TabGroup("group", "Info")]
        [SerializeField]
        private Transform recipeSlotContainer;
        [TabGroup("group", "Info")]
        [SerializeField]
        private Transform cookingSlotContainer;
        [TabGroup("group", "Info")]
        [SerializeField]
        private GameObject panelCookingList;
        [TabGroup("group", "Info")]
        [SerializeField]
        private GameObject panelRecipeList;

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

        protected override void OpenInfo() {
            base.OpenInfo();

            this.panelCookingList.SetActive(true);
            this.panelRecipeList.SetActive(false);

            for(int i = 0; i < recipeSlotContainer.childCount; i++) {
                Destroy(recipeSlotContainer.GetChild(i).gameObject);
            }
            for(int i = 0; i < restaurant.Statuses.Count; i++) {
                bool isLock = i > restaurant.Level;

                foreach(var recipeId in (restaurant.Statuses[i] as RestaurantStatusInfo).RecipeIds) {
                    var recipeData = TableLoader.Instance.RecipeTable.Datas.Find(e => e.Id == recipeId);

                    var slot = Instantiate(workShopSlotPrefab, this.recipeSlotContainer);

                    var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == recipeData.ResultId);

                    var icon = ResourcesManager.Instance.Load<Sprite>(itemData.IconPath);

                    slot.SetUI(icon, recipeData.ResultAmount, isLock, () => OpenCook(recipeData));
                }
            }

            for(int i = 0; i < cookingSlotContainer.childCount - 1; i++) {
                Destroy(cookingSlotContainer.GetChild(i).gameObject);
            }
            foreach(var cookingInfo in restaurant.Cookings) {
                var slot = Instantiate(restaurantSlotPrefab, cookingSlotContainer);

                slot.transform.SetSiblingIndex(0);

                slot.SetUI(cookingInfo, () => {
                    if(cookingInfo.Timer < 0) {
                        restaurant.OnCookingComplete(cookingInfo);
                        this.OpenInfo();
                    }
                });
            }
        }

        private void OpenCook(RecipeInfo info) {
            this.panelInfo.SetActive(false);
            this.panelCook.SetActive(true);

            this.info = info;

            for(int i = 0; i < cookSlotContainer.childCount; i++) {
                Destroy(cookSlotContainer.GetChild(i).gameObject);
            }
            foreach(var material in info.Materials) {
                var slot = Instantiate(itemSlotPrefab, this.cookSlotContainer);

                var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == material.Id);

                var icon = ResourcesManager.Instance.Load<Sprite>(itemData.IconPath);

                var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                slot.SetUI(icon, count, material.Amount);
            }

            var resultItemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == info.ResultId);
            var resultIcon = ResourcesManager.Instance.Load<Sprite>(resultItemData.IconPath);

            this.cookResultSlot.SetUI(resultIcon, info.ResultAmount.ToString());
        }

        public void OnCook() {
            foreach(var item in info.Materials) {
                var ownedItem = GameController.Instance.Items.Find(e => e.Id == item.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                if(count < item.Amount) return;
            }

            foreach(var item in info.Materials) {
                GameController.Instance.UseItem(item.Id, item.Amount);
            }

            restaurant.OnCooking(info);

            this.panelCook.SetActive(false);
            this.OpenInfo();
        }
    }
}