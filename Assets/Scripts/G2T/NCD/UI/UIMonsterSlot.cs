using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace G2T.NCD.UI {
    using Table;
    using Game;

    public class UIMonsterSlot : MonoBehaviour {
        public Image IconImage;
        public Text LevelText;
        public Button Button;

        public void SetUI(Monster monster) {
            var info = TableLoader.Instance.MonsterTable.Datas.Find(e => e.Id == monster.Id);

            var path = info.IconPath;
            path = path.Replace("Assets/Resources/", "").Replace(Path.GetExtension(path), "");

            var icon = Resources.Load<Sprite>(path);

            this.IconImage.sprite = icon;

            LevelText.text = string.Format("Lv. {0}", monster.Level + 1);
        }

        [Button]
        public void Init() {
            this.IconImage = this.transform.GetChild(0).GetComponent<Image>();
            this.LevelText = this.transform.GetChild(1).GetComponent<Text>();
            this.Button = this.GetComponent<Button>();
        }
    }
}