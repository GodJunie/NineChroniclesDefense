using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.Game {
    using Cysharp.Threading.Tasks;
    using Table;

    public class FarmingItem : MonoBehaviour, IInteractable {
        [SerializeField]
        private DropItem dropItemPrefab;

        [SerializeField]
        private GameObject panelSpacebar;

        public bool Interacting { get; private set; }
        public float PosX => this.transform.position.x;

        private bool pending;

        private FarmingItemInfo info;

        public void OnSpacebar() {
            if(Interacting) {
                CancelFarming();
            } else {
                TryFarming();
            }
        }

        public void ShowSpacebar() {
            this.panelSpacebar.SetActive(true);
        }

        public void HideSpacebar() {
            this.panelSpacebar.SetActive(false);
        }

        public void Init(FarmingItemInfo info) {
            this.info = info;
        }

        private async void TryFarming() {
            
        }

        private void CancelFarming() {

        }

        private void OnFarmingComplete() {
            foreach(var dropItem in this.info.DropItems) {
                var item = Instantiate(this.dropItemPrefab, this.transform.position, Quaternion.identity);

                item.Init(dropItem.Id, dropItem.Amount);
            }
            Destroy(this.gameObject);
        }
    }
}