using IKUtils;
using UnityEngine;

namespace DefaultNamespace
{
    public class MailmanIKController: MonoBehaviour
    {
        [Header("Hands")] 
        public IKConfig LeftHandConfig;
        public IKConfig RightHandConfig;

        [Header("Foots")]
        public IKConfig LeftFootConfig;
        public IKConfig RightFootConfig;
    
        public bool IsIKActive;
        public Animator animator;
        
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

        public void DisableIK()
        {
            LeftHandConfig.isEnabled = false;
            RightHandConfig.isEnabled = false;
        }
        
        public void EnableIK()
        {
            LeftHandConfig.isEnabled = true;
            RightHandConfig.isEnabled = true;
        }
    }
}