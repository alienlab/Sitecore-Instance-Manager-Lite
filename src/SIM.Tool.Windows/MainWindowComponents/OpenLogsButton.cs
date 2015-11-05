﻿using System.IO;
using System.Windows;
using SIM.Instances;
using SIM.Tool.Base;

using Sitecore.Diagnostics.Annotations;

namespace SIM.Tool.Windows.MainWindowComponents
{
  [UsedImplicitly]
  public class OpenLogsButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      var appFilePath = ApplicationManager.GetEmbeddedApp("Log Analyzer.zip", "SIM.Tool.Plugins.LogAnalyzer", "SitecoreLogAnalyzer.exe");
      if (instance != null)
      {
        string dataFolderPath = instance.DataFolderPath;
        FileSystem.FileSystem.Local.Directory.AssertExists(dataFolderPath, "The data folder ({0}) of the {1} instance doesn't exist".FormatWith(dataFolderPath, instance.Name));

        var logs = Path.Combine(dataFolderPath, "logs");
        WindowHelper.RunApp(appFilePath, logs);

        return;
      }

      WindowHelper.RunApp(appFilePath);
    }

    #endregion
  }
}