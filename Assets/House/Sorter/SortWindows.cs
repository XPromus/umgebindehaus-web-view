using System;
using System.Collections.Generic;
using System.Linq;
using House.Sorter;
using UnityEngine;

public class SortWindows : MonoBehaviour
{
    
    //TODO: Include windows and doors that have rotation or direction object. Remove manual list 
    //TODO: Apply window materials automatically
    //TODO: Regex for filtering
    
    [Header("Settings")] 
    [SerializeField] private int[] windowsToSort;
    [SerializeField] private bool invertWindowsToSortList;
    [SerializeField] private string windowParentObjectName = "Fenster";
    [SerializeField] private bool sortAllWindows = false;
    
    public void SortSelectedWindows(GameObject targetHouse)
    {
        var windowParents = GetWindowParentList(targetHouse.transform);
        var windowsToSortLists = new List<List<WindowToSort>>();
        foreach (var t in windowParents)
        {
            windowsToSortLists.Add(CreateWindowGroups(t));
        }

        foreach (var list in windowsToSortLists)
        {
            var parentObjects = SortObjectsInScene(list);
            var sortWindowComponents = new SortWindowComponents();
            foreach (var windowParent in parentObjects)
            {
                sortWindowComponents.SortObjectsInWindow(windowParent);
            }
        }

        foreach (var parent in windowParents)
        {
            ApplyLayerRecursively(parent);
        }
        
    }

    private List<GameObject> SortObjectsInScene(List<WindowToSort> list)
    {
        var returnList = new List<GameObject>();
        foreach (var windowToSort in list)
        {
            var newObject = new GameObject
            {
                name = windowToSort.WindowName.ToString(),
                transform =
                {
                    parent = windowToSort.Parent
                }
            };
            foreach (var windowPart in windowToSort.WindowParts)
            {
                windowPart.parent = newObject.transform;
            }
            returnList.Add(newObject);
        }

        return returnList;
    }

    private List<WindowToSort> CreateWindowGroups(Transform parent)
    {
        var windowsToSortList = new List<WindowToSort>();
        parent.gameObject.layer = LayerMask.NameToLayer("Windows");
        for (var i = 0; i < parent.childCount; i++)
        {
            var currentChild = parent.GetChild(i);
            var splitName = currentChild.name.Split("_");
            var windowNumber = Int32.Parse(splitName[1]);

            switch (sortAllWindows)
            {
                case true:
                    StartSort(windowsToSortList, windowNumber, currentChild, parent);
                    break;
                
                case false:
                    switch (invertWindowsToSortList)
                    {
                        case true:
                            if (ArrayDoesNotContain(windowsToSort, windowNumber))
                            {
                                StartSort(windowsToSortList, windowNumber, currentChild, parent);
                            }
                            break;
                    
                        case false:
                            if (!invertWindowsToSortList && ArrayContains(windowsToSort, windowNumber))
                            {
                                StartSort(windowsToSortList, windowNumber, currentChild, parent);
                            }
                            break;
                    }
                    break;
            }
        }

        return windowsToSortList;
    }

    private void StartSort(
        List<WindowToSort> windowsToSortList, 
        int windowNumber, 
        Transform currentChild,
        Transform parent
        )
    {
        var windowSortListCheck = WindowListContains(windowsToSortList, windowNumber);
        if (windowSortListCheck.Item1)
        {
            windowSortListCheck.Item2.AddElementToList(currentChild);
        }
        else
        {
            var newWindowSortObject = new WindowToSort
            {
                Parent = parent,
                WindowName = windowNumber
            };
            newWindowSortObject.AddElementToList(currentChild);
            windowsToSortList.Add(newWindowSortObject);
        }
    }

    private List<Transform> GetWindowParentList(Transform house)
    {
        var windowParents = new List<Transform>();
        for (var i = 0; i < house.childCount; i++)
        {
            var currentFloor = house.GetChild(i);
            for (var j = 0; j < currentFloor.childCount; j++)
            {
                var currentObjectParent = currentFloor.GetChild(j);
                if (currentObjectParent.name.Equals(windowParentObjectName))
                {
                    windowParents.Add(currentObjectParent);
                }
            }
        }

        return windowParents;
    }

    private bool ArrayContains(int[] array, int number)
    {
        return array.Any(t => t == number);
    }

    private bool ArrayDoesNotContain(int[] array, int number)
    {
        return array.Any(t => t != number);
    }

    private Tuple<bool, WindowToSort> WindowListContains(List<WindowToSort> list, int number)
    {
        foreach (var windowToSort in list)
        {
            if (windowToSort.WindowName == number)
            {
                return new Tuple<bool, WindowToSort>(true, windowToSort);
            }
        }

        return new Tuple<bool, WindowToSort>(false, null);
    }

    private void ApplyLayerRecursively(Transform transform)
    {
        transform.gameObject.layer = LayerMask.NameToLayer("Windows");
        for (var i = 0; i < transform.childCount; i++)
        {
            ApplyLayerRecursively(transform.GetChild(i));
        }
    }

    private class WindowToSort
    {
        public Transform Parent { get; set; }

        public int WindowName { get; set; }

        public List<Transform> WindowParts { get; set; } = new List<Transform>();

        public void AddElementToList(Transform element)
        {
            WindowParts.Add(element);
        }
        
    }
    
}
