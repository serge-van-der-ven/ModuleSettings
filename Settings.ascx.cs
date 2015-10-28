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
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using XEC.DNN.ModuleSettingsModule.Components;

namespace XEC.DNN.ModuleSettingsModule
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Settings class manages Module Settings
    /// 
    /// Typically your settings control would be used to manage settings for your module.
    /// There are two types of settings, ModuleSettings, and TabModuleSettings.
    /// 
    /// ModuleSettings apply to all "copies" of a module on a site, no matter which page the module is on. 
    /// 
    /// TabModuleSettings apply only to the current module on the current page, if you copy that module to
    /// another page the settings are not transferred.
    /// 
    /// If you happen to save both TabModuleSettings and ModuleSettings, TabModuleSettings overrides ModuleSettings.
    /// 
    /// Below we have some examples of how to access these settings but you will need to uncomment to use.
    /// 
    /// Because the control inherits from ModuleSettingsSettingsBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Settings : ModuleSettingsBase
    {
        private ModuleSettings _settings;
        public ModuleSettings ModSettings
        {
            get { return _settings ?? (_settings = XEC.DNN.ModuleSettingsModule.Components.ModuleSettings.GetSettings(ModuleConfiguration)); }
            set { _settings = value; }
        }


        #region Base Method Implementations

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            try
            {
                if (!this.Page.IsPostBack)
                {
                    this.ddlSettingStatus.Items.AddRange(Enum.GetValues(typeof(Status))
                                                             .OfType<Status>()
                                                             .Select(arg => new ListItem(this.LocalizeString("Status_" + arg.ToString().ToLowerInvariant()), arg.ToString()))
                                                             .ToArray());

                    this.chkSettingInitialize.Checked = ModSettings.Initialize.GetValueOrDefault(false);
                    this.ddlSettingStatus.SelectedValue = ModSettings.Status.ToString();
                    this.txtCssClass.Text = ModSettings.CssClass;
                    this.txtSettingMaximumRetries.Text = ModSettings.MaximumRetries.ToString(CultureInfo.CurrentUICulture);
                    this.txtSettingUserName.Text = ModSettings.UserName;
                }
            }
            catch (Exception exception)
            {
                Exceptions.ProcessModuleLoadException(this, exception);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UpdateSettings()
        {
            try
            {
                // Option 1: Recommended practice. Will load the defaults as well!
                // var typedModuleSettings = persister.Load(this.Settings);

                // Option 2: Not recommended since it misses the default values!
                // var typedModuleSettings = new MyModuleSettingsInfo();

                // Retrieve the settings from the form
                ModSettings.Initialize = this.chkSettingInitialize.Checked;
                ModSettings.CssClass = this.txtCssClass.Text;

                var maximumRetries = 0;
                ModSettings.MaximumRetries = int.TryParse(this.txtSettingMaximumRetries.Text, NumberStyles.Integer, CultureInfo.CurrentUICulture, out maximumRetries) ? maximumRetries : 0;

                var status = Status.Unknown;
                ModSettings.Status = Enum.TryParse(this.ddlSettingStatus.SelectedValue, true, out status) ? status : Status.Unknown;
                ModSettings.UserName = this.txtSettingUserName.Text;

                ModSettings.Save(this.ModuleConfiguration);
                // Obviously the statement below works as well...
                // persister.Save(typedModuleSettings, this.ModuleId, this.TabModuleId);
            }
            catch (Exception exception)
            {
                Exceptions.ProcessModuleLoadException(this, exception);
            }
        }

        #endregion
    }
}