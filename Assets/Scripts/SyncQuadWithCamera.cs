using UnityEngine;

namespace DefaultNamespace
{
    public class SyncQuadWithCamera : MonoBehaviour
    {
        public Camera mainCamera; // Reference to main camera
        public GameObject quad; // Reference to quad gameobject

        // This should be the local position of the Quad relative to the Camera
        private Vector3 offset = new Vector3(-0.5f, -0.5f, 0);
        public float quadSize = 0.2f; // Size of quad in viewport space
        public Vector3 screenOffset = new Vector3(0, 0, 1);
        
        private void Update()
        {
            SyncPositionAndRotation();
        }

        // This method syncs the Quad's position and rotation with the main camera
        private void SyncPositionAndRotation()
        {
            // Convert the quad's viewport position to a world position
            Vector3 quadPosition = mainCamera.ViewportToWorldPoint(new Vector3(quadSize, quadSize, mainCamera.nearClipPlane) + screenOffset);

            // Set the quad's position
            quad.transform.position = quadPosition;

            // Sync the rotation with the camera
            quad.transform.rotation = mainCamera.transform.rotation;
        }
    }
}