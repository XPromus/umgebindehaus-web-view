using System;
using System.Collections.Generic;
using House.Sorter.v2.sorters;
using House.Sorter.v2.sorters.door;
using House.Sorter.v2.sorters.window;
using UnityEngine;

namespace House.Sorter.v2
{
    public class HouseSorter : MonoBehaviour
    { 

        [SerializeField] private GameObject houseToSort;
        private GameObject house;
        
        [SerializeField] private List<SortKeyValue> floorKeyValueList;
        private readonly List<SortComponent> floorSortComponents = new();

        [SerializeField] private List<SortKeyValue> windowKeyValueList;
        private readonly List<SortComponent> windowSortComponents = new();

        [SerializeField] private List<SortKeyValue> doorKeyValueList;
        private readonly List<SortComponent> doorSortComponents = new();

        [SerializeField] private List<SortKeyValue> objectKeyValueList;
        private readonly List<SortComponent> objectSortComponents = new();

        //TODO: Apply window materials automatically
        [Header("Settings")] 
        [SerializeField] private Material windowMaterial;
        
        private void Start()
        {
            house = new GameObject()
            {
                name = "House",
            };
            
            CreateSortComponentLists();
            var floorObjectsParent = SortHouseFloors();

            var windowSorter = new WindowSorter(windowSortComponents, floorObjectsParent);
            windowSorter.SortWindowsInHouse();

            var doorSorter = new DoorSorter(doorSortComponents, floorObjectsParent);
            doorSorter.SortDoorsInHouse();
            
            var groupSorter = new GroupSorter();
            groupSorter.SortObjectsToGroups(floorObjectsParent);
        }

        public GameObject SortHouse()
        {
            house = new GameObject()
            {
                name = "House",
            };
            
            CreateSortComponentLists();
            var floorObjectsParent = SortHouseFloors();

            var windowSorter = new WindowSorter(windowSortComponents, floorObjectsParent);
            windowSorter.SortWindowsInHouse();

            var doorSorter = new DoorSorter(doorSortComponents, floorObjectsParent);
            doorSorter.SortDoorsInHouse();

            var groupSorter = new GroupSorter();
            groupSorter.SortObjectsToGroups(floorObjectsParent);
            
            return house;
        }
        
        //TODO: Sort objects into groups
        private void SortObjectsInFloorToGroups(GameObject floorParent)
        {
            var groupObjects = new List<GroupObject>();
            
            var pointer = 0;
            while (pointer < floorParent.transform.childCount)
            {
                var child = floorParent.transform.GetChild(pointer);
                if (child.childCount > 0)
                {
                    pointer++;
                    continue;
                }
                
                child.transform.parent = floorParent.transform;
            }
        }

        private void CreateSortComponentLists()
        {
            foreach (var value in floorKeyValueList)
            {
                floorSortComponents.Add(new SortComponent(value.objectName, value.key, value.options));
            }
            foreach (var value in windowKeyValueList)
            {
                windowSortComponents.Add(new SortComponent(value.objectName, value.key, value.options));
            }
            foreach (var value in doorKeyValueList)
            {
                doorSortComponents.Add(new SortComponent(value.objectName, value.key, value.options));
            }
            foreach (var value in objectKeyValueList)
            {
                objectSortComponents.Add(new SortComponent(value.objectName, value.key, value.options));
            }
        }

        private GameObject SortHouseFloors()
        {
            var floorObjectsParent = new GameObject
            {
                name = "Floors",
                transform =
                {
                    parent = house.transform
                }
            };
            
            foreach (var floor in floorSortComponents)
            {
                MoveObjectsToFloorInHouse(floor, floorObjectsParent);
            }

            houseToSort.name = "Leftover";
            houseToSort.transform.parent = house.transform;

            return floorObjectsParent;
        }

        private void MoveObjectsToFloorInHouse(SortComponent floor, GameObject floorObjectsParent)
        {
            var floorParent = new GameObject
            {
                name = floor.Name,
                transform =
                {
                    parent = floorObjectsParent.transform
                }
            };

            var pointer = 0;
            while (pointer < houseToSort.transform.childCount)
            {
                var child = houseToSort.transform.GetChild(pointer);
                if (!floor.CheckComponent(child.gameObject.name))
                {
                    pointer++;
                    continue;
                }
                child.transform.parent = floorParent.transform;
            }
                
            floorParent.transform.parent = floorObjectsParent.transform;
        }
        
    }
}