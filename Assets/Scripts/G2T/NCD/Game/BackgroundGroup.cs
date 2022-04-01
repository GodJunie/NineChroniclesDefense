// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// UnityEngine
using UnityEngine;
// Other
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace G2T.NCD.Game {
    public class BackgroundGroup : MonoBehaviour {
        [Serializable]
        public class BackgroundInfo {
            [LabelText("시간대")]
            [SerializeField]
            private DayTimePart timePart;
            [LabelText("오브젝트")]
            [SerializeField]
            private GameObject target;

            public DayTimePart TimePart { get => timePart; }
            public GameObject Target { get => target; }

            private List<(SpriteRenderer, float)> rendererAndAlphas;

            public void Init() {
                this.rendererAndAlphas = new List<(SpriteRenderer, float)>();
                foreach(var renderer in this.target.GetComponentsInChildren<SpriteRenderer>()) {
                    this.rendererAndAlphas.Add((renderer, renderer.color.a));
                }
            }

            public void FadeOut(float duration) {
                foreach(var pair in this.rendererAndAlphas) {
                    pair.Item1.DOFade(0f, duration);
                }
            }

            public void FadeIn(float duration) {
                foreach(var pair in this.rendererAndAlphas) {
                    pair.Item1.DOFade(pair.Item2, duration);
                }
            }

            public void SetAlpha(float alpha) {
                foreach(var pair in this.rendererAndAlphas) {
                    Color origin = pair.Item1.color;
                    origin.a = alpha * pair.Item2;
                    pair.Item1.color = origin;
                }
            }
        }

        [LabelText("배경 리스트")]
        [ListDrawerSettings]
        [SerializeField]
        private List<BackgroundInfo> backgrounds;

        [LabelText("전환 시간")]
        [SerializeField]
        private float duration;

        private DayTimePart curTimePart;

        private void Awake() {
            curTimePart = DayTimePart.Morning;
            foreach(var bg in this.backgrounds) {
                bg.Init();
                if(bg.TimePart == curTimePart) {
                    bg.Target.SetActive(true);
                    bg.SetAlpha(1f);
                } else {
                    bg.Target.SetActive(false);
                    bg.SetAlpha(0f);
                }
            }
        }

        [Button]
        public async void SwitchBakcground(DayTimePart timePart) {
            if(this.curTimePart == timePart) return;
            var curBg = this.backgrounds.Find(e => e.TimePart == curTimePart);
            var targetBg = this.backgrounds.Find(e => e.TimePart == timePart);

            curBg.FadeOut(duration);
            targetBg.FadeIn(duration);

            targetBg.Target.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            curBg.Target.SetActive(false);
            
            this.curTimePart = timePart;
        }

    }
}