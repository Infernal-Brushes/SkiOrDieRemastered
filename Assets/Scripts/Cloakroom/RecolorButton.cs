using Assets.Scripts;
using Assets.Scripts.Cloakroom;
using Assets.Scripts.Player;
using Assets.Scripts.Wears;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class RecolorButton : MonoBehaviour, IPointerDownHandler
{
    public string titleKey;
    public int partIndex;
    public int newMaterialIndex;

    private CloakroomController cloakroomController;

    private void Start()
    {
        cloakroomController = FindObjectOfType<CloakroomController>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var playerController = FindObjectOfType<PlayerAppearance>();
        var wear = playerController.GetComponentsInChildren<Wear>().ToList().Find(w => w.titleKey == titleKey);
        if (wear != null)
        {
            try
            {
                var skinnedMeshRenderer = wear.GetComponentInChildren<SkinnedMeshRenderer>();
                var materials = skinnedMeshRenderer.materials;
                materials[partIndex] = wear.materialPresets[partIndex].materirials[newMaterialIndex];
                skinnedMeshRenderer.materials = materials;

                //если в юзере меняем цвет то надо обновить
                if (cloakroomController.currentInventory == CloakroomController.Inventory.user)
                {
                    FindObjectOfType<User>().UpdateWear(wear);
                }
            }
            catch
            {
                Debug.LogError($"У шмотки{wear.Title} либо нет слота под материал с индексом {partIndex}, либо не назначен материал для этого слота с индексом {newMaterialIndex}");
            }
        }
    }
}
