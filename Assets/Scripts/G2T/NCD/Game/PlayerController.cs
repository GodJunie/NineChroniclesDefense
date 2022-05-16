// System
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// UnityEngine
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Cysharp.Threading.Tasks;


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
        [SerializeField]
        private Vector2 rootCanvasPosition;

        [SerializeField]
        private Transform buildingHolder;
        [SerializeField]
        private Vector2 buildingHolderPos;

        private new Transform transform;

        private Direction curDirection;

        private List<Monster> monsters = new List<Monster>();
        private List<BuildingBase> buildings = new List<BuildingBase>();
        private List<FarmingItem> farmingItems = new List<FarmingItem>();

        public Transform BuildingHoder { get => buildingHolder; }

        private IInteractable interactableTarget;

        private void Awake() {
            this.transform = GetComponent<Transform>();
        }

        // Start is called before the first frame update
        void Start() {
            SetDirection(Direction.Right);
        }

        // Update is called once per frame
        void Update() {
            KeyboardInput();

            if(this.interactableTarget != null && interactableTarget.Interacting) {

            } else {
                var interactables = monsters.Select(e => e as IInteractable).Concat(buildings.Select(e => e as IInteractable)).Concat(farmingItems.Select(e => e as IInteractable)).OrderBy(e => Mathf.Abs(this.transform.position.x - e.PosX)).ToList();

                if(interactables.Count > 0) {
                    interactableTarget = interactables[0];
                    interactableTarget.ShowSpacebar();

                    for(int i = 1; i < interactables.Count; i++) {
                        var interatable = interactables[i];
                        interatable.HideSpacebar();
                    }

                    if(Input.GetKeyDown(KeyCode.Space)) {
                        Debug.Log("Space!");
                        interactableTarget.OnInteract();
                    }
                } else {
                    interactableTarget = null;
                }
            }
        }

        public void ResetInteractableTarget() {
            this.interactableTarget = null;
        }

        private void KeyboardInput() {
            if(interactableTarget != null && interactableTarget.Interacting) return;

            float x = Input.GetAxisRaw("Horizontal");

            var pos = this.transform.position + new Vector3(x, 0f, 0f) * speed * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, GameController.Instance.RangeLeft, GameController.Instance.RangeRight);
            this.transform.position = pos;

            if(x != 0) {
                var direction = x > 0 ? Direction.Right : Direction.Left;
                if(this.curDirection != direction) {
                    // current direction is changed
                    SetDirection(direction);
                }
            }
        }

        private void SetDirection(Direction direction) {
            this.curDirection = direction;
            this.skeletonTransform.rotation = Quaternion.Euler(0f, this.defaultDirection == direction ? 0f : 180f, 0f);

            this.uiRoot.anchoredPosition = new Vector2(this.rootCanvasPosition.x * (direction == Direction.Right ? 1 : -1), this.rootCanvasPosition.y);
            this.buildingHolder.localPosition = new Vector2(this.buildingHolderPos.x * (direction == Direction.Right ? 1 : -1), this.buildingHolderPos.y);
        }

        private async void OnTriggerEnter2D(Collider2D collision) {
            switch(collision.tag) {
            case "Monster":
                var monster = collision.GetComponent<Monster>();
                if(this.monsters.Contains(monster))
                    return;
                this.monsters.Add(monster);
                break;
            case "Booty":
                var booty = collision.GetComponent<DropItem>();
                GameController.Instance.AddItem(booty.id, booty.count);
                booty.transform.SetParent(this.transform);
                await UniTask.WhenAll(
                    booty.transform.DOLocalMove(new Vector3(0f, -.3f, 0f), .7f).ToUniTask(), 
                    booty.transform.DOScale(.2f, .7f).ToUniTask()
                );
                Destroy(booty.gameObject);
                break;
            case "Building":
                var building = collision.GetComponent<BuildingBase>();
                if(this.buildings.Contains(building))
                    return;
                this.buildings.Add(building);
                break;
            case "FarmingItem":
                var farmingItem = collision.GetComponent<FarmingItem>();
                if(this.farmingItems.Contains(farmingItem))
                    return;
                this.farmingItems.Add(farmingItem);
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
                monster.HideSpacebar();
                break;
            case "Building":
                var building = collision.GetComponent<BuildingBase>();
                if(this.buildings.Contains(building)) {
                    this.buildings.Remove(building);
                }
                building.HideSpacebar();
                break;
            case "FarmingItem":
                var farmingItem = collision.GetComponent<FarmingItem>();
                if(this.farmingItems.Contains(farmingItem)) {
                    this.farmingItems.Remove(farmingItem);
                }
                farmingItem.HideSpacebar();
                break;
            }
        }
    }
}