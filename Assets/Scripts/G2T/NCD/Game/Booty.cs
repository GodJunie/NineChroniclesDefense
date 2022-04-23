using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.Game {
    using System.IO;
    using Table;

    public class Booty : MonoBehaviour {
        public new SpriteRenderer renderer;

        public int id;
        public int count;

        public void Init(int id, int count) {
            var data = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == id);

            var iconPath = data.IconPath;
            iconPath = iconPath.Replace("Assets/Resources/", "").Replace(Path.GetExtension(iconPath), "");

            var icon = Resources.Load<Sprite>(iconPath);

            renderer.sprite = icon;

            this.id = id;
            this.count = count;
        }
    }
}