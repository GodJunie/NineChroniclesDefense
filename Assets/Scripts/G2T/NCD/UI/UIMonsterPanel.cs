using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace G2T.NCD.UI {
    using Table;
    using Game;

    public class UIMonsterPanel : MonoBehaviour {
        [Serializable]
        public class StatusTextGroup {
            [SerializeField]
            private Text textHp;
            [SerializeField]
            private Text textAtk;
            [SerializeField]
            private Text textDef;
            [SerializeField]
            private Text textSpeed;
            [SerializeField]
            private Text textAttackSpeed;
            [SerializeField]
            private Text textCriRate;
            [SerializeField]
            private Text textCriDamage;

            public void SetUI(Status status) {
                textHp.text = status.Hp.ToString();
                textAtk.text = status.Atk.ToString();
                textDef.text = status.Def.ToString();
                textSpeed.text = status.MoveSpeed.ToString();
                textAttackSpeed.text = status.AttackSpeed.ToString();
                textCriRate.text = status.CriRate.ToString();
                textCriDamage.text = status.CriDamage.ToString();
            }
        }


        [SerializeField]
        private Transform container;
        [SerializeField]
        private UIMonsterSlot slotPrefab;

        [SerializeField]
        private GameObject panelInfo;
        [SerializeField]
        private Text textLevel;
        [SerializeField]
        private Text textName;
        [SerializeField]
        private Image imageHp;
        [SerializeField]
        private Text textHp;
        [SerializeField]
        private StatusTextGroup statusTextGroup;
        [SerializeField]
        private Image imageIcon;

        [SerializeField]
        private GameObject selectButton;

        private Monster monster;

        private Action<Monster> onSelect;

        public void Open(List<Monster> monsters = null, Action<Monster> onSelect = null) {
            this.onSelect = onSelect;
            this.gameObject.SetActive(true);

            if(monsters == null) monsters = GameController.Instance.Monsters.Where(e => e.MonsterType == MonsterType.Friendly).ToList();

            for(int i = 0; i < container.childCount; i++) {
                Destroy(container.GetChild(i).gameObject);
            }
            foreach(var monster in monsters) {
                var slot = Instantiate(slotPrefab, this.container);

                slot.SetUI(monster);
                slot.Button.onClick.AddListener(() => {
                    this.monster = monster;
                    this.ShowMonsterInfo();
                });
            }

            selectButton.SetActive(onSelect != null);
        }

        private void ShowMonsterInfo() {
            panelInfo.SetActive(true);

            var info = TableLoader.Instance.MonsterTable.Datas.Find(e => e.Id == monster.Id);

            var path = info.IconPath;
            path = path.Replace("Assets/Resources/", "").Replace(Path.GetExtension(path), "");

            var icon = Resources.Load<Sprite>(path);

            this.imageIcon.sprite = icon;

            textLevel.text = string.Format("Lv. {0}", monster.Level + 1);
            textName.text = info.Name;

            imageHp.fillAmount = monster.CurHp / monster.CurStatus.Hp;
            textHp.text = string.Format("{0:0}/{1:0}", monster.CurHp, monster.CurStatus.Hp);

            statusTextGroup.SetUI(monster.CurStatus);
        }

        public void OnSelect() {
            onSelect?.Invoke(monster);
            gameObject.SetActive(false);
        }
    }
}