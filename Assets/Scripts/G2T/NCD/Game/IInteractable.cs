using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.Game {
    public interface IInteractable {
        public void ShowSpacebar();
        public void HideSpacebar();
        public void OnSpacebar();
        public bool Interacting { get; }
        public float PosX { get; }
    }
}
