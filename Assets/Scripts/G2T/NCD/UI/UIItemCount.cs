using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.UI {
    public class UIItemCount : MonoBehaviour {
        [SerializeField]
        private Transform container;
        [SerializeField]
        private UIItemCountSlot slotPrefab;
        [SerializeField]
        private float interval = 0.1f;

        private List<UIItemCountSlot> pool = new List<UIItemCountSlot>();

        private Queue<(int id, int amount)> items = new Queue<(int id, int amount)>();

        private void Start() {
            StartCoroutine(PlayQueue());
        }

        public void EnqueueItem(int id, int amount) {
            items.Enqueue((id, amount));
        }

        private IEnumerator PlayQueue() {
            while(true) {
                if(items.Count > 0) {
                    var item = items.Dequeue();

                    var slot = pool.Find(e => !e.gameObject.activeInHierarchy);
                    if(slot == null) {
                        slot = Instantiate(slotPrefab, container);
                        pool.Add(slot);
                    }
                    slot.Init(item.id, item.amount);

                    yield return new WaitForSeconds(interval);
                }
                yield return null;
            }
        }

    }
}