using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utils
{
    public class Follow: MonoBehaviour
    {
        
        public Transform target;

        public bool syncRotation = true;
        [ShowIf("syncRotation")]
        public Transform rotationTarget;

        private void Update()
        {
            transform.position = target.position;
            
            if (syncRotation)
            {
                transform.rotation = rotationTarget.rotation;
            }
            
        }
    }
}