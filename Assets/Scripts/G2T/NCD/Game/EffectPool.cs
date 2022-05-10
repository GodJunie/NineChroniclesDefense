using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.Game {
    public class EffectPool : SingletonBehaviour<EffectPool> {
        [SerializeField]
        private List<Effect> prefabs;

        private List<Effect> effectPool = new List<Effect>();

        private Dictionary<string, Effect> effects;

        protected override void Awake() {
            this.effects = prefabs.ToDictionary(e => e.Id, e => e);
        }

        public void ShowEffect(string id, Transform parent = null) {
            var prefab = effects[id];

            var effect = this.effectPool.Find(e => e.Id == id && !e.gameObject.activeInHierarchy);
            
            if(effect == null) {
                effect = Instantiate(prefab);
                this.effectPool.Add(effect);
            }
            effect.transform.SetParent(parent);

            effect.Show();
        }
    }
}
