﻿namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  [UsedImplicitly]
  public class OpenCurrentLogButton : IMainWindowButton
  {
    #region Fields

    [CanBeNull]
    private readonly string logFileType;

    #endregion

    #region Constructors

    public OpenCurrentLogButton()
    {
    }

    public OpenCurrentLogButton([NotNull] string param)
    {
      Assert.ArgumentNotNull(param, "param");

      this.logFileType = param;
    }

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      if (instance != null)
      {
        InstanceHelperEx.OpenCurrentLogFile(instance, mainWindow, this.logFileType);
      }
    }

    #endregion
  }
}