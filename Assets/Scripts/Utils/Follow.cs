using System;
using UnityEngine;

namespace Utils
{
    public class Follow: MonoBehaviour
    {
        
        public Transform target;

        private void Update()
        {
            transform.position = target.position;
        }
    }
}