using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.Game {
    public interface ICombatable {
        public float CurHp { get; set; }
        public void OnDamaged(float damage);
    }
}