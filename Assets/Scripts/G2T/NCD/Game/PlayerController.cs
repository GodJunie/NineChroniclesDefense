// System
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// UnityEngine
using UnityEngine;
using Sirenix.OdinInspector;


namespace G2T.NCD.Game {
    public class PlayerController : MonoBehaviour {
        // Serialized Members
        [TitleGroup("Settings")]
        [SerializeField]
        [LabelText("Speed")]
        private float speed;
        [SerializeField]
        private Direction defaultDirection;
        [SerializeField]
        private Transform skeletonTransform;

        [SerializeField]
        private RectTransform uiRoot;

        private new Transform transform;

        private Direction curDirection;

        private List<Monster> monsters = new List<Monster>();

        private void Awake() {
            this.transform = GetComponent<Transform>();
        }

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            KeyboardInput();
        }

        private void KeyboardInput() {
            float x = Input.GetAxisRaw("Horizontal");

            var pos = this.transform.position + new Vector3(x, 0f, 0f) * speed * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, GameController.Instance.RangeLeft, GameController.Instance.RangeRight);
            this.transform.position = pos;

            if(x != 0) {
                var direction = x > 0 ? Direction.Right : Direction.Left;
                if(this.curDirection != direction) {
                    // current direction is changed
                    this.curDirection = direction;
                    this.skeletonTransform.rotation = Quaternion.Euler(0f, direction == this.defaultDirection ? 0f : 180f, 0f);
                }
            }

            if(Input.GetKeyDown(KeyCode.Space)) {
                if(this.monsters.Count > 0) {
                    // find closest monster
                    var first = this.monsters.OrderBy(e => Mathf.Abs(this.transform.position.x - e.transform.position.x)).First();
                    first.OnInteraction();
                }
            }
        }


        private void OnTriggerEnter2D(Collider2D collision) {
            switch(collision.tag) {
            case "Monster":
                var monster = collision.GetComponent<Monster>();
                if(this.monsters.Contains(monster))
                    return;
                this.monsters.Add(monster);
                break;
            case "Booty":
                var booty = collision.GetComponent<Booty>();
                GameController.Instance.AddItem(booty.id, booty.count);
                Destroy(booty.gameObject);
                break;
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            switch(collision.tag) {
            case "Monster":
                var monster = collision.GetComponent<Monster>();
                if(this.monsters.Contains(monster)) {
                    this.monsters.Remove(monster);
                }
                break;
            }
        }
    }
}