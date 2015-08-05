using System;
using Microsoft.BusinessData.Runtime;
using Microsoft.BusinessData.MetadataModel;
using Microsoft.BusinessData.MetadataModel.Collections;

namespace Glyma.SharePoint.Search
{
    public static class DynamicTypeExtensions
    {

        /// <summary>
        /// Initialises a DynamicType object using details from a ITypeDescriptorCollection object.
        /// </summary>
        /// <param name="node">A DynamicType object to initialise.</param>
        /// <param name="entityFields">A ITypeDescriptorCollection object containing the details of the fields.</param>
        public static void Initialise(this DynamicType node, ITypeDescriptorCollection entityFields)
        {
            // Add an entry for each field in entityFields and initialise its value to an appropriate default.
            foreach (ITypeDescriptor fieldDefinition in entityFields)
            {
                Type fieldType = Type.GetType(fieldDefinition.TypeName, true, true);
                object fieldValue;
                if (fieldType == typeof(string))
                {
                    fieldValue = string.Empty;
                }
                else if (fieldType == typeof(DateTime))
                {
                    fieldValue = new DateTime(1970, 1, 1);
                }
                else
                {
                    fieldValue = Activator.CreateInstance(fieldType);
                }
                node.Add(fieldDefinition.Name, fieldValue);
            }
        }


        /// <summary>
        /// Sets the value for a specified field in a DynamicType object.
        /// </summary>
        /// <param name="node">A DynamicType object to update.</param>
        /// <param name="fieldName">The name of the field to update.</param>
        /// <param name="value">The value of the field.</param>
        /// <remarks>
        /// If the field doesn't exist in the DynamicType object but the DynamicType object contains a "content" field, the field and its value is appended to the "content" field.
        /// </remarks>
        public static void SetValue(this DynamicType node, string fieldName, object value)
        {
            if (node.ContainsKey(fieldName))
            {
                node[fieldName] = value;
            }
            else if (node.ContainsKey(GlymaEntityFields.Content) && value != null)
            {
                // Only add the new metadata if it doesn't already exist in the content field.
                string newEntry = fieldName + ":" + value.ToString() + GlymaModelConstants.WordBreakString;
                string currentContent = (string)node[GlymaEntityFields.Content];
                if (string.IsNullOrEmpty(currentContent))
                {
                    node[GlymaEntityFields.Content] = newEntry;
                }
                else if (currentContent.IndexOf(newEntry, StringComparison.OrdinalIgnoreCase) == -1)
                {
                    node[GlymaEntityFields.Content] += fieldName + ":" + value.ToString() + GlymaModelConstants.WordBreakString;
                }
            }
        }
    }
}
