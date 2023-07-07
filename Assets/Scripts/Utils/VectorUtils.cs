using UnityEngine;

namespace Utils
{
    public static class VectorUtils
    {
        public static Vector3 Flatten(this Vector3 vector, float y = 0)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        public static Vector3 Round(this Vector3 vector, int digits = 2)
        {
            return new Vector3(
                (float) System.Math.Round(vector.x, digits), 
                (float) System.Math.Round(vector.y, digits),
                (float) System.Math.Round(vector.z, digits)
            );
        }

        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector3 AddY(this Vector3 vector, float yOffset)
        {
            return new Vector3(vector.x, vector.y + yOffset, vector.z);
        }

        public static Vector2[] RotateVectors(Vector2[] vertices, float angle)
        {
            Vector2 center = GetCenter(vertices);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] -= center;
                vertices[i] = rotation * vertices[i];
                vertices[i] += center;
            }

            return vertices;
        }

        public static Vector3[] RotateVectors(Vector3[] vertices, float angle)
        {
            Vector3 center = GetCenter(vertices);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] -= center;
                vertices[i] = rotation * vertices[i];
                vertices[i] += center;
            }

            return vertices;
        }

        private static Vector2 GetCenter(Vector2[] vertices)
        {
            Vector2 center = Vector2.zero;
            foreach (Vector2 vertex in vertices)
            {
                center += vertex;
            }

            center /= vertices.Length;
            return center;
        }

        private static Vector3 GetCenter(Vector3[] vertices)
        {
            Vector3 center = Vector2.zero;
            foreach (Vector3 vertex in vertices)
            {
                center += vertex;
            }

            center /= vertices.Length;
            return center;
        }
    }
}