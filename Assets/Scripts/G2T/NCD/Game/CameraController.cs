// System
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
// Other
using Sirenix.OdinInspector;


namespace G2T.NCD.Game {
    public class CameraController : MonoBehaviour {
        // Serialized Memnbers 
        [SerializeField]
        private float scrollSpeedMin;
        [SerializeField]
        private float scrollSpeedMax;
        [SerializeField]
        private float scrollAreaWidth;

        [SerializeField]
        private float distanceZ = -10;

        private new Transform transform;
        private Transform target;

        private bool fixPos;

        private void Awake() {
            this.transform = GetComponent<Transform>();
        }

        // Start is called before the first frame update
        void Start() {
            target = GameObject.FindWithTag("Player").transform;
        }

        // Update is called once per frame
        void Update() {
            if(fixPos) {
                this.transform.position = new Vector3(target.position.x, target.position.y, distanceZ);
            } else {
                MosueScroll();
            }

            if(Input.GetKeyDown(KeyCode.F)) {
                this.transform.position = new Vector3(target.position.x, target.position.y, distanceZ);
            }
            if(Input.GetKeyDown(KeyCode.Y)) {
                fixPos = !fixPos;
            }
        }

        private void MosueScroll() {
            var pos = Input.mousePosition;
            var width = UnityEngine.Screen.width;

            var rangeLeft = GameController.Instance.RangeLeft;
            var rangeRight = GameController.Instance.RangeRight;

            if(pos.x < scrollAreaWidth) {
                // scroll left
                var speed = Mathf.Lerp(scrollSpeedMax, scrollSpeedMin, Mathf.Clamp01(pos.x / scrollAreaWidth));
                var x = Mathf.Clamp(this.transform.position.x - speed * Time.deltaTime, rangeLeft, rangeRight);
                this.transform.position = new Vector3(x, this.transform.position.y, this.transform.position.z);
            }

            if(pos.x > width - scrollAreaWidth) {
                // scroll right
                var speed = Mathf.Lerp(scrollSpeedMax, scrollSpeedMin, Mathf.Clamp01((width - pos.x) / scrollAreaWidth));
                var x = Mathf.Clamp(this.transform.position.x + speed * Time.deltaTime, rangeLeft, rangeRight);
                this.transform.position = new Vector3(x, this.transform.position.y, this.transform.position.z);
            }
        }
    }
}