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

        // 나중에 테이블로 빼기
        [SerializeField]
        private float second;

        public bool Interacting { get; private set; }
        public float PosX => this.transform.position.x;

        private bool pending;

        private FarmingItemInfo info;

        public void OnInteract() {
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
            await UniTask.Delay(TimeSpan.FromSeconds(this.second));
            foreach(var data in this.info.DropItems) {
                var item = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == data.Id);
                var dropItem = Instantiate(this.dropItemPrefab, transform.position, Quaternion.identity, null).GetComponent<DropItem>();
                dropItem.Init(data.Id, data.Amount);
            }
            Interacting = false;
            Destroy(this.gameObject);
        }

        private void CancelFarming() {
            Interacting = false;
            // 파밍 연출 취소
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