/*
' Copyright (c) 2015  XCESS expertise center b.v.
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System.Reflection;

namespace DotNetNuke.Entities.Modules
{
    public class ParameterMapping
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMapping"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="property">The property.</param>
        public ParameterMapping(BaseParameterAttribute attribute, PropertyInfo property)
        {
            this.Attribute = attribute;
            this.Property = property;

            var parameterName = attribute.ParameterName;
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                parameterName = property.Name;
            }

            var parameterGrouping = attribute as IParameterGrouping;
            if (parameterGrouping != null)
            {
                if (!string.IsNullOrWhiteSpace(parameterGrouping.Prefix))
                {
                    parameterName = parameterGrouping.Prefix + parameterName;
                }
            }

            this.ParameterName = parameterName;
        }

        #endregion

        #region Properties

        public BaseParameterAttribute Attribute { get; set; }

        public string ParameterName { get; set;  }

        public PropertyInfo Property { get; set;  }

        #endregion
    }
}
