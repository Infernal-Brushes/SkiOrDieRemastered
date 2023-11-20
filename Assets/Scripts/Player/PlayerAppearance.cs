using Assets.Scripts.Wears;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerAppearance : MonoBehaviour
    {
        public List<GameObject> wears = new List<GameObject>();

        public void PutOnWear(Wear puttingWear, out GameObject wearGO, bool fromCloakRoom = false)
        {
            if (wears.Any(we => puttingWear.slotRestricts.Contains(we.GetComponent<Wear>().targetSlot)))
            {
                Debug.LogError("Нельзя надеть эту шмотку потому что есть надетые шмотки с которыми нельзя носить эту шмотку");
            }

            var oldWear = wears.Find(we => puttingWear.targetSlot == we.GetComponent<Wear>().targetSlot);
            if (oldWear != null)
            {
                wears.Remove(oldWear);
                Destroy(oldWear);
            }

            var playerSkin = GetComponentInChildren<SkinnedMeshRenderer>();

            wearGO = Instantiate(puttingWear.prefab);
            var wear = wearGO.AddComponent<Wear>();
            wear.prefab = puttingWear.prefab;
            wear.titleKey = puttingWear.titleKey;
            wear.targetSlot = puttingWear.targetSlot;
            wear.slotRestricts = puttingWear.slotRestricts;
            wear.materialPresets = puttingWear.materialPresets;

            //если есть какие-то материалы на части, то на каждую часть применяем случайный материал
            if (fromCloakRoom)
            {
                if (wear.materialPresets != null && wear.materialPresets.Count > 0)
                {
                    var skinnedMeshRenderer = wear.GetComponentInChildren<SkinnedMeshRenderer>();
                    var materials = skinnedMeshRenderer.materials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        materials[i] = wear.materialPresets[i].materirials[UnityEngine.Random.Range(0, wear.materialPresets[i].materirials.Count - 1)];
                    }
                    skinnedMeshRenderer.materials = materials;
                    wear.currentMaterials = materials;
                }
            }

            wearGO.transform.SetParent(playerSkin.transform.parent);
            var skinMeshRenderers = wearGO.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skinMeshRenderers.Length > 0)
            {
                for (int i = 0; i < skinMeshRenderers.Length; i++)
                {
                    skinMeshRenderers[i].bones = playerSkin.bones;
                    skinMeshRenderers[i].rootBone = playerSkin.rootBone;
                }
            }
            wears.Add(wearGO);
        }
    }
}
