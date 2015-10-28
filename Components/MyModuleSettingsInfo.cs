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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using System.Web.Caching;

namespace XEC.DNN.ModuleSettingsModule.Components
{
    /// <summary>
    /// A demo POCO used to get and set DNN module related settings like TabModuleSettings, ModuleSettings and PortalSettings. Note that all property names are indicative 
    /// and used only for demonstration purposes.
    /// </summary>
    public class ModuleSettings
    {
        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        [TabModuleSetting]
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets a value whether 'something' is initialized.
        /// </summary>
        /// <value>
        /// The initialized value.
        /// </value>
        [ModuleSetting]
        public bool? Initialize { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [ModuleSetting(ParameterName = "MyDemoStatus", DefaultValue = Status.New)]
        public Status Status { get; set; }

        /// <summary>
        /// Gets or sets the maximum retries.
        /// </summary>
        /// <value>
        /// The maximum retries.
        /// </value>
        [ModuleSetting(DefaultValue = 3)]
        public int MaximumRetries { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [PortalSetting]
        public string UserName { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized. Notice that this property is not decorated with any settings attribute, thus it will not be persisted.
        /// However it can be used by the module to trigger specific logic.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get { return !string.IsNullOrWhiteSpace(this.UserName); }
        }

        public static ModuleSettings GetSettings(ModuleInfo ctlModule)
        {
            return CBO.GetCachedObject<ModuleSettings>(new CacheItemArgs(CacheKey(ctlModule.TabModuleID), 20, CacheItemPriority.AboveNormal, ctlModule),
                                                       GetModuleSettingsCallback,
                                                       true);
        }
        private static ModuleSettings GetModuleSettingsCallback(CacheItemArgs args)
        {
            var persister = new ModuleSettingPersister<ModuleSettings>();
            return persister.Load((ModuleInfo)args.ParamList[0]);
        }

        public static string CacheKey(int moduleId)
        {
            return string.Format("SettingsModule{0}", moduleId);
        }

        public void Save(ModuleInfo ctlModule)
        {
            var persister = new ModuleSettingPersister<ModuleSettings>();
            persister.Save(this, ctlModule);
        }

    }
}