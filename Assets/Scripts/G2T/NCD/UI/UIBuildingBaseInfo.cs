using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace G2T.NCD.UI {
    using Table;
    using Game;
    using Management;

    public abstract class UIBuildingBaseInfo : MonoBehaviour {
        #region Settings
        [TabGroup("group", "Settings")]
        [SerializeField]
        protected UIItemSlot itemSlotPrefab;
        [TabGroup("group", "Settings")]
        [SerializeField]
        protected UIMonsterSlot monsterSlotPrefab;
        #endregion

        #region Info
        [TabGroup("group", "Info")]
        [LabelText("Panel")]
        [SerializeField]
        protected GameObject panelInfo;
        [TabGroup("group", "Info")]
        [SerializeField]
        private Text textName;
        [TabGroup("group", "Info")]
        [SerializeField]
        private Text textInfoLevel;
        [TabGroup("group", "Info")]
        [SerializeField]
        private GameObject buttonOpenLevelUp;
        #endregion

        #region Level Up
        [TabGroup("group", "LevelUp")]
        [LabelText("Panel")]
        [SerializeField]
        protected GameObject panelLevelUp;
        [TabGroup("group", "LevelUp")]
        [SerializeField]
        private Transform levelUpSlotContainer;
        [TabGroup("group", "LevelUp")]
        [SerializeField]
        private Text textLevelUpBefore;
        [TabGroup("group", "LevelUp")]
        [SerializeField]
        private Text textLevelUpAfter;
        #endregion

        protected BuildingBase building;
        
        protected void Open(BuildingBase building) {
            this.building = building;

            this.panelInfo.SetActive(false);
            this.panelLevelUp.SetActive(false);

            this.OpenInfo();
        }

        protected virtual void OpenInfo() {
            this.panelInfo.SetActive(true);

            textName.text = building.Info.Name;
            textInfoLevel.text = string.Format("Lv. {0}", building.Level + 1);

            buttonOpenLevelUp.SetActive(building.Level < building.Statuses.Count - 1);
        }

        public void OnOpenLevelUp() {
            this.OpenLevelUp();
        }

        protected virtual async void OpenLevelUp() {
            this.panelLevelUp.SetActive(true);
            this.panelInfo.SetActive(false);

            this.textLevelUpBefore.text = string.Format("Lv. {0}", building.Level + 1);
            this.textLevelUpAfter.text = string.Format("Lv. {0}", building.Level + 2);

            for(int i = 0; i < levelUpSlotContainer.childCount; i++) {
                Destroy(levelUpSlotContainer.GetChild(i).gameObject);
            }

            foreach(var item in building.Statuses[building.Level].LevelUpItems) {
                var slot = Instantiate(itemSlotPrefab, this.levelUpSlotContainer);

                var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == item.Id);

                var icon = await ResourcesManager.Instance.LoadAsync<Sprite>(itemData.IconPath);

                var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                slot.SetUI(icon, count, item.Amount);
            }
        }

        public void OnLevelUp() {
            foreach(var item in building.Statuses[building.Level].LevelUpItems) {
                var ownedItem = GameController.Instance.Items.Find(e => e.Id == item.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                if(count < item.Amount) return;
            }

            foreach(var item in building.Statuses[building.Level].LevelUpItems) {
                GameController.Instance.UseItem(item.Id, item.Amount);
            }

            building.OnLevelUp();
        }

        public void OnClose() {
            building.OnInteract();
        }
    }
}