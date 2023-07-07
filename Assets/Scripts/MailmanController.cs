using System;
using System.Collections;
using System.Collections.Generic;
using IKUtils;
using UnityEngine;
using Utils;

public class MailmanController : MonoBehaviour
{
    
    [Header("Hands")] 
    public IKConfig LeftHandConfig;
    public IKConfig RightHandConfig;

    [Header("Foots")]
    public IKConfig LeftFootConfig;
    public IKConfig RightFootConfig;
    
    public bool IsIKActive;
    public Animator animator;

    public float speed = 3;
    public float jumpSpeed = 5;
    public Transform box;
    public float animationMovement = 0.5f;
    public float angle;
    public Rigidbody rb;

    private BoxController boxController;

    public float mouseShiftToJump = 10f;
    private void Start()
    {
        boxController = FindObjectOfType<BoxController>();
    }

    public bool isJumpOnCooldown = false;
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     rb.velocity = Vector3.up * jumpSpeed;
        // }

        if (!isJumpOnCooldown && boxController.yMovement > 0.9f && boxController.maxMouseShiftY > mouseShiftToJump)
        {
            rb.velocity = Vector3.up * jumpSpeed;
            isJumpOnCooldown = true;
            Invoke(nameof(ResetJumpCooldown), 1f);
        }

        // var mailmanPoxXZ = transform.position.Flatten();
        // var boxPosXZ = box.position.Flatten();
        // var direction = (boxPosXZ - mailmanPoxXZ).normalized;
        //
        // // var currentPosition = transform.position;
        // var translation = direction * speed * Time.deltaTime;
        // rb.MovePosition(transform.position + translation);
        //
        // // currentPosition += translation;
        // // transform.position = currentPosition;
        // box.position = box.position + translation;
        //
        // angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        // animationMovement = Mathf.Clamp01((angle / 26f + 1f) / 2f);
        // animator.SetFloat("Movement", animationMovement);
    }
    
    public void ResetJumpCooldown()
    {
        isJumpOnCooldown = false;
    }

    private void FixedUpdate()
    {
        var mailmanPoxXZ = transform.position.Flatten();
        var boxPosXZ = box.position.Flatten();
        var direction = (boxPosXZ - mailmanPoxXZ).normalized;
        
        // var currentPosition = transform.position;
        var translation = direction * speed * Time.deltaTime;
        rb.MovePosition(transform.position + translation);
        
        // currentPosition += translation;
        // transform.position = currentPosition;
        box.position = box.position + translation;

        angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        animationMovement = Mathf.Clamp01((angle / 26f + 1f) / 2f);
        animator.SetFloat("Movement", animationMovement);
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (IsIKActive)
        {
            HandleConfig(AvatarIKGoal.LeftHand, LeftHandConfig);
            HandleConfig(AvatarIKGoal.RightHand, RightHandConfig);
            HandleConfig(AvatarIKGoal.LeftFoot, LeftFootConfig);
            HandleConfig(AvatarIKGoal.RightFoot, RightFootConfig);
        }
        else
        {
            SetIKWeights(AvatarIKGoal.LeftHand, new IKWeight(0f));
            SetIKWeights(AvatarIKGoal.RightHand, new IKWeight(0f));
            SetIKWeights(AvatarIKGoal.LeftFoot, new IKWeight(0f));
            SetIKWeights(AvatarIKGoal.RightFoot, new IKWeight(0f));
            
            animator.SetLookAtWeight(0);
        }
    }

    private void HandleConfig(AvatarIKGoal goal, IKConfig config)
    {
        if (config.isEnabled && config.target != null)
        {
            SetIKWeights(goal, config.weight);
            SetIKTarget(goal, config.target);
        }
        else
        {
            SetIKWeights(goal, new IKWeight(0f));
        }
    }

    private void SetIKWeights(AvatarIKGoal goal, IKWeight weight)
    {
        animator.SetIKPositionWeight(goal, weight.positionWeight);
        animator.SetIKRotationWeight(goal, weight.rotationWeight);
    }
        
    private void SetIKTarget(AvatarIKGoal goal, Transform target)
    {
        animator.SetIKPosition(goal, target.position);
        animator.SetIKRotation(goal, target.rotation);
    }
}
