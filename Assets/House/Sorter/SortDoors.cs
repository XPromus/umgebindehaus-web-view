using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class SortDoors : MonoBehaviour
{

    [Header("Settings")] 
    [SerializeField] private int[] doorsToSort;
    [SerializeField] private bool invertDoorsToSortList;
    [SerializeField] private string doorParentObjectName;
    private bool sortAllDoors;

    public void SortSelectedDoors(GameObject targetHouse)
    {
        var doorParents = GetDoorParentList(targetHouse.transform);
        var doorsToSortLists = new List<List<DoorToSort>>();
        foreach (var doorParent in doorParents)
        {
            doorsToSortLists.Add(CreateDoorGroups(doorParent));
        }

        foreach (var list in doorsToSortLists)
        {
            var parentObjects = SortObjectsInScene(list);
            var sortDoorComponents = new SortDoorComponents();
            foreach (var doorParent in parentObjects)
            {
                sortDoorComponents.SortObjectsInDoor(doorParent);
            }
        }
    }

    private List<GameObject> SortObjectsInScene(List<DoorToSort> list)
    {
        var returnList = new List<GameObject>();
        foreach (var doorToSort in list)
        {
            var newObject = new GameObject
            {
                name = doorToSort.DoorName.ToString(),
                transform =
                {
                    parent = doorToSort.Parent
                }
            };
            foreach (var doorPart in doorToSort.DoorParts)
            {
                doorPart.parent = newObject.transform;
            }
            returnList.Add(newObject);
        }

        return returnList;
    }

    private List<DoorToSort> CreateDoorGroups(Transform parent)
    {
        var doorsToSortList = new List<DoorToSort>();
        parent.gameObject.layer = LayerMask.NameToLayer("Doors");
        for (var i = 0; i < parent.childCount; i++)
        {
            var currentChild = parent.GetChild(i);
            var splitName = currentChild.name.Split("_");
            var doorNumber = Int32.Parse(splitName[1]);

            switch (invertDoorsToSortList)
            {
                case true:
                    if (ArrayDoesNotContain(doorsToSort, doorNumber))
                    {
                        StartSort(doorsToSortList, doorNumber, currentChild, parent);
                    }

                    break;
                
                case false:
                    if (!invertDoorsToSortList && ArrayContains(doorsToSort, doorNumber))
                    {
                        StartSort(doorsToSortList, doorNumber, currentChild, parent);
                    }

                    break;
            }
            
        }

        return doorsToSortList;
    }

    private void StartSort(
        List<DoorToSort> doorsToSortList,
        int doorNumber,
        Transform currentChild,
        Transform parent
        )
    {
        var doorSortListCheck = DoorListContains(doorsToSortList, doorNumber);
        if (doorSortListCheck.Item1)
        {
            doorSortListCheck.Item2.AddElementToList(currentChild);
        }
        else
        {
            var newDoorSortObject = new DoorToSort
            {
                Parent = parent,
                DoorName = doorNumber
            };
            newDoorSortObject.AddElementToList(currentChild);
            doorsToSortList.Add(newDoorSortObject);
        }
    }

    private List<Transform> GetDoorParentList(Transform house)
    {
        var doorParents = new List<Transform>();
        for (var i = 0; i < house.childCount; i++)
        {
            var currentFloor = house.GetChild(i);
            var floorChildCount = currentFloor.childCount;
            for (var j = 0; j < floorChildCount; j++)
            {
                var currentObjectParent = currentFloor.GetChild(j);
                if (currentObjectParent.name.Equals(doorParentObjectName))
                {
                    doorParents.Add(currentObjectParent);
                }
            }
        }

        return doorParents;
    }

    private bool ArrayContains(int[] array, int number)
    {
        return array.Any(t => t == number);
    }

    private bool ArrayDoesNotContain(int[] array, int number)
    {
        return array.Any(t => t != number);
    }

    private Tuple<bool, DoorToSort> DoorListContains(List<DoorToSort> list, int number)
    {
        foreach (var doorToSort in list)
        {
            if (doorToSort.DoorName == number)
            {
                return new Tuple<bool, DoorToSort>(true, doorToSort);
            }
        }

        return new Tuple<bool, DoorToSort>(false, null);
    }
    
    private class DoorToSort
    {
        public Transform Parent { get; set; }
        public int DoorName { get; set; }
        public List<Transform> DoorParts { get; set; } = new List<Transform>();

        public void AddElementToList(Transform element)
        {
            DoorParts.Add(element);
        }
    }
    
}
