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
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using XEC.DNN.ModuleSettings.Components;

namespace XEC.DNN.ModuleSettings
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from ModuleSettingsModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : PortalModuleBase
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var persister = new ModuleSettingPersister<MyModuleSettingsInfo>();
                var typedModuleSettings = persister.Load(this.Settings);

                // ReSharper disable once AssignmentInConditionalExpression
                if (this.pnlSettings.Visible = typedModuleSettings.IsInitialized)
                {
                    this.lblModuleInitializedMessage.Text = this.LocalizeString("ModuleInitializedMessage");


                    this.lblSettingCssClassValue.Text = typedModuleSettings.CssClass;
                    if (typedModuleSettings.Initialize.HasValue)
                    {
                        this.chkSettingInitialize.Checked = typedModuleSettings.Initialize.Value;
                    }
                    else
                    {
                        this.chkSettingInitialize.Enabled = false;
                    }

                    this.lblSettingMaximumRetriesValue.Text = typedModuleSettings.MaximumRetries.ToString(CultureInfo.CurrentUICulture);
                    this.lblSettingStatusValue.Text = typedModuleSettings.Status.ToString();
                    this.lblSettingUserNameValue.Text = typedModuleSettings.UserName;
                }
                else
                {
                    this.lblModuleInitializedMessage.Text = this.LocalizeString("ModuleNotInitializedMessage");
                }
            }
            catch (Exception exception)
            {
                Exceptions.ProcessModuleLoadException(this, exception);
            }
        }
    }
}