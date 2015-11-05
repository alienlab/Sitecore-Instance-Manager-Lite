﻿namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using SIM.Tool.Base;

  using Sitecore.Diagnostics.Annotations;

  [UsedImplicitly]
  public class OpenWebConfigButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        string webConfigPath = WebConfig.GetWebConfigPath(instance.WebRootPath);
        FileSystem.FileSystem.Local.File.AssertExists(webConfigPath, "The web.config file ({0}) of the {1} instance doesn't exist".FormatWith(webConfigPath, instance.Name));
        string editor = WindowsSettings.AppToolsConfigEditor.Value;
        if (!string.IsNullOrEmpty(editor))
        {
          WindowHelper.RunApp(editor, webConfigPath);
        }
        else
        {
          WindowHelper.OpenFile(webConfigPath);
        }
      }
    }

    #endregion
  }
}