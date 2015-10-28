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

using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetNuke.Common
{
    public static class ExtensionMethods
    {
        #region Helpers / Extension methods that should be within a generic library

        /// <summary>
        /// Combines the source hashtable with the specified target hastable; optionally overwrites existing values in the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="overwriteSource">if set to <c>true</c> [overwrite source].</param>
        /// <returns></returns>
        public static Hashtable Combine(this Hashtable source, Hashtable target, bool overwriteSource = true)
        {
            foreach (var key in target.Keys)
            {
                if (source.ContainsKey(key))
                {
                    if (overwriteSource)
                    {
                        source[key] = target[key];
                    }
                }
                else
                {
                    source.Add(key, target[key]);
                }
            }
            return source;
        }

        /// <summary>
        /// Executes an action for each element in the source collection.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IEnumerable<TType> ForEach<TType>(this IEnumerable<TType> source, Action<TType> action)
        {
            foreach (TType element in source)
            {
                action(element);
            }

            return source;
        }

        #endregion
    }
}