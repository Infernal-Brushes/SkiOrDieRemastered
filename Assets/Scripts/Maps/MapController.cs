using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
 
    public class MapController : MonoBehaviour
    {
        public MeshGenerator meshGeneratorPrefab;

        private Transform startPositionCamera;
        private Transform startPositionMeshGeneratror;


        public GameObject loseMenu;
        public TMP_Text metersText;

        private void Start()
        {
            loseMenu.SetActive(false);
            startPositionCamera = FindObjectOfType<Camera>().transform;
            startPositionMeshGeneratror = meshGeneratorPrefab.transform;
            CreateFirstShape();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }

        private void CreateFirstShape()
        {
            var meshGenerator = Instantiate(meshGeneratorPrefab, startPositionMeshGeneratror.position, startPositionMeshGeneratror.rotation);
            meshGenerator.CreateShape();
        }

        public void Restart()
        {
            FindObjectOfType<PlayerController>().StopAllCoroutines();

            FindObjectOfType<InGameMenu>().pauseButton.SetActive(true);

            var meshes = FindObjectsOfType<MeshGenerator>();
            for (int i = 0; i < meshes.Length; i++)
            {
                Destroy(meshes[i].gameObject);
            }

            CreateFirstShape();


            FindObjectOfType<PlayerController>().RestartToDefaultPosition();

            Camera camera = FindObjectOfType<Camera>();
            camera.transform.position = startPositionCamera.position;
            camera.transform.rotation = startPositionCamera.rotation;



            loseMenu.SetActive(false);
        }

        public void ShowLoseMenu(int meters)
        {
            metersText.text = meters.ToString();
            loseMenu.SetActive(true);
        }
    }
}
