﻿namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Windows.Dialogs;
  using Sitecore.Diagnostics.Annotations;

  [UsedImplicitly]
  public class AboutButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      WindowHelper.ShowDialog<AboutDialog>(null, mainWindow);
    }

    #endregion
  }
}