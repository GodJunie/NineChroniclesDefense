using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace G2T.NCD.UI {
    using Table;
    using Management;
    using System.Linq;

    public class UICollectionPanel : MonoBehaviour {
        #region Tab
        [TabGroup("group", "Tab")]
        [SerializeField]
        private Color colorTextTabOn;
        [TabGroup("group", "Tab")]
        [SerializeField]
        private Color colorTextTabOff;
        [TabGroup("group", "Tab")]
        [SerializeField]
        private Sprite spriteTabOn;
        [TabGroup("group", "Tab")]
        [SerializeField]
        private Sprite spriteTabOff;

        #region Tab Buttons
        [BoxGroup("group/Tab/Buttons")]
        [HorizontalGroup("group/Tab/Buttons/group")]
        [BoxGroup("group/Tab/Buttons/group/Item")]
        [HideLabel]
        [SerializeField]
        private Button buttonTabItem;
        [HorizontalGroup("group/Tab/Buttons/group")]
        [BoxGroup("group/Tab/Buttons/group/Monster")]
        [HideLabel]
        [SerializeField]
        private Button buttonTabMonster;
        [HorizontalGroup("group/Tab/Buttons/group")]
        [BoxGroup("group/Tab/Buttons/group/Enemy")]
        [HideLabel]
        [SerializeField]
        private Button buttonTabEnemy;
        #endregion

        #region Images
        [BoxGroup("group/Tab/Images")]
        [HorizontalGroup("group/Tab/Images/group")]
        [BoxGroup("group/Tab/Images/group/Item")]
        [HideLabel]
        [SerializeField]
        private Image imageTabItem;
        [HorizontalGroup("group/Tab/Images/group")]
        [BoxGroup("group/Tab/Images/group/Monster")]
        [HideLabel]
        [SerializeField]
        private Image imageTabMonster;
        [HorizontalGroup("group/Tab/Images/group")]
        [BoxGroup("group/Tab/Images/group/Enemy")]
        [HideLabel]
        [SerializeField]
        private Image imageTabEnemy;
        #endregion

        #region Texts
        [BoxGroup("group/Tab/Texts")]
        [HorizontalGroup("group/Tab/Texts/group")]
        [BoxGroup("group/Tab/Texts/group/Item")]
        [HideLabel]
        [SerializeField]
        private Text textTabItem;
        [HorizontalGroup("group/Tab/Texts/group")]
        [BoxGroup("group/Tab/Texts/group/Monster")]
        [HideLabel]
        [SerializeField]
        private Text textTabMonster;
        [HorizontalGroup("group/Tab/Texts/group")]
        [BoxGroup("group/Tab/Texts/group/Enemy")]
        [HideLabel]
        [SerializeField]
        private Text textTabEnemy;
        #endregion

        #region Panels
        [BoxGroup("group/Tab/Panels")]
        [HorizontalGroup("group/Tab/Panels/group")]
        [BoxGroup("group/Tab/Panels/group/Item")]
        [HideLabel]
        [SerializeField]
        private GameObject panelTabItem;
        [HorizontalGroup("group/Tab/Panels/group")]
        [BoxGroup("group/Tab/Panels/group/Monster")]
        [HideLabel]
        [SerializeField]
        private GameObject panelTabMonster;
        [HorizontalGroup("group/Tab/Panels/group")]
        [BoxGroup("group/Tab/Panels/group/Enemy")]
        [HideLabel]
        [SerializeField]
        private GameObject panelTabEnemy;
        #endregion
        #endregion

        #region Objects
        [TabGroup("group", "Objects")]
        [SerializeField]
        private UICollectionSlot collectionSlotPrefab;
        [TabGroup("group", "Objects")]
        [SerializeField]
        private UISlot itemSlotPrefab;
        #endregion

        #region Items
        [TabGroup("group", "Items")]
        [SerializeField]
        private Transform itemSlotContainer;
        [TabGroup("group", "Items")]
        [SerializeField]
        private ScrollRect itemScrollRect;
        [TabGroup("group", "Items")]
        [SerializeField]
        private ToggleGroup itemToggleGroup;
        [TabGroup("group", "Items")]
        [SerializeField]
        private Text itemTextName;
        [TabGroup("group", "Items")]
        [SerializeField]
        private Text itemTextDescription;
        #endregion

        #region Monsters
        [TabGroup("group", "Monsters")]
        [SerializeField]
        private Transform monsterSlotContainer;
        [TabGroup("group", "Monsters")]
        [SerializeField]
        private ScrollRect monsterScrollRect;
        [TabGroup("group", "Monsters")]
        [SerializeField]
        private ToggleGroup monsterToggleGroup;
        [TabGroup("group", "Monsters")]
        [SerializeField]
        private Text monsterTextName;
        [TabGroup("group", "Monsters")]
        [SerializeField]
        private Text monsterTextDescription;

        [BoxGroup("group/Monsters/Catch")]
        [SerializeField]
        private Text monsterCatchText;
        [BoxGroup("group/Monsters/Catch")]
        [SerializeField]
        private Transform monsterCatchSlotContainer;

        [BoxGroup("group/Monsters/LevelUp")]
        [SerializeField]
        private Text monsterLevelUpText;
        [BoxGroup("group/Monsters/LevelUp")]
        [SerializeField]
        private Transform monsterLevelUpSlotContainer;

        [BoxGroup("group/Monsters/Evolution")]
        [SerializeField]
        private Text monsterEvolutionText;
        [BoxGroup("group/Monsters/Evolution")]
        [SerializeField]
        private Transform monsterEvolutionSlotContainer;
        #endregion

        #region Enemies
        [TabGroup("group", "Enemies")]
        [SerializeField]
        private Transform enemySlotContainer;
        [TabGroup("group", "Enemies")]
        [SerializeField]
        private ScrollRect enemyScrollRect;
        [TabGroup("group", "Enemies")]
        [SerializeField]
        private ToggleGroup enemyToggleGroup;
        [TabGroup("group", "Enemies")]
        [SerializeField]
        private Text enemyTextName;
        [TabGroup("group", "Enemies")]
        [SerializeField]
        private Text enemyTextDescription;
        [TabGroup("group", "Enemies")]
        [SerializeField]
        private Transform enemyDropItemSlotContainer;
        #endregion

        private Dictionary<string, (Button button, Image image, Text text, GameObject panel)> tabButtons;

        private void Awake() {
            tabButtons = new Dictionary<string, (Button button, Image image, Text text, GameObject panel)> {
                { "Item", (buttonTabItem, imageTabItem, textTabItem, panelTabItem) },
                { "Monster", (buttonTabMonster, imageTabMonster, textTabMonster, panelTabMonster) },
                { "Enemy", (buttonTabEnemy, imageTabEnemy, textTabEnemy, panelTabEnemy) },
            };
        }

        private void OnEnable() {
            OnTab("Item");
        }

        public void OnTab(string sort) {
            foreach(var pair in tabButtons) {
                if(sort == pair.Key) {
                    pair.Value.button.interactable = false;
                    pair.Value.image.sprite = spriteTabOn;
                    pair.Value.text.color = colorTextTabOn;
                    pair.Value.panel.SetActive(true);
                } else {
                    pair.Value.button.interactable = true;
                    pair.Value.image.sprite = spriteTabOff;
                    pair.Value.text.color = colorTextTabOff;
                    pair.Value.panel.SetActive(false);
                }
            }
            if(sort == "Item")
                OpenItems();
            if(sort == "Monster")
                OpenMonsters();
            if(sort == "Enemy")
                OpenEnemies();
        }

        private void OpenItems() {
            for(int i = 0; i < itemSlotContainer.childCount; i++) {
                Destroy(itemSlotContainer.GetChild(i).gameObject);
            }

            var items = TableLoader.Instance.ItemTable.Datas;

            foreach(var item in items) {
                var slot = Instantiate(collectionSlotPrefab, itemSlotContainer);

                var icon = ResourcesManager.Instance.Load<Sprite>(item.IconPath);

                slot.SetUI(icon, (isOn) => {
                    if(isOn) {
                        itemTextName.text = item.Name;
                        itemTextDescription.text = item.Description;
                    }
                }, itemScrollRect, itemToggleGroup);
            }
        }

        private void OpenMonsters() {
            for(int i = 0; i < itemSlotContainer.childCount; i++) {
                Destroy(itemSlotContainer.GetChild(i).gameObject);
            }

            var monsters = TableLoader.Instance.MonsterTable.Datas;

            foreach(var monster in monsters) {
                var slot = Instantiate(collectionSlotPrefab, monsterSlotContainer);

                var icon = ResourcesManager.Instance.Load<Sprite>(monster.IconPath);

                slot.SetUI(icon, isOn => {
                    if(isOn) {
                        monsterTextName.text = monster.Name;
                        monsterTextDescription.text = monster.Description;

                        var items = TableLoader.Instance.ItemTable.Datas;

                        if(monster.CatchMaterials.Count > 0) {
                            monsterCatchText.text = "<color=white>포획 재료</color>";
                            monsterCatchSlotContainer.gameObject.SetActive(true);

                            for(int i = 0; i < monsterCatchSlotContainer.childCount; i++) {
                                Destroy(monsterCatchSlotContainer.GetChild(i).gameObject);
                            }

                            foreach(var catchItem in monster.CatchMaterials) {
                                var item = items.Find(e => e.Id == catchItem.Id);
                                var itemSlot = Instantiate(itemSlotPrefab, monsterCatchSlotContainer);
                                
                                var itemIcon = ResourcesManager.Instance.Load<Sprite>(item.IconPath);

                                itemSlot.SetUI(itemIcon);
                            }
                        } else {
                            monsterCatchText.text = "<color=red>포획 불가</color>";
                            monsterCatchSlotContainer.gameObject.SetActive(false);
                        }

                        var status = ResourcesManager.Instance.Load<ScriptableObject>(monster.StatusPath) as MonsterStatusTable;

                        var levelUpMaterialIds = status.Datas.SelectMany(e => e.LevelUpItems).Select(e => e.Id).Distinct();

                        for(int i = 0; i < monsterLevelUpSlotContainer.childCount; i++) {
                            Destroy(monsterLevelUpSlotContainer.GetChild(i).gameObject);
                        }

                        foreach(var levelUpItem in levelUpMaterialIds) {
                            var item = items.Find(e => e.Id == levelUpItem);
                            var itemSlot = Instantiate(itemSlotPrefab, monsterLevelUpSlotContainer);

                            var itemIcon = ResourcesManager.Instance.Load<Sprite>(item.IconPath);

                            itemSlot.SetUI(itemIcon);
                        }

                        if(monster.EvolutionMaterials.Count > 0) {
                            monsterEvolutionText.text = "<color=white>진화 재료</color>";
                            monsterEvolutionSlotContainer.gameObject.SetActive(true);

                            for(int i = 0; i < monsterEvolutionSlotContainer.childCount; i++) {
                                Destroy(monsterEvolutionSlotContainer.GetChild(i).gameObject);
                            }

                            foreach(var evolutionItem in monster.EvolutionMaterials) {
                                var item = items.Find(e => e.Id == evolutionItem.Id);
                                var itemSlot = Instantiate(itemSlotPrefab, monsterEvolutionSlotContainer);

                                var itemIcon = ResourcesManager.Instance.Load<Sprite>(item.IconPath);

                                itemSlot.SetUI(itemIcon);
                            }
                        } else {
                            monsterEvolutionText.text = "<color=red>진화 불가</color>";
                            monsterEvolutionSlotContainer.gameObject.SetActive(false);
                        }

                        foreach(var fitter in panelTabMonster.GetComponentsInChildren<ContentSizeFitter>()) {
                            LayoutRebuilder.ForceRebuildLayoutImmediate(fitter.GetComponent<RectTransform>());
                        }
                    }
                }, monsterScrollRect, monsterToggleGroup);
            }
        }

        private void OpenEnemies() {
            for(int i = 0; i < enemySlotContainer.childCount; i++) {
                Destroy(enemySlotContainer.GetChild(i).gameObject);
            }

            var enemies = TableLoader.Instance.EnemyTable.Datas;
            var items = TableLoader.Instance.ItemTable.Datas;

            foreach(var enemy in enemies) {
                var slot = Instantiate(collectionSlotPrefab, enemySlotContainer);

                var icon = ResourcesManager.Instance.Load<Sprite>(enemy.IconPath);

                slot.SetUI(icon, (isOn) => {
                    if(isOn) {
                        enemyTextName.text = enemy.Name;
                        enemyTextDescription.text = enemy.Description;

                        var dropItemIds = TableLoader.Instance.EnemyPresetTable.Datas.Where(e => e.EnemyId == enemy.Id).SelectMany(e => e.DropItems).Select(e => e.Id).Distinct();

                        for(int i = 0; i < enemyDropItemSlotContainer.childCount; i++) {
                            Destroy(enemyDropItemSlotContainer.GetChild(i).gameObject);
                        }

                        foreach(var dropItemId in dropItemIds) {
                            var item = items.Find(e => e.Id == dropItemId);
                            var itemSlot = Instantiate(itemSlotPrefab, enemyDropItemSlotContainer);

                            var itemIcon = ResourcesManager.Instance.Load<Sprite>(item.IconPath);

                            itemSlot.SetUI(itemIcon);
                        }

                        foreach(var fitter in panelTabEnemy.GetComponentsInChildren<ContentSizeFitter>()) {
                            LayoutRebuilder.ForceRebuildLayoutImmediate(fitter.GetComponent<RectTransform>());
                        }
                    }
                }, enemyScrollRect, enemyToggleGroup);
            }
        }
    }
}