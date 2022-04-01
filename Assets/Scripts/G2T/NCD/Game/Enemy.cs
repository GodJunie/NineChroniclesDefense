// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Spine
using Spine.Unity;
// Other
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace G2T.NCD.Game {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour {
        // Serialized Members
        // Componentes
        [SerializeField]
        private SkeletonAnimation anim;
        [SerializeField]
        private HpBar hpBar;
        // Settings
        [SerializeField]
        private Status status;

        [SerializeField]
        private Transform skeletonTransform;

        [SerializeField]
        private Direction defaultDirection;

        [SerializeField]
        private GameObject booty;

        // Private Members
        // Components
        private Rigidbody2D rigid;
        private new Transform transform;
        // game
        private Direction curDirection;
        private State curState;
        private float curHp;

        // objects
        private List<Monster> monsters = new List<Monster>();
        private House targetHouse;

        // Events
        public Action<Enemy> OnDead;

        private void Awake() {
            this.rigid = GetComponent<Rigidbody2D>();
            this.transform = GetComponent<Transform>();
        }

        // Start is called before the first frame update
        void Start() {
            this.curState = State.Move;

            var house = GameController.Instance.House;
            var diff = house.transform.position - this.transform.position;
            if(diff.x < 0) {
                // ¸ñÇ¥¹° ¿ÞÂÊ
                this.SetDirection(Direction.Left);
            } else {
                this.SetDirection(Direction.Right);
            }

            this.curHp = this.status.Hp;
            this.hpBar.Init(this.status.Hp);

            StartFSM();
        }

        // Update is called once per frame
        void Update() {

        }

        private void StartFSM() {
            StartCoroutine(FSM());
        }

        private IEnumerator FSM() {
            while (true) {
                switch(this.curState) {
                case State.Attack:
                    yield return Attack();
                    break;
                case State.Move:
                    yield return Move(this.status.MoveSpeed);
                    break;
                case State.Idle:
                    yield return Idle();
                    break;
                case State.Dead:
                    yield return Dead();
                    break;
                default:
                    yield return null;
                    break;
                }
            }
        }

        private IEnumerator Idle() {
            this.anim.AnimationState.SetAnimation(0, "Idle", true);
            yield return new WaitUntil(() => this.curState != State.Idle);
        }

        private IEnumerator Attack() {
            this.anim.AnimationState.SetAnimation(0, "Attack", false);

            float timer = this.status.AttackSpeed;
            while(timer > 0) {
                timer -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
                if(this.curState != State.Attack)
                    break;
            }
            if(this.curState == State.Attack) {
                try {
                    if(this.monsters.Count > 0) {
                        foreach(var monster in this.monsters) {
                            if(monster != null)
                                monster.OnDamaged(this.status.Atk);
                        }
                    }
                    if(targetHouse != null) {
                        targetHouse.OnDamaged(this.status.Atk);
                    }
                } 
                catch(Exception e) {

                }
            }
        }

        private IEnumerator Move(float speed) {
            this.rigid.velocity = (this.curDirection == Direction.Left ? Vector2.left : Vector2.right) * speed;
            this.anim.AnimationState.SetAnimation(0, "Run", true);
            yield return new WaitUntil(() => this.curState != State.Move);
            this.rigid.velocity = Vector2.zero;
        }

        private IEnumerator Dead() {
            var entry = this.anim.AnimationState.SetAnimation(0, "Die", false);
            yield return new WaitForSpineAnimationComplete(entry);
            Instantiate(booty, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
            yield break;
        }

        public void OnDamaged(float damage) {
            this.curHp -= damage;
            this.hpBar.SetHp(this.curHp);
            if(this.curHp <= 0f) {
                this.OnDead?.Invoke(this);
                this.curState = State.Dead;
            }
        }

        public void OnTriggerEnter2D(Collider2D collision) {
            switch(collision.tag) {
            case "Monster":
                var monster = collision.GetComponent<Monster>();
                if(this.monsters.Contains(monster)) return;
                this.monsters.Add(monster);
                this.curState = State.Attack;
                monster.OnDead += (monster) => {
                    if(this.monsters.Contains(monster)) {
                        this.monsters.Remove(monster);
                        if(this.monsters.Count == 0 && this.targetHouse == null) {
                            this.curState = State.Move;
                        }
                    }
                };
                break;
            case "House":
                this.targetHouse = collision.GetComponent<House>();
                this.curState = State.Attack;
                break;
            }
        }

        public void OnTriggerExit2D(Collider2D collision) {
            switch(collision.tag) {
            case "Monster":
                var monster = collision.GetComponent<Monster>();
                if(this.monsters.Contains(monster)) {
                    this.monsters.Remove(monster);
                    if(this.monsters.Count == 0) {
                        this.curState = State.Move;
                    }
                }
                break;
            }
        }

        private void SetDirection(Direction direction) {
            this.curDirection = direction;
            this.skeletonTransform.rotation = Quaternion.Euler(0f, this.defaultDirection == direction ? 0f : 180f, 0f);
        }
    }
}