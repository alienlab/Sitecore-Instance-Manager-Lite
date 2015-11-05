namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Windows;
  using SIM.Instances;
  
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  using SIM.Tool.Base;

  using TaskDialogInterop;

  [UsedImplicitly]
  public class RefreshButton : IMainWindowButton
  {
    #region Enums

    #endregion

    #region Constructors

    public RefreshButton([NotNull] string param)
    {
      Assert.ArgumentNotNull(param, "param");

    }

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      using (new ProfileSection("Refresh main window instances", this))
      {
        ProfileSection.Argument("mainWindow", mainWindow);
        ProfileSection.Argument("instance", instance);

        MainWindowHelper.RefreshEverything();
      }
    }

    #endregion

    #region Private methods

    #endregion
  }
}