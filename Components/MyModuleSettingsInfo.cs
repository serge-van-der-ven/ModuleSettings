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

using DotNetNuke.Entities.Modules.Settings;

namespace XEC.DNN.ModuleSettings.Components
{
    /// <summary>
    /// A demo POCO used to get and set DNN module related settings like TabModuleSettings, ModuleSettings and PortalSettings. Note that all property names are indicative 
    /// and used only for demonstration purposes.
    /// The IParameterSetupHandler is an optional interface and can be used for complex initialization either before or after loading the settings.
    /// </summary>
    public class MyModuleSettingsInfo : IParameterSetupHandler
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
        public bool IsInitialized => !string.IsNullOrWhiteSpace(this.UserName);

        #region Implementation of IParameterSetupHandler

        public bool SetupBeforeLoad => true;

        public void Setup()
        {
            // Do additional setup here...
            // Notes:
            // 1. IParameterSetupHandler is an optional interface. Only needed if the basic default values as provided by the attributes is not sufficient and additional setup is needed
            //    after the values have been loaded.
            // 2. Basic inti


        }

        #endregion
    }
}