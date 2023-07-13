using System;
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
    public bool isMoving = false;
    
    [BoxGroup("Movement")]
    public float baseSpeed = 4;

    [BoxGroup("Movement")]
    [ReadOnly]
    public float speed = 4;
    
    [BoxGroup("Movement")]
    public float baseJumpSpeed = 5;
    
    [BoxGroup("Movement")]
    [ReadOnly]
    public float jumpSpeed = 5;
    
    public float speedUpTime = 5f;
    
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

    [BoxGroup("Buffs")]
    public float speedUpMultiplier = 2f;
    [BoxGroup("Buffs")]
    public float jumpUpMultiplier = 2f;
    [BoxGroup("Buffs")]
    public float speedDownMultiplier = 0.8f;
    [BoxGroup("Buffs")]
    public float jumpDownMultiplier = 0.8f;

    
    [BoxGroup("Debug")]
    [ReadOnly]
    public float angle;

    private Transform box;

    public BuffEffectUI buffEffectUI;

    public bool isInBox = false;
    
    private void Start()
    {
        box = boxController.transform;
        StartCoroutine(trackRotation());
        
        jumpSpeed = baseJumpSpeed;
        speed = baseSpeed;
    }

    private void Update()
    {
        if (!isMoving) return;
        
        if (isInBox && Input.GetKeyDown(KeyCode.Space))
        {
            this.isMoving = false;
            rb.velocity = Vector3.up * ragdollSpeed + Vector3.forward * ragdollSpeed;
            boxController.isMovingEnabled = false;
            Invoke(nameof(EnableRagdoll), 0.2f);
            EnableRagdoll();
            GameManager.Instanse.Delivered();
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
        

        var translation = direction * (speed * 1f) * Time.fixedDeltaTime;
        // rb.MovePosition(transform.position + translation);
        
        IkHands.position = Vector3.Lerp(IkHands.position, boxPosition, 0.2f);
        IkHands.rotation = Quaternion.Lerp(IkHands.rotation,  Quaternion.LookRotation(box.forward), 0.2f);
        
        var velocityY = rb.velocity.y;
        rb.velocity = (translation / Time.fixedDeltaTime).Flatten(velocityY);
        // boxController.SetPosition();
        // boxPosition += translation;
        // box.position = boxPosition;
        // boxController.rb.MovePosition(boxPosition); //velocity = (translation / Time.deltaTime);
        
        // ragdollController.ikController.Sync();
        angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        animationMovement = Mathf.Clamp01((angle / boxController.maxAngle + 1f) / 2f);
        animator.SetFloat(MovementAnimationKey, animationMovement);
    }

    private bool IsObstacleInFront()
    {
        var humanTransform = transform;
        var ray = new Ray(humanTransform.position, humanTransform.forward);
        return Physics.Raycast(ray, out var hit, 1f);
    }

    public IEnumerator trackRotation()
    {
        while (true)
        {
            if (!isMoving) yield return null;
            
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
    }

    private Coroutine ResetSpeedCorountine = null;
    
    public void UpdateSpeed(bool isSpeedUp)
    {
        if (buffEffectUI != null)
        {
            if (isSpeedUp)
            {
                buffEffectUI.SetSpeed();
            } else {
                buffEffectUI.SetSlow();
            }
        }
        speed = isSpeedUp ? baseSpeed * speedUpMultiplier : baseSpeed * speedDownMultiplier;
        jumpSpeed = isSpeedUp ? baseJumpSpeed * jumpUpMultiplier  : baseSpeed * jumpDownMultiplier;
        
        if (ResetSpeedCorountine != null)
        {
            StopCoroutine(ResetSpeedCorountine);
        }
        ResetSpeedCorountine = StartCoroutine(ResetSpeedAsync());
    }
    
    
    public IEnumerator ResetSpeedAsync()
    {
        yield return new WaitForSeconds(speedUpTime);
        ResetSpeed();
    }
    
    private void ResetSpeed()
    {
        speed = baseSpeed;
        jumpSpeed = baseJumpSpeed;
        if (buffEffectUI != null)
        {
            buffEffectUI.SetDefault();
        }
    }

    public void SetIsInBox(bool isInBox)
    {
        this.isInBox = isInBox;
        if (isMoving)
        {
            GameUI.Instanse.SetVisibleForDeliverLabel(isInBox);
        }
    }

    public void EnableMoving()
    {
        isMoving = true;
        animator.SetBool("Walk", true);
        
        ragdollController.DisableRagdoll();
        rb.isKinematic = false;
        
        ragdollController.ikController.LeftHandConfig.isEnabled = true;
        ragdollController.ikController.RightHandConfig.isEnabled = true;
    }

    public void DisableMoving()
    {
        isMoving = false;
        animator.SetBool("Walk", false);

        rb.isKinematic = true;

        ragdollController.ikController.LeftHandConfig.isEnabled = false;
        ragdollController.ikController.RightHandConfig.isEnabled = false;
    }
}
