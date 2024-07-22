using System;
using System.Collections.Generic;
using UnityEngine;

namespace House.Sorter.v2
{
    [Serializable]
    public struct GroupObject
    {
        public string name;
        public List<Transform> gameObjects;
    }
}