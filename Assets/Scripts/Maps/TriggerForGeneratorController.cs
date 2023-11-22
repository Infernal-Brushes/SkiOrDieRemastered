using UnityEngine;

namespace Assets.Scripts
{
    public class TriggerForGeneratorController : MonoBehaviour
    {
        public GameObject previousMeshGenerator;
        private MeshGenerator thisMeshGenerator;


        private void Start()
        {
            thisMeshGenerator = GetComponentInParent<MeshGenerator>();
        }

        private void OnTriggerEnter(Collider enter)
        {
            if (enter.CompareTag("Chest") && !enter.isTrigger)
            {
                //Debug.Log($"Trigger Generator: {enter.gameObject.name}");
                if (previousMeshGenerator != null)
                {
                    Destroy(previousMeshGenerator);
                }

                MapController mapController = FindObjectOfType<MapController>();
                thisMeshGenerator.GenerateNext(mapController.SpawnBarriers);

                Destroy(this.gameObject);
            }
        }
    }
}
