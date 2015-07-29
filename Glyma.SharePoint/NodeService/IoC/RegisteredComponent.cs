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
using System.Reflection;

namespace IoC
{
    internal class RegisteredComponent
    {
        private Type _componentType = null;
        private object _componentInstance = null;
        private IIoCContainer _iocContainerParent = null;
        private RegisteredParameters _constructorParameters = null;

        public RegisteredComponent()
        {
        }

        public RegisteredComponent(IIoCContainer iocContainerParent, object componentInstance)
            : this()
        {
            _iocContainerParent = iocContainerParent;
            _componentInstance = componentInstance;
        }

        public RegisteredComponent(IIoCContainer iocContainerParent, Type componentType)
            : this()
        {
            _iocContainerParent = iocContainerParent;
            _componentType = componentType;
        }

        public RegisteredComponent(IIoCContainer iocContainerParent, Type componentType, RegisteredParameters parameters)
            : this()
        {
            _iocContainerParent = iocContainerParent;
            _componentType = componentType;
            _constructorParameters = parameters;
        }

        public Type ComponentType
        {
            get
            {
                return _componentType;
            }
        }

        public object GetInstance()
        {
            Dictionary<Type, int> attemptedInstances = new Dictionary<Type, int>();

            return GetInstance(attemptedInstances);
        }

        internal object GetInstance(Dictionary<Type, int> attemptedInstances)
        {
            if (_componentInstance == null)
            {
                if (!attemptedInstances.ContainsKey(ComponentType))
                {
                    attemptedInstances.Add(ComponentType, 1);

                    if (_constructorParameters == null)
                    {
                        ConstructorInfo defaultConstructor = _componentType.GetConstructor(new Type[0] { });

                        _componentInstance = defaultConstructor.Invoke(new object[0] { });
                    }
                    else
                    {
                        Type[] parameterTypes = new Type[Parameters.Count];
                        object[] parameterObjects = new object[Parameters.Count];

                        for (int i = 0; i < Parameters.Count; i++)
                        {
                            parameterTypes[i] = Parameters[i].ComponentType;

                            if (Parameters[i].Instance == null)
                            {
                                parameterObjects[i] = IoCContainerParent.GetInstance(Parameters[i].ComponentType, attemptedInstances);
                            }
                            else
                            {
                                parameterObjects[i] = Parameters[i].Instance;
                            }
                        }

                        ConstructorInfo constructor = _componentType.GetConstructor(parameterTypes);

                        _componentInstance = constructor.Invoke(parameterObjects);
                    }
                }
                else
                {
                    throw new CyclicInjectionException("A cyclic dependency has been detected between injected objects.");
                }
            }

            return _componentInstance;
        }

        public IIoCContainer IoCContainerParent
        {
            get
            {
                return _iocContainerParent;
            }
        }

        public RegisteredParameters Parameters
        {
            get
            {
                return _constructorParameters;
            }
        }
    }
}
