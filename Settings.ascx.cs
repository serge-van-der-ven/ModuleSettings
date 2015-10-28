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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using XEC.DNN.ModuleSettings.Components;

namespace XEC.DNN.ModuleSettings
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
                    var persister = new ModuleSettingPersister<MyModuleSettingsInfo>();
                    var typedSettings = persister.Load(this.Settings);

                    this.chkSettingInitialize.Checked = typedSettings.Initialize.GetValueOrDefault(false);
                    this.ddlSettingStatus.SelectedValue = typedSettings.Status.ToString();
                    this.txtCssClass.Text = typedSettings.CssClass;
                    this.txtSettingMaximumRetries.Text = typedSettings.MaximumRetries.ToString(CultureInfo.CurrentUICulture);
                    this.txtSettingUserName.Text = typedSettings.UserName;
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
                var persister = new ModuleSettingPersister<MyModuleSettingsInfo>();

                // Option 1:
                var typedModuleSettings = persister.Load(this.Settings);

                // Option 2:
                // var typedModuleSettings = new MyModuleSettingsInfo();

                // Retrieve the settings from the form
                typedModuleSettings.Initialize = this.chkSettingInitialize.Checked;
                typedModuleSettings.CssClass = this.txtCssClass.Text;

                var maximumRetries = 0;
                typedModuleSettings.MaximumRetries = int.TryParse(this.txtSettingMaximumRetries.Text, NumberStyles.Integer, CultureInfo.CurrentUICulture, out maximumRetries) ? maximumRetries : 0;

                var status = Status.Unknown;
                typedModuleSettings.Status = Enum.TryParse(this.ddlSettingStatus.SelectedValue, true, out status) ? status : Status.Unknown;
                typedModuleSettings.UserName = this.txtSettingUserName.Text;

                persister.Save(typedModuleSettings, this.ModuleContext.ModuleId, this.ModuleContext.TabModuleId);
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