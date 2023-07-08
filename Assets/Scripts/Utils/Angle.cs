using UnityEngine;

namespace Utils
{
    public struct Angle
    {

        public float rawAngle;

        public Angle(float rawAngle)
        {
            this.rawAngle = NormalizeAngle(rawAngle);
        }
        
        
        public Angle plus(float angle)
        {
            return new Angle(rawAngle + angle);
        }
        
        public bool IsBetween(Angle a, Angle b)
        {
            // Normalize all the angles to be within [0, 360).
            float angleA = NormalizeAngle(a.rawAngle);
            float angleB = NormalizeAngle(b.rawAngle);
            float currentAngle = NormalizeAngle(this.rawAngle);

            if (angleA <= angleB)
            {
                // If a <= b, we can directly check if currentAngle lies between a and b.
                return currentAngle >= angleA && currentAngle <= angleB;
            }
            else
            {
                // If a > b, currentAngle lies between a and b if it's greater than a
                // or less than b due to wrapping around 360 degrees.
                return currentAngle >= angleA || currentAngle <= angleB;
            }
        }
        
        public bool isLessThan(Angle other)
        {
            return this.rawAngle < other.rawAngle;
        }
        
        public bool isGreaterThan(Angle other)
        {
            return this.rawAngle > other.rawAngle;
        }
        
        private static float NormalizeAngle(float angle)
        {
            // The angle may be less than -360 or more than 360.
            angle = angle % 360;

            // Normalize negative angles to be within [0, 360).
            if (angle < 0)
            {
                angle += 360;
            }

            return angle;
        }


        public override string ToString()
        {
            return $"Angle({rawAngle})";
        }

        public Angle StickTo(Angle leftAngleObj, Angle rightAngleObj)
        {
         
            if (Difference(leftAngleObj) < Difference(rightAngleObj))
            {
                return new Angle(leftAngleObj.rawAngle);
            }
            else
            {
                return new Angle(rightAngleObj.rawAngle);
            }
        }
        
        
       public float Difference(Angle other)
        {
            float diff = Mathf.Abs(this.rawAngle - other.rawAngle);

            // If the difference is greater than 180, then it is faster to go the other way
            if (diff > 180)
            {
                diff = 360 - diff;
            }

            return diff;
        }
        
    }
}