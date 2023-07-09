using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
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
    
    public bool isMovingEnabled = false;

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

    public Transform arrow;
    
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
        Sync();
    }

    public void Sync()
    {
        if (!isMovingEnabled) return;

        float delta = Time.fixedDeltaTime;
        var mailManPosition = mailMan.position;
        var currentPosition = transform.position;

        mouseShiftX = Input.GetAxis("Mouse X");
        mouseShiftY = Input.GetAxis("Mouse Y");
        if (mouseShiftX > 10f) mouseShiftX = 10f; // TODO: maybe depends on screen resolution
        if (mouseShiftY > 10f) mouseShiftY = 10f; // TODO: maybe depends on screen resolution
        
        maxMouseShiftY = Mathf.Max(maxMouseShiftY, mouseShiftY);
        trackTime += delta;
        if (trackTime > trackSeconds)
        {
            trackTime = 0f;
            maxMouseShiftY = mouseShiftY;
        }
        
        var translationY = mouseShiftY * speedY * delta;
        yMovement = Mathf.Clamp01(yMovement + translationY);

        var mailManForward = mailMan.forward;
        var leftMostDirection = Quaternion.AngleAxis(-maxAngle, Vector3.up) * mailManForward;
        var rightMostDirection = Quaternion.AngleAxis(maxAngle, Vector3.up) * mailManForward;
        
        var boxForward = transform.forward;


        Debug.DrawRay(mailManPosition, leftMostDirection * 5, Color.red);
        Debug.DrawRay(mailManPosition, rightMostDirection * 5, Color.green);
        Debug.DrawRay(mailManPosition, boxForward * 5, Color.yellow);
        
        this.leftAngle = Vector3.SignedAngle(Vector3.forward, leftMostDirection, Vector3.up);
        float currentAngle = Vector3.SignedAngle(Vector3.forward, boxForward, Vector3.up);
        var deltaAngle = angleSpeed * mouseShiftX * delta;
        if (deltaAngle > maxAngle * 2) deltaAngle = maxAngle * 2;
        
        var leftAngleObj = new Angle(leftAngle);
        var rightAngleObj = leftAngleObj.plus(maxAngle * 2f);
        
        var currentAngleObj = new Angle(currentAngle);
        var targetAngleObj = currentAngleObj.plus(deltaAngle);

        if (targetAngleObj.IsBetween(leftAngleObj, rightAngleObj))
        {
            this.targetAngle = targetAngleObj.rawAngle;
        }
        else
        {
            this.targetAngle = targetAngleObj.StickTo(leftAngleObj, rightAngleObj).rawAngle;
        }
        
        SetPosition();
        
        // var targetDirection = Quaternion.AngleAxis(targetAngle, Vector3.up) * Vector3.forward;
        // currentPosition = mailManPosition + targetDirection * radius;
        //
        // yMovement = Mathf.Clamp01(yMovement + translationY);
        // currentPosition.y = mailManPosition.y + Mathf.Lerp(startPos.y, topPosition.y, yMovement);
        //
        // transform.position = currentPosition;
        //
        // var mailmanPoxXZ = mailManPosition.Flatten();
        // var boxPosition = transform.position;
        // var boxPosXZ = boxPosition.Flatten();
        // var direction = (boxPosXZ - mailmanPoxXZ).normalized;
        // float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        // transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public void SetPosition()
    {
        var currentPosition = transform.position;
        var mailManPosition = mailMan.position;

        var targetDirection = Quaternion.AngleAxis(targetAngle, Vector3.up) * Vector3.forward;
        currentPosition = mailManPosition + targetDirection * radius;

        // yMovement = Mathf.Clamp01(yMovement + translationY);
        currentPosition.y = mailManPosition.y + Mathf.Lerp(startPos.y, topPosition.y, yMovement);
        
        transform.position = currentPosition;
        
        var mailmanPoxXZ = mailManPosition.Flatten();
        var boxPosition = transform.position;
        var boxPosXZ = boxPosition.Flatten();
        var direction = (boxPosXZ - mailmanPoxXZ).normalized;
        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        var deliveryBoxTransform = GameManager.Instanse.deliveryBox.transform;
        var deliveryBoxPosition = deliveryBoxTransform.position.Flatten();
        var deliveryBoxDirection = (deliveryBoxPosition - boxPosXZ).normalized;
        var deliveryBoxAngle = Vector3.SignedAngle(Vector3.forward, deliveryBoxDirection, Vector3.up);  
        arrow.rotation = Quaternion.Euler(0f, deliveryBoxAngle, 0f);
    }

    public void EnableMoving()
    {
        isMovingEnabled = true;
    }

    public void DisableMoving()
    {
        isMovingEnabled = false;
    }
}
