using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.UI {
    public class UISplash : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            GameManager.Instance.GoToTitle();       
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
