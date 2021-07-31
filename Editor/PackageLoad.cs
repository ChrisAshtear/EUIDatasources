using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Packages.ChrisAsh.EUI.DataSources.Editor
{
    //Make sure all data sources are loaded when recompiling code in the editor
    public static class PackageSetup
    {
        [InitializeOnLoadMethod]
        public static void DoSetup()
        {
            DataSourceInit.setupSettings();
            DataSourceInit.setupSources();
        }
    }
}
