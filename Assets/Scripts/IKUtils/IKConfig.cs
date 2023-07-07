using System;
using UnityEngine;

namespace IKUtils
{
    [Serializable]
    public struct IKConfig
    {
        public IKWeight weight;
        public Transform target;
        public bool isEnabled;
    }
    
    [Serializable]
    public struct IKWeight
    {
        [Range(0, 1)] public float positionWeight;
        [Range(0, 1)] public float rotationWeight;

        public IKWeight(float weight)
        {
            positionWeight = weight;
            rotationWeight = weight;
        }
        
        public IKWeight(float positionWeight, float rotationWeight)
        {
            this.positionWeight = positionWeight;
            this.rotationWeight = rotationWeight;
        }
    }
}