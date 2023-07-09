using System;
using DwarfTrains.Sound;
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
                if (isSpeedUp)
                {
                    GlobalSoundManager.Instanse.PlaySound("Coke");
                }
                else
                {
                    GlobalSoundManager.Instanse.PlaySound("Hamburger");
                }
                Destroy(gameObject);
            }
        }
    }
}