using UnityEngine;

namespace Assets.Scripts.Maps

{
    public class TriggerForGeneratorController : MonoBehaviour
    {
        private MeshGenerator _meshGenerator;

        private void Start()
        {
            _meshGenerator = GetComponentInParent<MeshGenerator>();
        }

        private void OnTriggerEnter(Collider enter)
        {
            if (enter.CompareTag("Chest") && !enter.isTrigger)
            {
                MapController mapController = FindObjectOfType<MapController>();
                mapController.GenerateNextShape(_meshGenerator.zSize, _meshGenerator.Vertices, _meshGenerator.Triangles);
                mapController.MeshPoolManager.ReturnObject(_meshGenerator.gameObject);
            }
        }
    }
}
