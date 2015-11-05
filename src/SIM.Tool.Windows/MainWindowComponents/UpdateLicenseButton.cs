﻿namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  
  using Sitecore.Diagnostics.Annotations;

  [UsedImplicitly]
  public class UpdateLicenseButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      LicenseUpdater.Update(mainWindow, instance);
    }

    #endregion
  }
}