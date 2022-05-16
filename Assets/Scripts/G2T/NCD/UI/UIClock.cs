using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace G2T.NCD.Game {
    public class UIClock : MonoBehaviour {
        [SerializeField]
        private Transform hand;
        [SerializeField]
        private Image fill;
        [SerializeField]
        private Color colorMorning;
        [SerializeField]
        private Color colorDawn;
        [SerializeField]
        private Color colorNight;

        public void SetTimepart(DayTimePart timepart) {
            switch(timepart) {
            case DayTimePart.Morning:
                fill.color = colorMorning;
                break;
            case DayTimePart.Night:
                fill.color = colorNight;
                break;
            case DayTimePart.Dawn:
                fill.color = colorDawn;
                break;
            default:
                break;
            }
        }

        public void RotateHand(float time) {
            this.hand.DORotate(new Vector3(0f, 0f, -360f), time, RotateMode.FastBeyond360).SetEase(Ease.Linear);
        }
    }
}