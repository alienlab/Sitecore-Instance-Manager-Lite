﻿namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Linq;
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Annotations;

  [UsedImplicitly]
  public class BrowseButton : IMainWindowButton
  {
    #region Fields

    [CanBeNull]
    protected readonly string Browser;

    [NotNull]
    protected readonly string VirtualPath;

    [NotNull]
    private string[] Params;

    #endregion

    #region Constructors

    public BrowseButton()
    {
      this.VirtualPath = string.Empty;
      this.Browser = null;
      this.Params = new string[0];
    }

    public BrowseButton(string param)
    {
      var arr = (param + ":").Split(':');
      this.VirtualPath = arr[0];
      this.Browser = arr[1];
      this.Params = arr.Skip(2).ToArray();
    }

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        if (!InstanceHelperEx.PreheatInstance(instance, mainWindow))
        {
          return;
        }

        InstanceHelperEx.BrowseInstance(instance, mainWindow, this.VirtualPath, true, this.Browser, this.Params);
      }
    }

    #endregion
  }
}