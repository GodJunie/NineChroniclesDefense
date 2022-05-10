// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Other
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace G2T.NCD.UI {
    using Game;
    using Table;
    using Data;
    using Management;

    public class UIItemCountSlot : MonoBehaviour {
        [SerializeField]
        private Text textCount;
        [SerializeField]
        private Image imageIcon;

        [SerializeField]
        private Color colorPositive;
        [SerializeField]
        private Color colorNegative;

        [SerializeField]
        private float endY = 10f;
        [SerializeField]
        private float duration = 1f;
        [SerializeField]
        private AnimationCurve curve;

        public async void Init(int id, int amount) {
            var data = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == id);

            var icon = ResourcesManager.Instance.Load<Sprite>(data.IconPath);

            imageIcon.sprite = icon;
            textCount.text = amount.ToString("+#;-#;0");

            imageIcon.color = Color.white;
            textCount.color = amount > 0 ? colorPositive : colorNegative;

            gameObject.SetActive(true);
            transform.localPosition = Vector3.zero;

            await UniTask.WhenAll(transform.DOLocalMoveY(Math.Sign(amount) * endY, duration).ToUniTask(), textCount.DOFade(0f, duration).SetEase(curve).ToUniTask(), imageIcon.DOFade(0f, duration).SetEase(curve).ToUniTask());

            gameObject.SetActive(false);
        }
    }
}