﻿/*
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Caching;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Entities.Modules.Settings
{
    public class ModuleSettingPersister<TType>
        where TType : class, new()
    {
        public const string CachePrefix = "ModuleSettingsPersister_";

        public TType Load(ModuleInfo moduleInfo)
        {
            Requires.NotNull("moduleInfo", moduleInfo);

            var settings = new TType();
            var setupHandler = settings as IParameterSetupHandler;
            if (setupHandler != null && setupHandler.SetupBeforeLoad)
            {
                // NOTE: this type of initialization or setup can usually be accomplished within the constructor of the POCO.
                setupHandler.Setup();
            }

            var persisterMapping = this.LoadMappings();
            persisterMapping.ForEach(mapping =>
                                 {
                                     object settingValue = null;

                                     var attribute = mapping.Attribute;
                                     var property = mapping.Property;
                                     if (attribute is PortalSettingAttribute)
                                     {
                                         settingValue = PortalController.GetPortalSetting(mapping.ParameterName, moduleInfo.PortalID, null);
                                         if (string.IsNullOrWhiteSpace((string)settingValue) && (attribute.DefaultValue != null))
                                         {
                                             settingValue = attribute.DefaultValue;
                                         }
                                     }
                                     else if (attribute is ModuleSettingAttribute && moduleInfo.ModuleSettings.ContainsKey(mapping.ParameterName))
                                     {
                                         settingValue = moduleInfo.ModuleSettings[mapping.ParameterName];
                                     }
                                     else if (attribute is TabModuleSettingAttribute && moduleInfo.TabModuleSettings.ContainsKey(mapping.ParameterName))
                                     {
                                         settingValue = moduleInfo.TabModuleSettings[mapping.ParameterName];
                                     }
                                     else if (attribute.DefaultValue != null)
                                     {
                                         settingValue = attribute.DefaultValue;
                                     }

                                     if (settingValue != null && property.CanWrite)
                                     {
                                         this.WriteProperty(settings, property, settingValue);
                                     }
                                 });

            if (setupHandler != null && !setupHandler.SetupBeforeLoad)
            {
                setupHandler.Setup();
            }

            return settings;
        }

        /// <summary>
        /// Saves the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="moduleInfo">The module information.</param>
        public void Save(TType settings, ModuleInfo moduleInfo)
        {
            Requires.NotNull("settings", settings);
            Requires.NotNull("moduleInfo", moduleInfo);

            var controller = new ModuleController();
            var mappings = this.LoadMappings();
            mappings.ForEach(mapping =>
                                 {
                                     var attribute = mapping.Attribute;
                                     var property = mapping.Property;

                                     if (property.CanRead) // Should be, because we asked for properties with a Get accessor.
                                     {
                                         var settingValue = property.GetValue(settings, null);
                                         if (settingValue != null)
                                         {
                                             if (attribute is ModuleSettingAttribute)
                                             {
                                                 controller.UpdateModuleSetting(moduleInfo.ModuleID, mapping.ParameterName, settingValue.ToString());
                                             }
                                             else if (attribute is TabModuleSettingAttribute)
                                             {
                                                 controller.UpdateTabModuleSetting(moduleInfo.TabModuleID, mapping.ParameterName, settingValue.ToString());
                                             }
                                             else if (attribute is PortalSettingAttribute)
                                             {
                                                 PortalController.UpdatePortalSetting(moduleInfo.PortalID, mapping.ParameterName, settingValue.ToString());
                                             }
                                         }
                                     }
                                 });
        }

        #region Helpers

        protected IList<ParameterMapping> LoadMappings()
        {
            var cacheKey = this.CacheKey;
            var mapping = CachingProvider.Instance().GetItem(cacheKey) as IList<ParameterMapping>;
            if (mapping == null)
            {
                mapping = this.CreateMappings();
                // HARDCODED: 2 hour expiration. 
                // Note that "caching" can also be accomplished with a static dictionary since the Attribute/Property mapping does not change unless the module is updated.
                CachingProvider.Instance().Insert(cacheKey, mapping, (DNNCacheDependency)null, DateTime.Now.AddHours(2), Cache.NoSlidingExpiration);
            }

            return mapping;
        }

        protected virtual string CacheKey
        {
            get
            {
                var type = typeof(TType);
                return ModuleSettingPersister<TType>.CachePrefix + type.FullName.Replace(".", "_");
            }
        }

        protected virtual IList<ParameterMapping> CreateMappings()
        {
            var mapping = new List<ParameterMapping>();
            var type = typeof(TType);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);

            properties.ForEach(property =>
                               {
                                   // In .NET Framework 4.5.x the call below can be replaced by property.GetCustomAttributes<BaseParameterAttribute>(true);
                                   var attributes = property.GetCustomAttributes(typeof(BaseParameterAttribute), true)
                                                            .OfType<BaseParameterAttribute>();
                                   attributes.ForEach(attribute => mapping.Add(new ParameterMapping(attribute, property)));
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
                // TODO: Localize exception
                throw new InvalidCastException(string.Format(CultureInfo.CurrentUICulture, "Could not cast {0} to property {1} of type {2}",
                                                             propertyValue,
                                                             property.Name,
                                                             property.PropertyType), exception);
            }
        }

        #endregion
    }
}
