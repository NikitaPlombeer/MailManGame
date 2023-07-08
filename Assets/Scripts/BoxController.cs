using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

public class BoxController : MonoBehaviour
{
    public float angleSpeed = 30f;
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

    [BoxGroup("Settings")] 
    public float maxAngle;

    
    [BoxGroup("Debug")] 
    [ReadOnly] 
    public float targetAngle;
    
    [BoxGroup("Debug")] 
    [ReadOnly] 
    public float leftAngle;
        
    [BoxGroup("Debug")] 
    [ReadOnly] 
    public float rightAngle;
    
    [BoxGroup("Debug")] 
    [ReadOnly] 
    public float leftAngleDiff;
    
    [BoxGroup("Debug")]
    [ReadOnly]
    public float rightAngleDiff;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        radius = startPos.z - mailMan.position.z;
    }

    public float trackSeconds = 0.5f;
    public float trackTime = 0f;
    public float maxMouseShiftY = -100f;

    // Update is called once per frame

    void LateUpdate()
    {
        if (!isMovingEnabled) return;

        var mailManPosition = mailMan.position;
        var currentPosition = transform.position;

        mouseShiftX = Input.GetAxis("Mouse X");
        mouseShiftY = Input.GetAxis("Mouse Y");
        if (mouseShiftX > 10f) mouseShiftX = 10f; // TODO: maybe depends on screen resolution
        
        maxMouseShiftY = Mathf.Max(maxMouseShiftY, mouseShiftY);
        trackTime += Time.deltaTime;
        if (trackTime > trackSeconds)
        {
            trackTime = 0f;
            maxMouseShiftY = mouseShiftY;
        }
        
        var translationX = mouseShiftX * speedX * Time.deltaTime;
        var translationY = mouseShiftY * speedY * Time.deltaTime;

        var mailManForward = mailMan.forward;
        var leftMostDirection = Quaternion.AngleAxis(-maxAngle, Vector3.up) * mailManForward;
        var rightMostDirection = Quaternion.AngleAxis(maxAngle, Vector3.up) * mailManForward;
        
        var boxForward = transform.forward;


        Debug.DrawRay(mailManPosition, leftMostDirection * 5, Color.red);
        Debug.DrawRay(mailManPosition, rightMostDirection * 5, Color.green);
        Debug.DrawRay(mailManPosition, boxForward * 5, Color.yellow);
        
        this.leftAngle = Vector3.SignedAngle(Vector3.forward, leftMostDirection, Vector3.up);
        if (leftAngle < 0) leftAngle += 360;
        this.rightAngle = leftAngle + maxAngle * 2f;
        
        var deltaAngle = angleSpeed * mouseShiftX * Time.deltaTime;
        if (deltaAngle > maxAngle * 2) deltaAngle = maxAngle * 2;
        
        float currentAngle = Vector3.SignedAngle(Vector3.forward, boxForward, Vector3.up);
        if (currentAngle < 0) currentAngle += 360;
        if (rightAngle > 180f && currentAngle < 180f) currentAngle += 360f;
  
        if (currentAngle > 180f && leftAngle < 180f)
        {
            leftAngle += 360f;
            rightAngle += 360f;
        }
        
        leftAngleDiff = Mathf.Abs(currentAngle - leftAngle);
        rightAngleDiff = Mathf.Abs(currentAngle - rightAngle);

        
        this.targetAngle = currentAngle + deltaAngle;
        if (targetAngle < leftAngle || targetAngle > rightAngle)
        {
            if (leftAngleDiff < rightAngleDiff)
            {
                targetAngle = leftAngle;
                // Debug.Log($"Force left leftAngleDiff={leftAngleDiff} rightAngleDiff={rightAngleDiff} targetAngle={targetAngle} currentAngle={currentAngle} leftAngle={leftAngle} rightAngle={rightAngle}");
            }
            else
            {
                targetAngle = rightAngle;
                // Debug.Log($"Force right leftAngleDiff={leftAngleDiff} rightAngleDiff={rightAngleDiff} targetAngle={targetAngle} currentAngle={currentAngle} leftAngle={leftAngle} rightAngle={rightAngle}");
            }
        }

        var targetDirection = Quaternion.AngleAxis(targetAngle, Vector3.up) * Vector3.forward;
        currentPosition = mailManPosition + targetDirection * radius;

        yMovement = Mathf.Clamp01(yMovement + translationY);
        currentPosition.y = mailManPosition.y + Mathf.Lerp(startPos.y, topPosition.y, yMovement);
        
        transform.position = currentPosition;
        
        var mailmanPoxXZ = mailManPosition.Flatten();
        var boxPosition = transform.position;
        var boxPosXZ = boxPosition.Flatten();
        var direction = (boxPosXZ - mailmanPoxXZ).normalized;
        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
}
