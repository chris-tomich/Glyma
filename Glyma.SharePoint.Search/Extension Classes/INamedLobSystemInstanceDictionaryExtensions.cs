using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.BusinessData.MetadataModel;
using Microsoft.BusinessData.MetadataModel.Collections;

namespace Glyma.SharePoint.Search
{
    public static class INamedLobSystemInstanceDictionaryExtensions
    {
        /// <summary>
        /// Checks the existence of a key within a INamedLobSystemInstanceDictionary object using a specific string comparer.
        /// </summary>
        /// <param name="lobInstances">A INamedLobSystemInstanceDictionary object to be searched.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="comparisonType">The string comparer to use to compare the keys.</param>
        /// <returns>true if a match is found; otherwise, false.</returns>
        public static bool ContainsKey(this INamedLobSystemInstanceDictionary lobInstances, string key, StringComparison comparisonType)
        {
            bool result = false;
            foreach (string lobInstanceKey in lobInstances.Keys)
            {
                if (lobInstanceKey.Equals(key, comparisonType))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }


        /// <summary>
        /// Gets a LOB system instance from a INamedLobSystemInstanceDictionary object using a key and a specific string comparer to examine the keys.
        /// </summary>
        /// <param name="lobInstances">A INamedLobSystemInstanceDictionary object to be searched.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="comparisonType">The string comparer to use to compare the keys.</param>
        /// <returns>A ILobSystemInstance object that matches the key; otherwise, null.</returns>
        public static ILobSystemInstance GetByKey(this INamedLobSystemInstanceDictionary lobInstances, string key, StringComparison comparisonType)
        {
            ILobSystemInstance lobInstance = null;
            foreach (string lobInstanceKey in lobInstances.Keys)
            {
                if (lobInstanceKey.Equals(key, comparisonType))
                {
                    lobInstance = lobInstances[lobInstanceKey];
                    break;
                }
            }
            return lobInstance;
        }
    }
}
