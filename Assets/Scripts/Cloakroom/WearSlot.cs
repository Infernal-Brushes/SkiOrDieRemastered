using Assets.Scripts.Player;
using Assets.Scripts.Wears;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Cloakroom
{
    public class WearSlot : MonoBehaviour, IPointerDownHandler
    {
        public Wear wear;
        public TMP_Text title;
        public CloakroomController cloakroomController;

        private void Start()
        {
            if (wear != null)
                title.text = wear.Title;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (wear != null)
            {
                GameObject wearGO;
                if (cloakroomController.currentInventory == CloakroomController.Inventory.market)
                    FindObjectOfType<PlayerAppearance>().PutOnWear(wear, out wearGO, true);
                else
                    FindObjectOfType<PlayerAppearance>().PutOnWear(wear, out wearGO);
                cloakroomController.PutOnWear(wearGO.GetComponent<Wear>());
            }
        }
    }
}
