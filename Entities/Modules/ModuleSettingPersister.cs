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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Caching;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Entities.Modules
{
    public class ModuleSettingPersister<TType>
        where TType : new()
    {
        public const string CachePrefix = "ModuleSettingsPersister_";

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleSettingPersister{TType}"/> class.
        /// </summary>
        public ModuleSettingPersister()
        {
            this.PortalSettings = PortalSettings.Current;
            this.Mapping = new Dictionary<BaseParameterAttribute, PropertyInfo>();
        }

        #endregion

        private IDictionary<BaseParameterAttribute, PropertyInfo> Mapping { get; }

        private PortalSettings PortalSettings { get; }

        public TType Load(Hashtable hastable)
        {
            Requires.NotNull("hashtable", hastable);

            var settings = new TType();
            this.Mapping.ForEach(mapping =>
                                 {
                                     object settingValue = null;
                                     if (mapping.Key is PortalSettingAttribute)
                                     {
                                         settingValue = PortalController.GetPortalSetting(mapping.Value.Name, this.PortalSettings.PortalId, null);
                                         if (string.IsNullOrWhiteSpace((string)settingValue) && (mapping.Key.DefaultValue != null))
                                         {
                                             settingValue = mapping.Key.DefaultValue;
                                         }
                                     }
                                     else if (hastable.ContainsKey(mapping.Key.ParameterName))
                                     {
                                         settingValue = hastable[mapping.Key.ParameterName];
                                     }
                                     else if (mapping.Key.DefaultValue != null)
                                     {
                                         settingValue = mapping.Key.DefaultValue;
                                     }

                                     if (settingValue != null)
                                     {
                                         var property = mapping.Value;
                                         if (property.CanWrite)
                                         {
                                             this.WriteProperty(settings, property, settingValue);
                                         }
                                     }

                                 });

            return settings;
        }

        public TType Load(int tabModuleId)
        {
            Requires.NotNegative("tabModuleId", tabModuleId);

            var controller = new ModuleController();
            var module = controller.GetTabModule(tabModuleId);
            if (module == null)
            {
                throw new ArgumentOutOfRangeException("tabModuleId", tabModuleId, "TabModuleId not found!");
            }

            return this.Load(module.ModuleSettings.Combine(module.TabModuleSettings));
        }

        public void Save(TType settings, int moduleId, int tabModuleId)
        {
            Requires.NotNull("settings", settings);
            Requires.NotNegative("moduleId", moduleId);
            Requires.NotNegative("tabModuleId", tabModuleId);

            var controller = new ModuleController();
            this.Mapping.ForEach(mapping =>
                                 {
                                     var attribute = mapping.Key;
                                     var property = mapping.Value;

                                     if (property.CanRead) // Should be, because we asked for properties with a Get accessor.
                                     {
                                         var settingValue = property.GetValue(settings, null);
                                         if (settingValue != null)
                                         {
                                             if (attribute is ModuleSettingAttribute)
                                             {
                                                 controller.UpdateModuleSetting(moduleId, attribute.ParameterName, settingValue.ToString());
                                             }
                                             else if (attribute is TabModuleSettingAttribute)
                                             {
                                                 controller.UpdateModuleSetting(moduleId, attribute.ParameterName, settingValue.ToString());
                                             }
                                             else if (attribute is PortalSettingAttribute)
                                             {
                                                 PortalController.UpdatePortalSetting(this.PortalSettings.PortalId, attribute.ParameterName, settingValue.ToString());
                                             }
                                         }
                                     }
                                 });
        }

        #region Helpers

        protected void LoadMapping()
        {
            var cacheKey = this.CacheKey;
            var mapping = CachingProvider.Instance().GetItem(cacheKey) as IDictionary<BaseParameterAttribute, PropertyInfo>;
            if (mapping == null)
            {
                mapping = this.CreateMapping();
                // HARDCODED: 2 hour expiration
                CachingProvider.Instance().Insert(cacheKey, mapping, (DNNCacheDependency)null, DateTime.Now.AddHours(2), Cache.NoSlidingExpiration);
            }
        }

        protected virtual string CacheKey
        {
            get
            {
                var type = typeof(TType);
                return ModuleSettingPersister<TType>.CachePrefix + type.FullName.Replace(".", "_");
            }
        }

        protected IDictionary<BaseParameterAttribute, PropertyInfo> CreateMapping()
        {
            var mapping = new Dictionary<BaseParameterAttribute, PropertyInfo>();
            var type = typeof(TType);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);

            properties.ForEach(property =>
                               {
                                   // In .NET Framework 4.5.x the call below can be replaced by property.GetCustomAttributes<BaseParameterAttribute>(true);
                                   var attributes = property.GetCustomAttributes(typeof(BaseParameterAttribute), true).OfType<BaseParameterAttribute>(); 
                                   attributes.ForEach(attribute =>
                                                      {
                                                          var attributeName = attribute.ParameterName;
                                                          if (string.IsNullOrWhiteSpace(attributeName))
                                                          {
                                                              attribute.ParameterName = property.Name;
                                                          }


                                                          var parameterGrouping = attribute as IParameterGrouping;
                                                          if (parameterGrouping != null)
                                                          {
                                                              if (!string.IsNullOrWhiteSpace(parameterGrouping.Prefix))
                                                              {
                                                                  attributeName = parameterGrouping.Prefix + attributeName;
                                                              }
                                                          }

                                                          // Update the ParameterName
                                                          attribute.ParameterName = attributeName;

                                                          this.Mapping.Add(attribute, property);
                                                      });
                               });

            return mapping;
        }

        /// <summary>
        /// Writes the property.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="property">The property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <exception cref="System.InvalidCastException"></exception>
        private void WriteProperty(TType settings, PropertyInfo property, object propertyValue)
        {
            try
            {
                var valueType = propertyValue.GetType();
                var propertyType = property.PropertyType;
                if (propertyType.GetGenericArguments().Any() && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // Nullable type
                    propertyType = propertyType.GetGenericArguments()[0];
                }

                if (propertyType == valueType)
                {
                    // The property and settingsValue have the same type - no conversion needed - just update!
                    property.SetValue(settings, propertyValue, null);
                }
                else if (propertyType.BaseType == typeof(Enum))
                {
                    // The property is an enum. Determine if the enum value is persisted as string or numeric.
                    if (Regex.IsMatch(propertyValue.ToString(), "^\\d+$"))
                    {
                        // The enum value is a number
                        property.SetValue(settings, Enum.ToObject(propertyType, Convert.ToInt32(propertyValue, CultureInfo.InvariantCulture)), null);
                    }
                    else
                    {
                        try
                        {
                            property.SetValue(settings, Enum.Parse(propertyType, propertyValue.ToString(), true), null);
                        }
                        catch (ArgumentException exception)
                        {
                            // Just log the exception. Use the default.
                            Exceptions.LogException(exception);
                        }
                    }
                }
                else if (!(propertyValue is IConvertible))
                {
                    // The property value does not support IConvertible interface - assign the value direct.
                    property.SetValue(settings, propertyValue, null);
                }
                else
                {
                    property.SetValue(settings, Convert.ChangeType(propertyValue, propertyType, CultureInfo.InvariantCulture), null);
                }
            }
            catch (Exception exception)
            {
                throw new InvalidCastException(string.Format(CultureInfo.CurrentUICulture, "Could not cast {0} to property {1} of type {2}",
                                                             propertyValue,
                                                             property.Name,
                                                             property.PropertyType), exception);
            }
        }

        #endregion
    }
}
