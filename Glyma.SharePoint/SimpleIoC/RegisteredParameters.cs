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

namespace SimpleIoC
{
    public class RegisteredParameters : List<RegisteredParameter>
    {
        public RegisteredParameters()
            : base()
        {
        }

        public RegisteredParameters(int capacity)
            : base(capacity)
        {
        }

        public RegisteredParameters(IEnumerable<RegisteredParameter> collection)
            : base(collection)
        {
        }
    }
}
