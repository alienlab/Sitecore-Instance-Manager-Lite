namespace SIM.Tool.Base
{
  using System.Windows;

  using Sitecore.Diagnostics.Annotations;

  using SIM.Instances;

  public interface IMainWindowButton
  {
    #region Public methods

    bool IsEnabled([NotNull] Window mainWindow, [CanBeNull] Instance instance);

    void OnClick([NotNull] Window mainWindow, [CanBeNull] Instance instance);

    #endregion
  }
}