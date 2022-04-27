using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace G2T.NCD.Game {
    using System.IO;
    using Table;
    using UI;

    public class Restaurant : BuildingBase {
        [SerializeField]
        private RestaurantStatusTable table;

        private RestaurantStatusInfo data;

        private List<int> synthesisIds;

        [SerializeField]
        private UIItemSlot slotPrefab;

        public int MonsterAmount { get; private set; }

        public override void Init() {
            this.level = 0;
            this.data = table.Datas[level];
            this.curStatus = data.Status;
            this.curHp = curStatus.Hp;
            this.hpBar.Init(curHp);

            this.synthesisIds = data.SynthesisIds;

            //var d = TableLoader.Instance.BuildingTable.Datas.Find(e => e.Id == Id);
        }

        public override void OnLevelUp() {
            foreach(var item in data.LevelUpItems) {
                var ownedItem = GameController.Instance.Items.Find(e => e.Id == item.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                if(count < item.Amount) return;
            }

            foreach(var item in data.LevelUpItems) {
                GameController.Instance.UseItem(item.Id, item.Amount);
            }

            this.level++;
            this.data = table.Datas[level];
            this.curStatus = data.Status;
            this.curHp = curStatus.Hp;
            this.hpBar.Init(curHp);

            GameController.Instance.SetMonsterAmountsUI();

            if(this.level == table.Datas.Count - 1) {
                this.buttonLevelUp.gameObject.SetActive(false);
            }

            this.ClosePanel();
        }

        public override void OpenPanel() {
            base.OpenPanel();
        }

        public override void OpenLevelUpUI() {
            base.OpenLevelUpUI();
            for(int i = 0; i < levelUpItemSlotContainer.childCount; i++) {
                Destroy(levelUpItemSlotContainer.GetChild(i).gameObject);
            }
            foreach(var item in data.LevelUpItems) {
                var slot = Instantiate(slotPrefab, this.levelUpItemSlotContainer);

                var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == item.Id);

                var path = itemData.IconPath;
                path = path.Replace("Assets/Resources/", "").Replace(Path.GetExtension(path), "");

                var icon = Resources.Load<Sprite>(path);

                //slot.IconImage.sprite = icon;

                var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                //slot.CountText.text = string.Format("{0}/{1}", count, item.Amount);
                slot.SetUI(icon, count, item.Amount);
            }
        }
    }
}