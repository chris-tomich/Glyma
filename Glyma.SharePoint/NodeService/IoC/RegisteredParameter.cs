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

namespace IoC
{
    public class RegisteredParameter
    {
        private Type _parameterType = null;
        private object _instance = null;

        public RegisteredParameter(Type parameterType)
        {
            _parameterType = parameterType;
        }

        public RegisteredParameter(object instance)
            : this(instance.GetType())
        {
            _instance = instance;
        }

        public RegisteredParameter(Type parameterType, object instance)
            : this(parameterType)
        {
            _instance = instance;
        }

        public Type ComponentType
        {
            get
            {
                return _parameterType;
            }
        }

        public object Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
