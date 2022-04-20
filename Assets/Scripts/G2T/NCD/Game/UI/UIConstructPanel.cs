using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace G2T.NCD.Game.UI {
    using Table;
    using UnityEngine.EventSystems;

    public class UIConstructPanel : MonoBehaviour {
        private BuildingTable table;

        [SerializeField]
        private List<Image> iconImages;
        [SerializeField]
        private List<Button> buttons;

        [SerializeField]
        private RectTransform tooltipRect;
        [SerializeField]
        private Text tootipName;
        [SerializeField]
        private Text tooltipDescription;

        private void Awake() {
            table = TableLoader.Instance.BuildingTable;

            for(int i = 0; i < buttons.Count; i++) {
                var button = buttons[i];
                var iconImage = iconImages[i];

                if(i < table.Datas.Count) {
                    var data = table.Datas[i];

                    button.onClick.AddListener(() => {
                        int id = data.Id;
                        Debug.Log(string.Format("Try to construct building id : {0}", id));
                    });

                    //try {
                    //    var icon = Resources.Load<Sprite>(data.IconPath);
                    //    iconImage.sprite = icon;
                    //}
                    //catch {

                    //}

                    var trigger = button.GetComponent<EventTrigger>();

                    var pointerEnterEntry = new EventTrigger.Entry();
                    pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
                    pointerEnterEntry.callback.AddListener(pointer => OnPointerEnter((PointerEventData)pointer, data));

                    var pointerExitEntry = new EventTrigger.Entry();
                    pointerExitEntry.eventID = EventTriggerType.PointerExit;
                    pointerExitEntry.callback.AddListener(pointer => OnPointerExit((PointerEventData)pointer));
                  
                    trigger.triggers.Add(pointerEnterEntry);
                    trigger.triggers.Add(pointerExitEntry);
                } else {
                    iconImage.sprite = null;
                    iconImage.color = Color.clear;
                }
            }
        }

        private void Update() {
            if(tooltipRect.gameObject.activeInHierarchy) {
                var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0;

                this.tooltipRect.position = worldPos;
            }
        }

        private void OnPointerEnter(PointerEventData pointer, BuildingInfo data) {
            this.tooltipRect.gameObject.SetActive(true);

            this.tootipName.text = data.Name;
            this.tooltipDescription.text = data.Description;
        }

        private void OnPointerExit(PointerEventData pointer) {
            this.tooltipRect.gameObject.SetActive(false);
        }
    }
}