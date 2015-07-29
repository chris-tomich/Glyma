/*
 * Original Author: Chris Tomich
 * Web Site: http://mymemorysucks.com
 * Released into the public domain on the 2nd August, 2009 6:30PM +8:00 GMT.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHOR
 * BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.Runtime.Serialization;

namespace SimpleIoC
{
    public class IoCContainer : IIoCContainer
    {
        private static readonly object _threadLock = new object();
        private static IoCContainer _globalInstance = null;
        private static Dictionary<string, IoCContainer> _iocInstances = new Dictionary<string, IoCContainer>();

        public static IoCContainer GetInjectionInstance()
        {
            lock (_threadLock)
            {
                if (_globalInstance == null)
                {
                    _globalInstance = new IoCContainer();
                }

                return _globalInstance;
            }
        }

        public static IoCContainer GetInjectionInstance(string iocInstanceKey)
        {
            lock (_threadLock)
            {
                if (_iocInstances.ContainsKey(iocInstanceKey))
                {
                    return _iocInstances[iocInstanceKey];
                }
                else
                {
                    IoCContainer injectionInstance = new IoCContainer(iocInstanceKey);

                    _iocInstances[iocInstanceKey] = injectionInstance;

                    return injectionInstance;
                }
            }
        }

        private Dictionary<Type, RegisteredComponent> _registeredComponents = null;

        protected IoCContainer()
        {
            _registeredComponents = new Dictionary<Type, RegisteredComponent>();
        }

        protected IoCContainer(string injectionInstanceKey)
            : this()
        {
        }

        public void RegisterComponent<ComponentType>(ComponentType instance)
        {
            Type componentType = typeof(ComponentType);

            RegisteredComponent registeredComponent = new RegisteredComponent(this, instance);

            _registeredComponents.Add(componentType, registeredComponent);
        }

        public void RegisterComponent<ComponentType>(params RegisteredParameter[] parameters)
        {
            Type componentType = typeof(ComponentType);

            RegisteredParameters registeredParameters = new RegisteredParameters(parameters);
            RegisteredComponent registeredComponent = new RegisteredComponent(this, componentType, registeredParameters);

            _registeredComponents.Add(componentType, registeredComponent);
        }

        public void RegisterComponent<ComponentAbstractType, ComponentConcreteType>()
        {
            Type abstractType = typeof(ComponentAbstractType);
            Type concreteType = typeof(ComponentConcreteType);

            if (concreteType.IsAbstract || concreteType.IsInterface)
            {
                throw new NotSupportedException("The provided type is not a concrete type.");
            }

            RegisteredComponent registeredComponent = new RegisteredComponent(this, concreteType);

            _registeredComponents.Add(abstractType, registeredComponent);
        }

        public void RegisterComponent<ComponentAbstractType, ComponentConcreteType>(ComponentConcreteType instance)
        {
            Type abstractType = typeof(ComponentAbstractType);
            Type concreteType = typeof(ComponentConcreteType);

            if (concreteType.IsAbstract || concreteType.IsInterface)
            {
                throw new NotSupportedException("The provided type is not a concrete type.");
            }

            RegisteredComponent registeredComponent = new RegisteredComponent(this, concreteType, instance);

            _registeredComponents.Add(abstractType, registeredComponent);
        }

        public void RegisterComponent<ComponentAbstractType, ComponentConcreteType>(params RegisteredParameter[] parameters)
        {
            Type abstractType = typeof(ComponentAbstractType);
            Type concreteType = typeof(ComponentConcreteType);

            if (concreteType.IsAbstract || concreteType.IsInterface)
            {
                throw new NotSupportedException("The provided type is not a concrete type.");
            }

            RegisteredParameters registeredParameters = new RegisteredParameters(parameters);
            RegisteredComponent registeredComponent = new RegisteredComponent(this, concreteType, registeredParameters);

            _registeredComponents.Add(abstractType, registeredComponent);
        }

        public ComponentType GetInstance<ComponentType>()
        {
            Type componentType = typeof(ComponentType);
            ComponentType instance = default(ComponentType);

            if (_registeredComponents.ContainsKey(componentType))
            {
                if (_registeredComponents[componentType].GetInstance() is ComponentType)
                {
                    instance = (ComponentType)_registeredComponents[componentType].GetInstance();
                }
                else
                {
                    throw new InvalidCastException("The stored instance cannot be cast to the given type.");
                }
            }
            else
            {
                RegisteredComponent registeredComponent = new RegisteredComponent(this, componentType);

                _registeredComponents[componentType] = registeredComponent;

                instance = (ComponentType)registeredComponent.GetInstance();
            }

            return instance;
        }

        public object GetInstance(Type componentType)
        {
            object instance = null;

            if (_registeredComponents.ContainsKey(componentType))
            {
                object generatedInstance = _registeredComponents[componentType].GetInstance();

                if (componentType.Equals(generatedInstance.GetType()))
                {
                    instance = _registeredComponents[componentType].GetInstance();
                }
                else
                {
                    throw new InvalidCastException("The stored instance cannot be cast to the given type.");
                }
            }
            else
            {
                RegisteredComponent registeredComponent = new RegisteredComponent(this, componentType);

                _registeredComponents[componentType] = registeredComponent;

                instance = registeredComponent.GetInstance();
            }

            return instance;
        }

        public object GetInstance(Type componentType, Dictionary<Type, int> attemptedInstances)
        {
            object instance = null;

            if (_registeredComponents.ContainsKey(componentType))
            {
                instance = _registeredComponents[componentType].GetInstance(attemptedInstances);
            }
            else
            {
                RegisteredComponent registeredComponent = new RegisteredComponent(this, componentType);

                _registeredComponents[componentType] = registeredComponent;

                instance = registeredComponent.GetInstance(attemptedInstances);
            }

            return instance;
        }
    }
}
