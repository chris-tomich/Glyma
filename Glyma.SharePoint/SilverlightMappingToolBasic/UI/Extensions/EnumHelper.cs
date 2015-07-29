using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace SilverlightMappingToolBasic.UI.Extensions
{
    public static class EnumHelper
    {
        public static ObservableCollection<string> GetEnumItems<T>() where T: struct 
        {
            var enumItems = new ObservableCollection<string>();
            foreach (var value in System.Enum.GetValues(typeof(T)))
            {
                enumItems.Add(((T)value).GetDisplayName());
            }
            return enumItems;
        }

        public static string GetDisplayName<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DisplayAttribute)attrs[0]).Name;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        public static T GetEnum<T>(string displayName)
        {
            foreach (var value in System.Enum.GetValues(typeof(T)))
            {
                var array = value.GetType().GetField(value.ToString()).GetCustomAttributes(false);
                var display = array.FirstOrDefault(q => q is DisplayAttribute);
                if (display != null)
                {
                    if (((DisplayAttribute)display).Name == displayName)
                    {
                        return (T)value;
                    }
                }
                else if (value.ToString() == displayName)
                {
                    return (T)value;
                }

            }
            throw new KeyNotFoundException("Cannot found the item");
        }
    }
}
