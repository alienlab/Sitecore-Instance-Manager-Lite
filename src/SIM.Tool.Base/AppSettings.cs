﻿namespace SIM.Tool.Base
{
  using Sitecore.Diagnostics.Annotations;

  public static class AppSettings
  {
    #region Fields

    [NotNull]
    public static readonly AdvancedProperty<string> AppBrowsersBackend = AdvancedSettings.Create("App/Browsers/Backend", "explorer.exe");

    [NotNull]
    public static readonly AdvancedProperty<string> AppBrowsersFrontend = AdvancedSettings.Create("App/Browsers/Frontend", "explorer.exe");

    [NotNull]
    public static readonly AdvancedProperty<string> AppLoginAsAdminPageUrl = AdvancedSettings.Create("App/LoginAsAdmin/PageUrl", "/sitecore");

    [NotNull]
    public static readonly AdvancedProperty<string> AppLoginAsAdminUserName = AdvancedSettings.Create("App/LoginAsAdmin/UserName", "sitecore\\admin");

    [NotNull]
    public static readonly AdvancedProperty<bool> AppPreheatEnabled = AdvancedSettings.Create("App/Preheat/Enabled", false);

    [NotNull]
    public static readonly AdvancedProperty<bool> AppUiHighDpiEnabled = AdvancedSettings.Create("App/UI/HighDPI/Enabled", false);

    [NotNull]
    public static readonly AdvancedProperty<bool> AppSysAllowMultipleInstances = AdvancedSettings.Create("App/Sys/AllowMultipleInstances", false);

    [NotNull]
    public static readonly AdvancedProperty<bool> AppSysIsSingleThreaded = AdvancedSettings.Create("App/Sys/IsSingleThreaded", false);

    [NotNull]
    public static readonly AdvancedProperty<string> AppToolsLogViewer = AdvancedSettings.Create("App/Tools/LogViewer", "logview.exe");
    
    #endregion
  }
}