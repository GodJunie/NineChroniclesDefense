using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.UI {
    using Game;

    public class UIGameOver : MonoBehaviour {
        public void Open() {
            this.gameObject.SetActive(true);
        }

        public void Retry() {
            GameManager.Instance.GameStart(GameController.Instance.Stage);
        }

        public void Main() {
            GameManager.Instance.GoToMain();
        }
    }
}