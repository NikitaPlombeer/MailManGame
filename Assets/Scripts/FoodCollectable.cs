using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class FoodCollectable: MonoBehaviour
    {

        public float rotationSpeed = 30f;
        public bool isSpeedUp = true;
        
        void Update()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.gameObject.GetComponent<MailmanController>().UpdateSpeed(isSpeedUp);
                Destroy(gameObject);
            }
        }
    }
}