using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class DeliveryBox: MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<MailmanController>().SetIsInBox(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<MailmanController>().SetIsInBox(false);
            }
        }
    }
}