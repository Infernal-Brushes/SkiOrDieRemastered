using Assets.Scripts.Wears;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Cloakroom
{
    public class CloakroomController : MonoBehaviour
    {
        /// <summary>
        /// одежда на текущей вкладке
        /// </summary>
        private List<Wear> currentWears;
        private Wear.Slot currentSlot;
        public GameObject currentWear;
        private User user;

        /// <summary>
        /// true если метод RefreshWearPanel должен сработать даже если слот не поменялся
        /// </summary>
        private bool refreshAnyway;


        public GameObject panelWear;
        public GameObject panelColors;
        public GameObject linesForColors;
        public GameObject buyButton;
        public GameObject rowPrefab;
        public GameObject colorsLinePrefab;
        public GameObject palettePrefab;
        
        private List<Wear> torsoTab;
        private List<Wear> legsTab;
        private List<Wear> bootsTab;
        private List<Wear> hatsTab;
        private List<Wear> wristsTab;
        private List<Wear> glassesTab;
        private List<Wear> masksTab;
        private List<Wear> scarfTab;

        public Camera camera;
        public Transform defaultCameraTransform;
        public Transform torsoCameraTransform;
        public Transform legsCameraTransform;
        public Transform bootsCameraTransform;
        public Transform hatCameraTransform;
        public Transform wristsCameraTransform;
        public Transform glassesCameraTransform;
        public Transform masksCameraTransform;
        public Transform scarfCameraTransform;

        public float cameraMoveSpeed = 2f;
        public float cameraRotateSpeed = 2f;

        private Coroutine moveCoroutine;
        private Coroutine rotateCoroutine;

        private bool isRotatedToDefault;

        public enum Inventory
        {
            /// <summary>
            /// открыт инвентарь магазина
            /// </summary>
            market,
            /// <summary>
            /// открыт инвентарь игрока
            /// </summary>
            user
        }
        public Inventory currentInventory;

        List<GameObject> rows = new List<GameObject>();

        private void Start()
        {
            torsoTab = FindObjectOfType<Prefabs>().torso;
            legsTab = FindObjectOfType<Prefabs>().legs;
            bootsTab = FindObjectOfType<Prefabs>().boots;
            hatsTab = FindObjectOfType<Prefabs>().hats;
            wristsTab = FindObjectOfType<Prefabs>().wrists;
            glassesTab = FindObjectOfType<Prefabs>().glasses;
            masksTab = FindObjectOfType<Prefabs>().masks;
            scarfTab = FindObjectOfType<Prefabs>().scarf;

            refreshAnyway = true;
            currentInventory = Inventory.market;
            user = user = FindObjectOfType<User>();
            RefreshWearPanel(0);
        }

        public void SwitchInventory(int inventory)
        {
            currentInventory = (Inventory)inventory;
            refreshAnyway = true;
            RefreshWearPanel((int)currentSlot);
        }

        public void RefreshWearPanel(int slot)
        {
            buyButton.SetActive(false);
            Wear.Slot newTab = (Wear.Slot)slot;
         

            if (currentSlot == newTab && !refreshAnyway)
                return;
            else
            {
                refreshAnyway = false;
            }

            DestroyColorsLines();
            panelColors.SetActive(false);

            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            if (rotateCoroutine != null)
                StopCoroutine(rotateCoroutine);

            if (rows.Count > 0)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    Destroy(rows[i]);
                }
            }

            currentSlot = newTab;
            RotateByCurrentSlot();
            if (currentInventory == Inventory.user)
            {
                GetWearsFromUser();
            }
            else
            {
                //показываем все кроме тех которые уже есть у юзера
                if (currentSlot == Wear.Slot.torso)
                {
                    currentWears = torsoTab.Where(w => !user.UserWears.Any(ww => ww.titleKey == w.titleKey)).ToList();
                }
                else if (currentSlot == Wear.Slot.legs)
                {
                    currentWears = legsTab.Where(w => !user.UserWears.Any(ww => ww.titleKey == w.titleKey)).ToList();
                }
                else if (currentSlot == Wear.Slot.boots)
                {
                    currentWears = bootsTab.Where(w => !user.UserWears.Any(ww => ww.titleKey == w.titleKey)).ToList();
                }
                else if (currentSlot == Wear.Slot.hat)
                {
                    currentWears = hatsTab.Where(w => !user.UserWears.Any(ww => ww.titleKey == w.titleKey)).ToList();
                }
                else if (currentSlot == Wear.Slot.wrists)
                {
                    currentWears = wristsTab.Where(w => !user.UserWears.Any(ww => ww.titleKey == w.titleKey)).ToList();
                }
                else if (currentSlot == Wear.Slot.glasses)
                {
                    currentWears = glassesTab.Where(w => !user.UserWears.Any(ww => ww.titleKey == w.titleKey)).ToList();
                }
                else if (currentSlot == Wear.Slot.mask)
                {
                    currentWears = masksTab.Where(w => !user.UserWears.Any(ww => ww.titleKey == w.titleKey)).ToList();
                }    
                else if (currentSlot == Wear.Slot.scarf)
                {
                    currentWears = scarfTab.Where(w => !user.UserWears.Any(ww => ww.titleKey == w.titleKey)).ToList();
                }      
            }


            if (currentWears.Count > 0)
            {
                int wearIndex = 0;
                int rowsCount = currentWears.Count / 3;
                if (currentWears.Count % 3 > 0)
                    rowsCount++;

                for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    var row = Instantiate(rowPrefab, panelWear.transform);
                    rows.Add(row);

                    var slots = row.GetComponentsInChildren<WearSlot>();

                    for (int slotIndex = 0; slotIndex < 3; slotIndex++)
                    {
                        if (wearIndex < currentWears.Count)
                        {
                            slots[slotIndex].wear = currentWears[wearIndex];
                            wearIndex++;
                            slots[slotIndex].cloakroomController = this;
                        }
                        else
                        {
                            //break;
                            Destroy(slots[slotIndex].gameObject);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// получить одежду игрока для текущего слота
        /// </summary>
        private void GetWearsFromUser()
        {
            currentWears.Clear();

            var userWears = user.UserWears.Where(w => (int)w.targetSlot == (int)currentSlot).ToList();
            for (int i = 0; i < userWears.Count; i++)
            {
                Wear wear = null;
                if ((Wear.Slot)userWears[i].targetSlot == Wear.Slot.torso)
                {
                    wear = FindObjectOfType<Prefabs>().torso.Find(w => w.titleKey == userWears[i].titleKey);
                }
                else if ((Wear.Slot)userWears[i].targetSlot == Wear.Slot.legs)
                {
                    wear = FindObjectOfType<Prefabs>().legs.Find(w => w.titleKey == userWears[i].titleKey);
                }
                else if ((Wear.Slot)userWears[i].targetSlot  == Wear.Slot.boots)
                {
                    wear = FindObjectOfType<Prefabs>().boots.Find(w => w.titleKey == userWears[i].titleKey);
                }
                else if ((Wear.Slot)userWears[i].targetSlot  == Wear.Slot.hat)
                {
                    wear = FindObjectOfType<Prefabs>().hats.Find(w => w.titleKey == userWears[i].titleKey);
                }
                else if ((Wear.Slot)userWears[i].targetSlot  == Wear.Slot.wrists)
                {
                    wear = FindObjectOfType<Prefabs>().wrists.Find(w => w.titleKey == userWears[i].titleKey);
                }
                else if ((Wear.Slot)userWears[i].targetSlot  == Wear.Slot.glasses)
                {
                    wear = FindObjectOfType<Prefabs>().glasses.Find(w => w.titleKey == userWears[i].titleKey);
                }
                else if ((Wear.Slot)userWears[i].targetSlot  == Wear.Slot.mask)
                {
                    wear = FindObjectOfType<Prefabs>().masks.Find(w => w.titleKey == userWears[i].titleKey);
                }
                else if ((Wear.Slot)userWears[i].targetSlot  == Wear.Slot.scarf)
                {
                    wear = FindObjectOfType<Prefabs>().scarf.Find(w => w.titleKey == userWears[i].titleKey);
                }
                    

                if (wear.prefab == null)
                {
                    Debug.LogError($"В Prefabs нет префаба с ключом {wear.titleKey} в слоте {wear.targetSlot}");
                }

                if (userWears[i].slotRestricts != null)
                {
                    for (int j = 0; j < userWears[i].slotRestricts.Count; j++)
                    {
                        wear.slotRestricts.Add((Wear.Slot)userWears[i].slotRestricts[j]);
                    }
                }
                else
                {
                    userWears[i].slotRestricts = new List<WearSerializable.Slot>();
                }

                var skinnedMeshRenderer = wear.prefab.GetComponentInChildren<SkinnedMeshRenderer>();
                Material[] materials = new Material[wear.materialPresets.Count];
                for (int index = 0; index < materials.Length; index++)
                {
                    materials[index] = wear.materialPresets[index].materirials[userWears[i].currentMaterialsIndexes[index]];
                }
                skinnedMeshRenderer.materials = materials;
                currentWears.Add(wear);
            }
        }

        private void DestroyColorsLines()
        {
            int lastColorLinesCount = linesForColors.transform.childCount;
            for (int i = 0; i < lastColorLinesCount; i++)
            {
                DestroyImmediate(linesForColors.transform.GetChild(0).gameObject);
            }
        }

        public void PutOnWear(Wear wear)
        {
            DestroyColorsLines();

            if (currentInventory == Inventory.market)
            {
                buyButton.SetActive(true);
            }
          
            currentWear = wear.gameObject;

            //отображаем панель цветов
            if (wear.materialPresets.Count > 0 &&
                !wear.materialPresets.All(w => w.canChooseThisSlot == false))
                panelColors.SetActive(true);
            else
            {
                panelColors.SetActive(false);
                //return;
            }   
            //заполняем панель цветов
            for (int lineIndex = 0; lineIndex < wear.materialPresets.Count; lineIndex++)
            {
                if (!wear.materialPresets[lineIndex].canChooseThisSlot)
                    continue;

                var line = Instantiate(colorsLinePrefab, linesForColors.transform);
                for(int materialIndex = 0; 
                    materialIndex < wear.materialPresets[lineIndex].materirials.Count; 
                    materialIndex++)
                {
                    line.GetComponentInChildren<TMP_Text>().text = FindObjectOfType<LocalizationManager>().
                        GetValue(wear.materialPresets[lineIndex].coloringPartTitleKey);
                    var palette = Instantiate(palettePrefab, line.transform.GetChild(1));
                    var colorImageGO = palette.transform.GetChild(0).gameObject;
                    colorImageGO.GetComponent<Image>().color = 
                        wear.materialPresets[lineIndex].materirials[materialIndex].color;

                    var recolorButton = colorImageGO.AddComponent<RecolorButton>();
                    recolorButton.titleKey = wear.titleKey;
                    recolorButton.partIndex = lineIndex;
                    recolorButton.newMaterialIndex = materialIndex;
                }
            }
        }

        public void BuyCurrentWear()
        {
            user.BuyWear(currentWear.GetComponent<Wear>());
            buyButton.SetActive(false);
            refreshAnyway = true;
            RefreshWearPanel((int)currentSlot);
        }

        public void SwitchViewMode()
        {
            if (!isRotatedToDefault)
            {
                isRotatedToDefault = true;
                moveCoroutine = StartCoroutine(MoveCamera(defaultCameraTransform.position));
                rotateCoroutine = StartCoroutine(RotateCamera(defaultCameraTransform.rotation));
            }
            else
            {
                RotateByCurrentSlot();
            }
        }

        private void RotateByCurrentSlot()
        {
            isRotatedToDefault = false;
            if (currentSlot == Wear.Slot.torso)
            {
                moveCoroutine = StartCoroutine(MoveCamera(torsoCameraTransform.position));
                rotateCoroutine = StartCoroutine(RotateCamera(torsoCameraTransform.rotation));
            }
            else if (currentSlot == Wear.Slot.legs)
            {
                moveCoroutine = StartCoroutine(MoveCamera(legsCameraTransform.position));
                rotateCoroutine = StartCoroutine(RotateCamera(legsCameraTransform.rotation));
            }
            else if (currentSlot == Wear.Slot.boots)
            {
                moveCoroutine = StartCoroutine(MoveCamera(bootsCameraTransform.position));
                rotateCoroutine = StartCoroutine(RotateCamera(bootsCameraTransform.rotation));
            }
            else if (currentSlot == Wear.Slot.hat)
            {
                moveCoroutine = StartCoroutine(MoveCamera(hatCameraTransform.position));
                rotateCoroutine = StartCoroutine(RotateCamera(hatCameraTransform.rotation));
            }
            else if (currentSlot == Wear.Slot.wrists)
            {
                moveCoroutine = StartCoroutine(MoveCamera(wristsCameraTransform.position));
                rotateCoroutine = StartCoroutine(RotateCamera(wristsCameraTransform.rotation));
            }
            else if (currentSlot == Wear.Slot.glasses)
            {
                moveCoroutine = StartCoroutine(MoveCamera(glassesCameraTransform.position));
                rotateCoroutine = StartCoroutine(RotateCamera(glassesCameraTransform.rotation));
            }
            else if (currentSlot == Wear.Slot.mask)
            {
                moveCoroutine = StartCoroutine(MoveCamera(masksCameraTransform.position));
                rotateCoroutine = StartCoroutine(RotateCamera(masksCameraTransform.rotation));
            }
            else if (currentSlot == Wear.Slot.scarf)
            {
                moveCoroutine = StartCoroutine(MoveCamera(scarfCameraTransform.position));
                rotateCoroutine = StartCoroutine(RotateCamera(scarfCameraTransform.rotation));
            }
        }

        IEnumerator MoveCamera(Vector3 position)
        {
            while (Vector3.Distance(camera.transform.position, position) > 0.0001f)
            {
                camera.transform.position = Vector3.MoveTowards(camera.transform.position, position, Time.deltaTime * cameraMoveSpeed);
                yield return new WaitForFixedUpdate();
            }
            camera.transform.position = position;
        }

        IEnumerator RotateCamera (Quaternion rotation)
        {
            while (Quaternion.Angle(camera.transform.rotation, rotation) > 0.0001f)
            {
                camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, rotation, Time.deltaTime * cameraRotateSpeed);
                yield return new WaitForFixedUpdate();
            }
            camera.transform.rotation = rotation;
        }
    }
}
