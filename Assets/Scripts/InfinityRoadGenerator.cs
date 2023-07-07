using DwarfTrains.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace
{
    public class InfinityRoadGenerator: MonoBehaviour
    {

        public GameObject roadPartPrefab;
        public GameObject obstaclePrefab;
        public float offset = 10;
        public int count = 10;
        
        [Button("Generate")]
        public void Generate()
        {
            GameObjectUtils.ClearChildren(transform);
            for (int i = 0; i < count; i++)
            {
                var roadPart = Instantiate(roadPartPrefab, transform);
                roadPart.transform.position = new Vector3(0, 0, offset + i * 5);
                
                if (i == 0) continue;
                var obstacle = Instantiate(obstaclePrefab, transform);
                var currentScale = obstacle.transform.localScale;
                currentScale.x = Random.Range(3, 11);
                obstacle.transform.localScale = currentScale;
                obstacle.transform.position = new Vector3(
                    -5 + currentScale.x / 2f + Random.Range(0f, 10 - currentScale.x), 
                    0, 
                    offset + i * 5
                );
            }
            
        }
    }
}