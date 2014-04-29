using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.IO;

namespace WideRuled2
{
    
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string VERSION = "3.61";
        public static string CurrentWorkingDirectory;

        protected override void OnStartup(StartupEventArgs e)
        {
 
            base.OnStartup(e);
            Xceed.Wpf.DataGrid.Licenser.LicenseKey = "DGF20-AUTJ7-3K8MD-DNNA";
            //CurrentWorkingDirectory = Directory.GetCurrentDirectory();
            CurrentWorkingDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
        }
    }

  
}
