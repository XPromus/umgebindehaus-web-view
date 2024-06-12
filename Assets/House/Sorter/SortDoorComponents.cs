using System;
using System.Collections.Generic;
using UnityEngine;

public class SortDoorComponents : MonoBehaviour
{

    private const string TextSeperator = "_";

    public void SortObjectsInDoor(GameObject doorParent)
    {
        var movingPartChildren = new List<Transform>();
        var frameChildren = new List<Transform>();

        Transform doorPivot = null;

        for (var i = 0; i < doorParent.transform.childCount; i++)
        {
            var child = doorParent.transform.GetChild(i);
            var doorPartValue = CalcElementPlacementInDoor(child);
            
            switch(doorPartValue)
            {
                case DoorParentComponent.Frame:
                    frameChildren.Add(child);
                    break;
                
                case DoorParentComponent.MovingPart:
                    movingPartChildren.Add(child);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var movingPartParent = new GameObject
            {
                name = "Moving Part",
                transform =
                {
                    parent = doorParent.transform,
                    position = GetObjectCenter(doorPivot!)
                }
            };

            var doorFrameParent = new GameObject
            {
                name = "Door Frame",
                transform =
                {
                    parent = doorParent.transform
                }
            };
            
            AddChildrenToParent(doorFrameParent, frameChildren);
            AddChildrenToParent(movingPartParent, movingPartChildren);
            
            doorPivot.gameObject.SetActive(false);
            
        }
    }

    private void AddChildrenToParent(GameObject parent, List<Transform> children)
    {
        foreach (var child in children)
        {
            child.GetComponent<MeshCollider>().enabled = false;
            child.parent = parent.transform;
        }
    }

    private void AddDoorFunctionality()
    {
        
    }

    private Vector3 GetObjectCenter(Transform target)
    {
        return target.gameObject.GetComponent<Renderer>().bounds.center;
    }

    private DoorParentComponent CalcElementPlacementInDoor(Transform element)
    {
        var objectName = element.gameObject.name;
        if (CheckTextForMovingPart(objectName))
        {
            Debug.Log("Found moving part: " + objectName);
            return DoorParentComponent.MovingPart;
        }

        Debug.Log("Found frame: " + objectName);
        return DoorParentComponent.Frame;
    }

    private bool CheckTextForMovingPart(string text)
    {
        return !text.Contains("Zarge");
    }

    private enum DoorParentComponent
    {
        MovingPart,
        Frame,
    }
    
}
