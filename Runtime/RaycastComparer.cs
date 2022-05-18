using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Futurus.RemoteInput
{
    public static class RaycastComparer
    {
        // Copied from UnityEngine.EventSystems.EventSystem
        // I, like you dear programmer, am surprised that some of the EventSystem namespace isn't compiled...but also 
        // is still needlessly private or internal? For the life of me I won't understand why so many low-level unity systems are 
        // largely hidden from users. Just let me directly ref these things unity !!!!
        // Use as follows
        // static readonly Comparison<RaycastResult> RaycastComparer = UIHelpers.RaycastComparer;
        public static int Compare(RaycastResult lhs, RaycastResult rhs)
        {
            if (lhs.module != rhs.module)
            {
                Camera eventCamera = lhs.module.eventCamera;
                Camera eventCamera2 = rhs.module.eventCamera;
                if (eventCamera != null && eventCamera2 != null && eventCamera.depth != eventCamera2.depth)
                {
                    if (eventCamera.depth < eventCamera2.depth)
                    {
                        return 1;
                    }

                    if (eventCamera.depth == eventCamera2.depth)
                    {
                        return 0;
                    }

                    return -1;
                }

                if (lhs.module.sortOrderPriority != rhs.module.sortOrderPriority)
                {
                    return rhs.module.sortOrderPriority.CompareTo(lhs.module.sortOrderPriority);
                }

                if (lhs.module.renderOrderPriority != rhs.module.renderOrderPriority)
                {
                    return rhs.module.renderOrderPriority.CompareTo(lhs.module.renderOrderPriority);
                }
            }

            if (lhs.sortingLayer != rhs.sortingLayer)
            {
                int layerValueFromID = SortingLayer.GetLayerValueFromID(rhs.sortingLayer);
                int layerValueFromID2 = SortingLayer.GetLayerValueFromID(lhs.sortingLayer);
                return layerValueFromID.CompareTo(layerValueFromID2);
            }

            if (lhs.sortingOrder != rhs.sortingOrder)
            {
                return rhs.sortingOrder.CompareTo(lhs.sortingOrder);
            }

            if (lhs.depth != rhs.depth && lhs.module.rootRaycaster == rhs.module.rootRaycaster)
            {
                return rhs.depth.CompareTo(lhs.depth);
            }

            if (lhs.distance != rhs.distance)
            {
                return lhs.distance.CompareTo(rhs.distance);
            }

            return lhs.index.CompareTo(rhs.index);
        }
    }
}