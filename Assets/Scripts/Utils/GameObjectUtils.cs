using UnityEngine;

namespace DwarfTrains.Utils
{
    public static class GameObjectUtils
    {
        public static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj)
            {
                return;
            }
       
            obj.layer = newLayer;
       
            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
        
        public static T[] GetComponentsInDirectChildren<T>(GameObject gameObject)
        {
            int indexer = 0;
 
            foreach (Transform transform in gameObject.transform)
            {
                if (transform.GetComponent<T>() != null)
                {
                    indexer++;
                }
            }
 
            T[] returnArray = new T[indexer];
 
            indexer = 0;
 
            foreach (Transform transform in gameObject.transform)
            {
                if (transform.GetComponent<T>() != null)
                {
                    returnArray[indexer++] = transform.GetComponent<T>();
                }
            }
 
            return returnArray;
        }
        
        public static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount; i > 0; --i)
                Object.DestroyImmediate(parent.GetChild(0).gameObject);
        }
        
    }
}