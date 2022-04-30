// System
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.UI {
    using Game;
    using Table;
    using Management;

    public class UIMonsterInfo : MonoBehaviour {
        #region Settings
        [TabGroup("group", "Settings")]
        [SerializeField]
        private UIItemSlot itemSlotPrefab;
        [TabGroup("group", "Settings")]
        [SerializeField]
        private UIMonsterSlot monsterSlotPrefab;
        #endregion
        
        #region Catch
        [TabGroup("group", "Catch")]
        [LabelText("Panel")]
        [SerializeField]
        private GameObject panelCatch;
        [TabGroup("group", "Catch")]
        [LabelText("Slot Container")]
        [SerializeField]
        private Transform catchSlotContainer;
        #endregion

        #region Info
        [TabGroup("group", "Info")]
        [LabelText("Panel")]
        [SerializeField]
        private GameObject panelInfo;
        [TabGroup("group", "Info")]
        [SerializeField]
        private Text textInfoName;
        [TabGroup("group", "Info")]
        [SerializeField]
        private Text textInfoLevel;
        [TabGroup("group", "Info")]
        [SerializeField]
        private UIStatus uiStatusInfo;
        [TabGroup("group", "Info")]
        [SerializeField]
        private Text textButtonGrowth;
        #endregion

        #region Level Up
        [TabGroup("group", "LevelUp")]
        [LabelText("Panel")]
        [SerializeField]
        private GameObject panelLevelUp;
        [TabGroup("group", "LevelUp")]
        [SerializeField]
        private Transform levelUpSlotContainer;
        [TabGroup("group", "LevelUp")]
        [SerializeField]
        private Text textLevelUpBefore;
        [TabGroup("group", "LevelUp")]
        [SerializeField]
        private Text textLevelUpAfter;
        [TabGroup("group", "LevelUp")]
        [SerializeField]
        private Button buttonLevelUp;
        #endregion

        #region Evolution
        [TabGroup("group", "Evolution")]
        [LabelText("Panel")]
        [SerializeField]
        private GameObject panelEvolution;
        [TabGroup("group", "Evolution")]
        [SerializeField]
        private Transform evolutionSlotContainer;
        [TabGroup("group", "Evolution")]
        [SerializeField]
        private Button buttonEvolution;
        [TabGroup("group", "Evolution")]
        [SerializeField]
        private UISlot evolutionResultSlot;
        #endregion

        private Monster monster;
        private List<Monster> evolutionMaterials = new List<Monster>();

        public void Open(Monster monster) {
            this.monster = monster;

            panelCatch.SetActive(false);
            panelInfo.SetActive(false);
            panelEvolution.SetActive(false);
            panelLevelUp.SetActive(false);

            if(monster.MonsterType == MonsterType.Wild) {
                OpenCatch();
            } else {
                OpenInfo();
            }
        }

        private async void OpenCatch() {
            this.panelCatch.SetActive(true);

            for(int i = 0; i < catchSlotContainer.childCount; i++) {
                Destroy(catchSlotContainer.GetChild(i).gameObject);
            }
            foreach(var item in monster.Info.CatchMaterials) {
                var slot = Instantiate(itemSlotPrefab, this.catchSlotContainer);

                var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == item.Id);

                var icon = await ResourcesManager.Instance.LoadAsync<Sprite>(itemData.IconPath);

                var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                slot.SetUI(icon, count, item.Amount);
            }
        }

        private void OpenInfo() {
            this.panelInfo.SetActive(true);

            this.uiStatusInfo.SetUI(monster.CurStatus);
            this.textInfoName.text = monster.Info.Name;
            this.textInfoLevel.text = string.Format("Lv. {0}", monster.Level + 1);

            if(monster.Level < monster.StatusTable.Datas.Count - 1) {
                this.textButtonGrowth.text = "Level Up";
            } else {
                this.textButtonGrowth.text = "Evolution";
            }
        }

        public void OnOpenLevelUp() {
            if(monster.Level < monster.StatusTable.Datas.Count - 1) {
                this.OpenLevelUp();
            } else {
                this.OpenEvolution();
            }
        }

        private async void OpenLevelUp() {
            this.panelLevelUp.SetActive(true);
            this.panelInfo.SetActive(false);

            this.textLevelUpBefore.text = string.Format("Lv. {0}", monster.Level + 1);
            this.textLevelUpAfter.text = string.Format("Lv. {0}", monster.Level + 2);

            for(int i = 0; i < levelUpSlotContainer.childCount; i++) {
                Destroy(levelUpSlotContainer.GetChild(i).gameObject);
            }

            foreach(var item in monster.StatusTable.Datas[monster.Level].LevelUpItems) {
                var slot = Instantiate(itemSlotPrefab, this.levelUpSlotContainer);

                var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == item.Id);

                var icon = await ResourcesManager.Instance.LoadAsync<Sprite>(itemData.IconPath);

                var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                slot.SetUI(icon, count, item.Amount);
            }
        }

        private async void OpenEvolution() {
            this.panelInfo.SetActive(false);
            this.panelEvolution.SetActive(true);

            var evolutionResult = TableLoader.Instance.MonsterTable.Datas.Find(e => e.Id == monster.Info.EvoutionResult);

            var resultIcon = await ResourcesManager.Instance.LoadAsync<Sprite>(evolutionResult.IconPath);

            this.evolutionResultSlot.SetUI(resultIcon);


            evolutionMaterials.Clear();
            for(int i = 0; i < evolutionSlotContainer.childCount; i++) {
                Destroy(evolutionSlotContainer.GetChild(i).gameObject);
            }

            foreach(var mat in monster.Info.EvolutionMaterials) {
                var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == mat.Id);

                if(itemData != null) {
                    // material is item
                    var slot = Instantiate(itemSlotPrefab, this.evolutionSlotContainer);

                    var icon = await ResourcesManager.Instance.LoadAsync
                        <Sprite>(itemData.IconPath);

                    var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
                    int count = ownedItem == null ? 0 : ownedItem.Count;

                    slot.SetUI(icon, count, mat.Amount);

                    continue;
                }

                var monsterData = TableLoader.Instance.MonsterTable.Datas.Find(e => e.Id == mat.Id);
                if(monsterData != null) {
                    // material is monster
                    var slot = Instantiate(itemSlotPrefab, this.evolutionSlotContainer);

                    var icon = await ResourcesManager.Instance.LoadAsync<Sprite>(monsterData.IconPath);

                    slot.SetUI(icon, 0, mat.Amount, () => {
                        GameController.Instance.OpenMonsterPanel(GameController.Instance.Monsters.Where(e => e.MonsterType == MonsterType.Friendly && e.Id == mat.Id && !evolutionMaterials.Contains(e)).ToList(), monster => {
                            this.evolutionMaterials.Add(monster);
                        });
                    });
                }
            }
        }

        public void OnCatch() {
            foreach(var item in monster.Info.CatchMaterials) {
                var ownedItem = GameController.Instance.Items.Find(e => e.Id == item.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                if(count < item.Amount) return;
            }

            foreach(var item in monster.Info.CatchMaterials) {
                GameController.Instance.UseItem(item.Id, item.Amount);
            }

            monster.OnCatch();
        }

        public void OnLevelUp() {
            var data = monster.StatusTable.Datas[monster.Level];
            foreach(var item in data.LevelUpItems) {
                var ownedItem = GameController.Instance.Items.Find(e => e.Id == item.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                if(count < item.Amount) return;
            }

            foreach(var item in data.LevelUpItems) {
                GameController.Instance.UseItem(item.Id, item.Amount);
            }

            monster.OnLevelUp();
        }

        public void OnEvolution() {
            var items = monster.Info.EvolutionMaterials.Where(e => TableLoader.Instance.ItemTable.Datas.Find(i => i.Id == e.Id) != null);

            var monsters = monster.Info.EvolutionMaterials.Where(e => TableLoader.Instance.MonsterTable.Datas.Find(m => m.Id == e.Id) != null);

            foreach(var item in items) {
                var ownedItem = GameController.Instance.Items.Find(e => e.Id == item.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                if(count < item.Amount) return;
            }

            foreach(var monster in monsters) {
                if(evolutionMaterials.Count(e => e.Id == monster.Id) < monster.Amount)
                    return;
            }

            foreach(var item in items) {
                GameController.Instance.UseItem(item.Id, item.Amount);
            }

            foreach(var monster in evolutionMaterials) {
                monster.OnDead?.Invoke(monster);
                Destroy(monster.gameObject);
            }

            monster.OnEvolution();
        }

        public void OnMove(string direction) {
            if(direction.Equals("Left")) {
                monster.OnMove(Direction.Left);
            } else if(direction.Equals("Right")) {
                monster.OnMove(Direction.Right);
            } else {
                Debug.LogError(string.Format("Undefined direction, {0}", direction));
            }
        }

        public void OnClose() {
            monster.OnInteract();
        }
    }
}