using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XEC.DNN.ModuleSettingsModule.Components
{
    public class ModuleBase : PortalModuleBase
    {

        private ModuleSettings _settings;
        public new ModuleSettings Settings
        {
            get { return _settings ?? (_settings = ModuleSettings.GetSettings(ModuleConfiguration)); }
            set { _settings = value; }
        }

    }
}