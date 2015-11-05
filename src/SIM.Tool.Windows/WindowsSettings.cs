namespace SIM.Tool.Windows
{
  using System;
  using Sitecore.Diagnostics.Annotations;

  public static class WindowsSettings
  {
    #region Fields
    
    [NotNull]
    public static readonly AdvancedProperty<bool> AppInstanceSearchEnabled = AdvancedSettings.Create("App/InstanceSearch/Enabled", true);

    [NotNull]
    public static readonly AdvancedProperty<int> AppInstanceSearchTimeout = AdvancedSettings.Create("App/InstanceSearch/Timeout", 300);

    [NotNull]
    public static readonly AdvancedProperty<string> AppToolsConfigEditor = AdvancedSettings.Create("App/Tools/ConfigEditor", string.Empty);

    [NotNull]
    public static readonly AdvancedProperty<string> AppUiMainWindowDoubleClick = AdvancedSettings.Create("App/UI/InstanceDoubleClick", @"SIM.Tool.Windows.MainWindowComponents.BrowseButton, SIM.Tool.Windows");
    
    #endregion
  }
}