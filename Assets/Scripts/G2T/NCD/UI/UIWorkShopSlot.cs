using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.UI {
    public class UIWorkShopSlot : UISlot {
        [SerializeField]
        private GameObject imageLock;

        public void SetUI(Sprite icon, int count, bool isLock, Action onClick) {
            SetUI(icon, count.ToString(), onClick);

            imageLock.SetActive(isLock);

            button.interactable = !isLock;
        }
    }
}