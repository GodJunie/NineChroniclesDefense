using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace G2T.NCD.UI {
    using Table;
    using Game;
    using Management;

    public class UIRestaurantSlot : UISlot {
        [SerializeField]
        private GameObject imageComplete;
        [SerializeField]
        private Image imageCooltime;
        [SerializeField]
        private Text textCooltime;

        private Restaurant.CookingInfo info;

        public void SetUI(Restaurant.CookingInfo info, Action onClick) {
            this.info = info;

            var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == info.Info.ResultId);

            var icon = ResourcesManager.Instance.Load<Sprite>(itemData.IconPath);

            base.SetUI(icon, info.Info.ResultAmount.ToString(), onClick);
        }

        private void Update() {
            this.imageComplete.SetActive(info.Timer <= 0);
            this.imageCooltime.gameObject.SetActive(info.Timer > 0);
            this.imageCooltime.fillAmount = info.Timer / info.Cooltime;
            textCooltime.text = info.Timer.ToString("0");
        }
    }
}