using System.Collections;
using DefaultNamespace;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

public class MailmanController : MonoBehaviour
{
    private static readonly int MovementAnimationKey = Animator.StringToHash("Movement");

    [BoxGroup("Dependencies")]
    public Animator animator;
    
    [BoxGroup("Dependencies")]
    public Rigidbody rb;
    
    [BoxGroup("Dependencies")]
    public BoxController boxController;
    
    [BoxGroup("Dependencies")]
    public MailmanRagdollController ragdollController;

    
    [BoxGroup("Movement")]
    [ReadOnly]
    public bool isMoving = true;
    
    [BoxGroup("Movement")]
    public float speed = 3;
    
    [BoxGroup("Movement")]
    public float jumpSpeed = 5;
    
    [BoxGroup("Movement")]
    public float animationMovement = 0.5f;
    
    [BoxGroup("Movement")]
    public float mouseShiftToJump = 10f;
    
    [BoxGroup("Movement")]
    public float ragdollSpeed = 10f;

    [BoxGroup("Movement")]
    public float rotationSyncTime = 1f;
    
    [BoxGroup("Movement")]
    [ReadOnly]
    public bool isJumpOnCooldown = false;

   
    
    [BoxGroup("Debug")]
    [ReadOnly]
    public float angle;

    private Transform box;
    
    
    private void Start()
    {
        box = boxController.transform;
        StartCoroutine(trackRotation());
    }

    private void Update()
    {
        if (!isMoving) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.isMoving = false;
            rb.velocity = Vector3.up * ragdollSpeed + Vector3.forward * ragdollSpeed;
            boxController.isMovingEnabled = false;
            Invoke(nameof(EnableRagdoll), 0.2f);
            EnableRagdoll();
            return;
        }

        if (!isJumpOnCooldown && boxController.yMovement > 0.9f && boxController.maxMouseShiftY > mouseShiftToJump)
        {
            rb.velocity = Vector3.up * jumpSpeed;
            isJumpOnCooldown = true;
            Invoke(nameof(ResetJumpCooldown), 1f);
        }
    }

    public void EnableRagdoll()
    {
        rb.isKinematic = true;
        ragdollController.EnableRagdoll();
    }
    
    public void ResetJumpCooldown()
    {
        isJumpOnCooldown = false;
    }

    public Transform IkHands;
    
    public Quaternion prevRotation = Quaternion.LookRotation(Vector3.forward);
    private void FixedUpdate()
    {
        if (!isMoving) return;

        var mailmanPoxXZ = transform.position.Flatten();
        var boxPosition = box.position;
        var boxPosXZ = boxPosition.Flatten();
        var direction = (boxPosXZ - mailmanPoxXZ).normalized;
        
        var speedK = Mathf.Clamp(1f - boxController.yMovement, 0.7f, 1f);
        if (isJumpOnCooldown)
        {
            speedK = 1f;
        }
        var translation = direction * (speed * speedK) * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + translation);
        // IkHands.position = boxPosition;
        // IkHands.rotation = Quaternion.LookRotation(box.forward);
        
        IkHands.position = Vector3.Lerp(IkHands.position, boxPosition, 0.2f);
        IkHands.rotation = Quaternion.Lerp(IkHands.rotation,  Quaternion.LookRotation(box.forward), 0.2f);
        
        // SetSmoothPositionForHands(boxPosition, Quaternion.LookRotation(box.forward));
        // var velocityY = rb.velocity.y;
        // rb.velocity = (translation / Time.fixedDeltaTime).Flatten(velocityY);
        // boxController.SetPosition();
        // boxPosition += translation;
        // box.position = boxPosition;
        // boxController.rb.MovePosition(boxPosition); //velocity = (translation / Time.deltaTime);
        
        // ragdollController.ikController.Sync();
        angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        animationMovement = Mathf.Clamp01((angle / boxController.maxAngle + 1f) / 2f);
        animator.SetFloat(MovementAnimationKey, animationMovement);
    }

    public Coroutine SmoothPositionForHandsCoroutine = null;
    
    public void SetSmoothPositionForHands(Vector3 position, Quaternion rotation)
    {
        if (SmoothPositionForHandsCoroutine != null) return;
        Debug.Log("SetSmoothPositionForHands");
        SmoothPositionForHandsCoroutine = StartCoroutine(SetSmoothPositionForHandsAsync(position, rotation));
    }

    public IEnumerator SetSmoothPositionForHandsAsync(Vector3 position, Quaternion rotation)
    {
        float time = 0f;
        var current = IkHands.position;
        var currentRotation = IkHands.rotation;
        while (time < 0.2f)
        {
            var k = time / 0.2f;
            IkHands.position = Vector3.Lerp(current, position, k);
            IkHands.rotation = Quaternion.Lerp(currentRotation, rotation, k);
            yield return null;
         
            // Debug.Log("SetSmoothPositionForHands tick");
            time += Time.deltaTime;
        }
        IkHands.position = position;
        IkHands.rotation = rotation;
        // StopCoroutine(SmoothPositionForHandsCoroutine);
        SmoothPositionForHandsCoroutine = null;
    }

    public IEnumerator trackRotation()
    {
        while (isMoving)
        {
            prevRotation = Quaternion.LookRotation(box.forward);
            var current = rb.rotation;
            float time = 0f;
            while (time < rotationSyncTime)
            {
                var k = time / rotationSyncTime;
                rb.MoveRotation(Quaternion.Lerp(current, prevRotation, k));
                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;
            }
            rb.MoveRotation(prevRotation);
            
        }

        yield return null;
    }

}
