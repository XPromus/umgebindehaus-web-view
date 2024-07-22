using System;
using System.Collections.Generic;
using Lean.Touch;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

public class SortDoorComponents
{

    private const string TextSeperator = "_";

    public void SortObjectsInDoor(GameObject doorParent)
    {
        var movingPartChildren = new List<Transform>();
        var frameChildren = new List<Transform>();

        Tuple<bool, Transform> doorPivot = GetDoorPivot(doorParent);
        if (!doorPivot.Item1) return;
        
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
        }
        
        var movingPartParent = new GameObject
        {
            name = "Moving Part",
            transform =
            {
                parent = doorParent.transform,
                position = GetObjectCenter(doorPivot.Item2)
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
            
        doorPivot.Item2.gameObject.SetActive(false);
        
        AddDoorFunctionality(doorParent, movingPartParent);
    }

    private void AddChildrenToParent(GameObject parent, List<Transform> children)
    {
        foreach (var child in children)
        {
            //child.GetComponent<MeshCollider>().enabled = false;
            child.parent = parent.transform;
        }
    }

    private Transform GetPivotPointPosition()
    {
        return new RectTransform();
    }

    private Tuple<bool, Transform> GetDoorPivot(GameObject doorParent)
    {
        for (var i = 0; i < doorParent.transform.childCount; i++)
        {
            var currentChild = doorParent.transform.GetChild(i);
            var splitName = currentChild.name.Split("_");
            if (splitName.Length < 4) continue;
            if (splitName[3].Equals("Drehpunkt"))
            {
                return new Tuple<bool, Transform>(true, currentChild);
            }
        }

        return new Tuple<bool, Transform>(false, null);
    }

    private void AddDoorFunctionality(GameObject doorParent, GameObject pivot)
    {
        var controller = doorParent.AddComponent<DoorController>();
        controller.DoorObject = pivot;
        controller.OpeningDirection = DoorController.OpeningDirectionEnum.OUTWARDS;
        controller.DoorOpenAngle = 90f;
        controller.OpeningTime = 3f;
        controller.ShowDebugUI = true;

        UnityAction<GameObject> action = controller.UseDoor;
        var selectable = pivot.AddComponent<LeanSelectableByFinger>();
        UnityEventTools.AddObjectPersistentListener(selectable.OnSelected, action, pivot);
        UnityEventTools.AddObjectPersistentListener(selectable.OnDeselected, action, pivot);

        var doorCollider = pivot.AddComponent<BoxCollider>();
        doorCollider.center = new Vector3(0f, 0f, 0f);
        doorCollider.size = new Vector3(0f, 0f, 0f);

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
        var splitText = text.Split("_");
        if (splitText.Length <= 3)
        {
            return true;
        }

        if (splitText.Length > 3 && splitText[3].Equals("Scharnier"))
        {
            return true;
        }
        return false;
    }

    private enum DoorParentComponent
    {
        MovingPart,
        Frame,
    }
    
}
