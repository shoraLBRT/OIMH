using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CogsAndGoggles.Library.Extensions
{
    public static class TransformExtensions
    {
        public static Transform Clear(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }

            return transform;
        }
        
        public static Transform FirstChildOrDefault(this Transform parent, Func<Transform, bool> query)
        {
            if (parent.childCount == 0)
            {
                return null;
            }

            Transform result = null;
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (query(child))
                {
                    return child;
                }
                result = FirstChildOrDefault(child, query);
                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

    }
}