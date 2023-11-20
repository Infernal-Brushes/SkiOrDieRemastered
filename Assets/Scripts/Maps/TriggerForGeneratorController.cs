using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    Destroy(previousMeshGenerator);

                thisMeshGenerator.GenerateNext();

                Destroy(this.gameObject);
            }
        }
    }
}
