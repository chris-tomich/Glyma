using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.BusinessData.MetadataModel;
using Microsoft.BusinessData.MetadataModel.Collections;

namespace Glyma.SharePoint.Search
{
    public static class INamedPropertyDictionaryExtensions
    {
        /// <summary>
        /// Checks the existence of a key within a INamedPropertyDictionary object using a specific string comparer.
        /// </summary>
        /// <param name="lobInstances">A INamedPropertyDictionary object to be searched.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="comparisonType">The string comparer to use to compare the keys.</param>
        /// <returns>true if a match is found; otherwise, false.</returns>
        public static bool ContainsKey(this INamedPropertyDictionary properties, string key, StringComparison comparisonType)
        {
            bool result = false;
            foreach (string propertyKey in properties.Keys)
            {
                if (propertyKey.Equals(key, comparisonType))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }


        /// <summary>
        /// Gets a property value from a INamedPropertyDictionary object using a key and a specific string comparer to examine the keys.
        /// </summary>
        /// <param name="properties">A INamedPropertyDictionary object to be searched.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="comparisonType">The string comparer to use to compare the keys.</param>
        /// <returns>An object that matches the key; otherwise, null.</returns>
        public static object GetByKey(this INamedPropertyDictionary properties, string key, StringComparison comparisonType)
        {
            object propertyValue = null;
            foreach (string propertyKey in properties.Keys)
            {
                if (propertyKey.Equals(key, comparisonType))
                {
                    propertyValue = properties[propertyKey];
                    break;
                }
            }
            return propertyValue;
        }
    }
}
