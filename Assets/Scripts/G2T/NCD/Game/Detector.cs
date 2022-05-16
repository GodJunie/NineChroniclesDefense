using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.Game {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Detector : MonoBehaviour {
        public Action<Collider2D> OnEnter;
        public Action<Collider2D> OnExit;

        private new CircleCollider2D collider;

        private void Awake() {
            collider = GetComponent<CircleCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            OnEnter?.Invoke(collision);
        }

        private void OnTriggerExit2D(Collider2D collision) {
            OnExit?.Invoke(collision);
        }

        public void SetRange(float range) {
            collider.radius = range;
        }
    }
}