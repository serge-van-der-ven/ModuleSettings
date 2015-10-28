using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetNuke.Entities.Modules.Settings
{
    public interface IParameterSetupHandler
    {
        bool SetupBeforeLoad { get; }

        void Setup();
    }
}
