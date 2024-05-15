using System;
using System.Collections.Generic;
using System.Linq;
using House.Sorter;
using UnityEngine;

public class SortWindows : MonoBehaviour
{

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
    }

    private List<GameObject> SortObjectsInScene(List<WindowToSort> list)
    {
        var returnList = new List<GameObject>();
        foreach (var windowToSort in list)
        {
            var newObject = new GameObject
            {
                name = windowToSort.Name.ToString(),
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
        for (int i = 0; i < parent.childCount; i++)
        {
            var currentChild = parent.GetChild(i);
            var splitName = currentChild.name.Split("_");
            var windowNumber = Int32.Parse(splitName[1]);

            if (sortAllWindows)
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
                        Name = windowNumber
                    };
                    newWindowSortObject.AddElementToList(currentChild);
                    windowsToSortList.Add(newWindowSortObject);
                }
            }
            else
            {
                if (ArrayContains(windowsToSort, windowNumber))
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
                            Name = windowNumber
                        };
                        newWindowSortObject.AddElementToList(currentChild);
                        windowsToSortList.Add(newWindowSortObject);
                    }
                }
            }
        }

        return windowsToSortList;
    }

    private List<Transform> GetWindowParentList(Transform house)
    {
        List<Transform> windowParents = new List<Transform>();
        var houseChildrenCount = house.childCount;
        for (int i = 0; i < houseChildrenCount; i++)
        {
            var currentFloor = house.GetChild(i);
            var floorChildCount = currentFloor.childCount;
            for (int j = 0; j < floorChildCount; j++)
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

    private Tuple<bool, WindowToSort> WindowListContains(List<WindowToSort> list, int number)
    {
        foreach (var windowToSort in list)
        {
            if (windowToSort.Name == number)
            {
                return new Tuple<bool, WindowToSort>(true, windowToSort);
            }
        }

        return new Tuple<bool, WindowToSort>(false, null);
    }

    private class WindowToSort
    {
        private Transform _parent;
        public Transform Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        
        private int _name;
        public int Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        private List<Transform> _windowParts = new List<Transform>();
        public List<Transform> WindowParts
        {
            get { return _windowParts; }
            set {  }
        }

        public void AddElementToList(Transform element)
        {
            _windowParts.Add(element);
        }
        
    }
    
}
