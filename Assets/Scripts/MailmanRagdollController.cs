using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class MailmanRagdollController: MonoBehaviour
    {
        
        public List<Collider> ragdollColliders;
        public Animator animator;
        public MailmanIKController ikController;
        public Collider playmodeCollider;
        
        private void Start()
        {
            DisableRagdoll();
        }

        public void EnableRagdoll()
        {
            animator.enabled = false;
            foreach (Collider ragdollCollider in ragdollColliders)
            {
                ragdollCollider.enabled = true;
                ragdollCollider.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
            ikController.DisableIK();
        }
        
        public void DisableRagdoll()
        {
            animator.enabled = true;
            foreach (Collider ragdollCollider in ragdollColliders)
            {
                ragdollCollider.enabled = false;
                ragdollCollider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            
            ikController.EnableIK();
        }
    }
}