using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace G2T.NCD.Game {
    public class Effect : MonoBehaviour {
        [SerializeField]
        private float duration;
        [SerializeField]
        private string id;

        public async void Show() {
            this.transform.localPosition = Vector3.zero;
            this.transform.localScale = Vector3.one;

            this.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            this.gameObject.SetActive(false);
        }

        public string Id { get => id; }
    }
}