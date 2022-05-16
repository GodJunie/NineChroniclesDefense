using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.UI {

    public class UITitle : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void OnQuit() {
            Application.Quit();
        }

        public void OnNewGame() {
            GameManager.Instance.GoToMain();
        }
    }
}