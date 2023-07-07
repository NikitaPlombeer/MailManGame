using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class FaceUI: MonoBehaviour
    {
        private Camera mainCamera;
        public bool changeYRotation = true;
        public GameObject faceObject;
        
        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            Vector3 lookForward = mainCamera.transform.forward;
            if (!changeYRotation)
            {
                lookForward.y = 0;
            }
            faceObject.transform.rotation = Quaternion.LookRotation(lookForward, Vector3.up);
        }
    }
}
