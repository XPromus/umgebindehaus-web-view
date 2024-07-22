using System;

namespace House.Sorter.v2
{
    [Serializable]
    public struct SortKeyValue
    {
        public string key;
        public string objectName;
        public string[] options;
    }
}