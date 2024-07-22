using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;

namespace House.Sorter.v2
{
    public class SortComponent
    {
        private readonly Regex regex;
        private readonly string name;
        private readonly Regex[] options;
        
        public string Name => name;
        public Regex Regex => regex;
        public Regex[] Options => options;
        
        public SortComponent(string name, string regexKey)
        {
            this.name = name;
            regex = new Regex(@"\b\w*" + regexKey + @"\w*\b");
        }
        
        public SortComponent(string name, string regexKey, string[] options)
        {
            this.name = name;
            regex = new Regex(@"\b\w*" + regexKey + @"\w*\b");
            this.options = CreateOptions(options);
        }

        public bool CheckComponent(string componentName)
        {
            return regex.Match(componentName).Success;
        }

        private static Regex[] CreateOptions(string[] optionsStringList)
        {
            var returnList = new Regex[optionsStringList.Length];
            for (var i = 0; i < optionsStringList.Length; i++)
            {
                returnList[i] = new Regex(@"\b\w*" + optionsStringList[i] + @"\w*\b");
            }

            return returnList;
        }
        
    }
}