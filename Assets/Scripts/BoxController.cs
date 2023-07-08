using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class BoxController : MonoBehaviour
{
    public float speedX = 1f;
    public float speedY = 1f;
    public float radius;
    
    public Vector3 startPos;
    public Vector3 lastMousePosition;
    public float mouseShiftX;
    public float mouseShiftY;


    public Transform leftHandIKPoint;
    public Transform rightHandIKPoint;

    public float boxMinX = -0.25f;
    public float boxMaxX = 0.25f;

    public Transform mailMan;

    public Vector3 topPosition = new Vector3(0f, 2.15f, 0.45f);

    public float yMovement = 0f;
    
    public bool isMovingEnabled = true;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    public float trackSeconds = 0.5f;
    public float trackTime = 0f;
    public float maxMouseShiftY = -100f;
    // Update is called once per frame
    void LateUpdate()
    {
        if (!isMovingEnabled) return;
        // mouseShift = Input.mousePosition - lastMousePosition;
        // lastMousePosition = Input.mousePosition;

        var mailManPosition = mailMan.position;
        var currentPosition = transform.position;

        // var mouseShiftX = mouseShift.x;
        mouseShiftX = Input.GetAxis("Mouse X");
        mouseShiftY = Input.GetAxis("Mouse Y");
        
        maxMouseShiftY = Mathf.Max(maxMouseShiftY, mouseShiftY);
        trackTime += Time.deltaTime;
        if (trackTime > trackSeconds)
        {
            trackTime = 0f;
            if (maxMouseShiftY > 0f)
            {
                // Debug.Log("Current maxMouseShiftY: " + maxMouseShiftY);
            }
            maxMouseShiftY = -100f;
  
        }
        
        var translationX = mouseShiftX * speedX * Time.deltaTime;
        var translationY = mouseShiftY * speedY * Time.deltaTime;
        currentPosition.x = Mathf.Clamp( currentPosition.x + translationX, mailManPosition.x + boxMinX, mailManPosition.x + boxMaxX);

        yMovement = Mathf.Clamp01(yMovement + translationY);
        var startPoxYZ = new Vector3(0f, startPos.y, startPos.z);
        var lerp = Vector3.Lerp(startPoxYZ, topPosition, yMovement);
        currentPosition.y = mailManPosition.y + lerp.y;
        currentPosition.z = mailManPosition.z + lerp.z;
        transform.position = currentPosition;
        
        var mailmanPoxXZ = mailManPosition.Flatten();
        var boxPosition = transform.position;
        var boxPosXZ = boxPosition.Flatten();
        var direction = (boxPosXZ - mailmanPoxXZ).normalized;
        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
}
