using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Glyma.Powershell.Smo
{
    public abstract class SmoObjectBase
    {
        protected SmoObjectBase()
        {
        }

        protected SmoObjectBase(SqlAssemblies assemblies, string typeName, params object[] constructorParameters)
            : base()
        {
            Assemblies = assemblies;

            ReflectedObject = Assemblies.SmoAssembly.CreateInstance(typeName, false, BindingFlags.Default, null, constructorParameters, null, null);
        }

        public SqlAssemblies Assemblies
        {
            get;
            protected set;
        }

        public object ReflectedObject
        {
            get;
            protected set;
        }

        private Type[] GetTypes(object[] objs)
        {
            Type[] types = new Type[objs.Length];

            for (int i = 0; i < objs.Length; i++)
            {
                object index = objs[i];

                if (index != null)
                {
                    types[i] = index.GetType();
                }
                else
                {
                    types[i] = null;
                }
            }

            return types;
        }

        protected object GetPropertyValue(string propertyName)
        {
            return GetPropertyValue(propertyName, ReflectedObject);
        }

        protected object GetPropertyValue(string propertyName, object obj, params object[] indexes)
        {
            Type objType = obj.GetType();

            if (indexes != null && indexes.Length > 0)
            {
                Type[] types = GetTypes(indexes);

                PropertyInfo objPropertyInfo = objType.GetProperty(propertyName, types);

                return objPropertyInfo.GetValue(obj, indexes);
            }
            else
            {
                PropertyInfo objPropertyInfo = objType.GetProperty(propertyName);

                return objPropertyInfo.GetValue(obj, null);
            }
        }

        protected void SetPropertyValue(string propertyName, object value)
        {
            SetPropertyValue(propertyName, ReflectedObject, value);
        }

        protected void SetPropertyValue(string propertyName, object obj, object value, params object[] indexes)
        {
            Type objType = obj.GetType();

            if (indexes != null && indexes.Length > 0)
            {
                Type[] types = GetTypes(indexes);

                PropertyInfo objPropertyInfo = objType.GetProperty(propertyName, types);

                objPropertyInfo.SetValue(obj, value, indexes);
            }
            else
            {
                PropertyInfo objPropertyInfo = objType.GetProperty(propertyName);

                objPropertyInfo.SetValue(obj, value, null);
            }
        }

        protected object InvokeMethod(string methodName, params object[] methodParameters)
        {
            return InvokeMethod(methodName, ReflectedObject, methodParameters);
        }

        protected object InvokeMethod(string methodName, object obj, params object[] methodParameters)
        {
            Type objType = obj.GetType();

            if (methodParameters != null && methodParameters.Length > 0)
            {
                Type[] types = GetTypes(methodParameters);

                MethodInfo objMethodInfo = objType.GetMethod(methodName);

                return objMethodInfo.Invoke(obj, methodParameters);
            }
            else
            {
                MethodInfo objMethodInfo = objType.GetMethod(methodName, Type.EmptyTypes);

                return objMethodInfo.Invoke(obj, null);
            }
        }
    }
}
