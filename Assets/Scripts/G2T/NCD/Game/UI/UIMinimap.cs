using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace G2T.NCD.Game.UI {
    public class UIMinimap : MonoBehaviour {
        [TabGroup("group","Colors")]
        [SerializeField]
        private Color jadeColor;
        [TabGroup("group","Colors")]
        [SerializeField]
        private Color enemyColor;
        [TabGroup("group","Colors")]
        [SerializeField]
        private Color buildingColor;
        [TabGroup("group","Colors")]
        [SerializeField]
        private Color myMonsterColor;
        [TabGroup("group","Colors")]
        [SerializeField]
        private Color wildMonsterColor;

        [TabGroup("group", "Settings")]
        [SerializeField]
        private float width;
        [TabGroup("group", "Settings")]
        [SerializeField]
        private float worldWidth;

        [TabGroup("group", "Objects")]
        [SerializeField]
        private RectTransform markerContainer;
        [TabGroup("group", "Objects")]
        [SerializeField]
        private UIMinimapMarker markerPrefab;

        private List<UIMinimapMarker> markers = new List<UIMinimapMarker>();


        private Transform playerTransform;

        private void Awake() {

        }

        private void Start() {
            playerTransform = GameController.Instance.Player.transform;

            var marker = GetNewMarker();
            marker.SetTarget(playerTransform);
            marker.SetColor(jadeColor);
            marker.SetPosition(0);
        }

        private void Update() {
            var gameController = GameController.Instance;

            var buildings = gameController.Buildings.Where(e => (e.transform.position - playerTransform.position).x < width / 2);
            var monsters = gameController.Monsters.Where(e => (e.transform.position - playerTransform.position).x < width / 2);
            var enemies = gameController.Enemies.Where(e => (e.transform.position - playerTransform.position).x < width / 2);

            foreach(var marker in markers) {
                if(marker.Target == this.playerTransform) continue;
                if(buildings.Select(e => e.transform).Contains(marker.Target))
                    continue;
                if(monsters.Select(e => e.transform).Contains(marker.Target))
                    continue;
                if(enemies.Select(e => e.transform).Contains(marker.Target))
                    continue;

                marker.gameObject.SetActive(false);
            }
               
            foreach(var building in buildings) {
                var marker = markers.Find(e => e.Target == building.transform);
                if(marker == null) {
                    marker = GetNewMarker();
                }

                marker.SetColor(buildingColor);
                marker.SetTarget(building.transform);
                var x = building.transform.position.x - playerTransform.position.x;
                marker.SetPosition(x * width / worldWidth);
            }
            
            foreach(var monster in monsters) {
                var marker = markers.Find(e => e.Target == monster.transform);
                if(marker == null) {
                    marker = GetNewMarker();
                }

                marker.SetColor(monster.CurrentMonsterType == MonsterType.Friendly ? myMonsterColor : wildMonsterColor);
                marker.SetTarget(monster.transform);

                var x = monster.transform.position.x - playerTransform.position.x;
                marker.SetPosition(x * width / worldWidth);
            }

            foreach(var enemy in enemies) {
                var marker = markers.Find(e => e.Target == enemy.transform);
                if(marker == null) {
                    marker = GetNewMarker();
                }

                marker.SetColor(enemyColor);
                marker.SetTarget(enemy.transform);
                var x = enemy.transform.position.x - playerTransform.position.x;
                marker.SetPosition(x * width / worldWidth);
            }
        }

        private UIMinimapMarker GetNewMarker() {
            var pool = this.markers.Where(e => !e.gameObject.activeInHierarchy).ToList();
            if(pool.Count > 0) {
                var marker = pool[0];
                marker.gameObject.SetActive(true);
                return marker;
            } else {
                var marker = Instantiate(this.markerPrefab, this.markerContainer);
                marker.gameObject.SetActive(true);
                this.markers.Add(marker);
                return marker;
            }
        }
    }
}