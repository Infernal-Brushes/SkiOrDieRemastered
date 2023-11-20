using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Wears
{
    public class Wear : MonoBehaviour
    {
        public GameObject prefab;

        public string titleKey;
        public string Title
        {
            get
            {
                try
                {
                    return FindObjectOfType<LocalizationManager>().GetValue(titleKey);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public enum Slot
        {
            torso,
            legs,
            boots,
            wrists,
            hat,
            glasses, //5
            mask, //нижняя часть лица, либо фулл маска
            scarf,
            ski,
            skiPoles,
        }

        public Slot targetSlot;

        /// <summary>
        /// В какие слоты нельзя будет надеть одежду, если надеть эту одежду
        /// </summary>
        [InspectorName("В какие слоты нельзя будет надеть одежду, если надеть эту одежду")]
        public List<Slot> slotRestricts;

        public List<MaterialPreset> materialPresets;
        public Material[] currentMaterials;
    }

    [Serializable]
    public class MaterialPreset
    {
        public string coloringPartTitleKey;
        public List<Material> materirials;
        public bool canChooseThisSlot = true;
    }

    [Serializable]
    public class WearSerializable
    {
        public string titleKey;
       
        public enum Slot
        {
            torso,
            legs,
            boots,
            wrists,
            hat,
            glasses, //5
            mask, //нижняя часть лица, либо фулл маска
            scarf,
            ski,
            skiPoles,
        }

        public Slot targetSlot;

        /// <summary>
        /// В какие слоты нельзя будет надеть одежду, если надеть эту одежду
        /// </summary>
        [InspectorName("В какие слоты нельзя будет надеть одежду, если надеть эту одежду")]
        public List<Slot> slotRestricts;

        //public List<MaterialPreset> materialPresets;
        /// <summary>
        /// у элемента 0 материал по индексу как тут индекс 0
        /// </summary>
        public List<int> currentMaterialsIndexes;
    }
}
