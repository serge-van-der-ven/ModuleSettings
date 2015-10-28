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

namespace DotNetNuke.Entities.Modules
{
    public abstract class DnnPortalModuleBase<TType> : PortalModuleBase
        where TType : class, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DnnPortalModuleBase{TType}"/> class.
        /// </summary>
        protected DnnPortalModuleBase()
        {
            this.Persister = new ModuleSettingPersister<TType>();
        }

        #endregion

        private TType _settings;
        /// <summary>
        /// The settings. Read only(!)
        /// </summary>
        public new TType Settings => this._settings ?? (this._settings = this.Persister.Load(this.ModuleConfiguration));

        protected ModuleSettingPersister<TType> Persister { get; }

        /// <summary>
        /// Saves the settings. Please note that module settings should in general be modified using the ModuleSettings component. This method is provided for fallback.
        /// </summary>
        public void SaveSettings()
        {
            this.Persister.Save(this.Settings, this.ModuleConfiguration);
        }
    }
}
