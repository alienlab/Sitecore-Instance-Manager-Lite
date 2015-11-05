namespace SIM.Adapters.WebServer
{
  #region

  using System;
  using System.Data.SqlClient;
  using System.IO;
  using System.Xml;

  using SIM.Properties;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #endregion

  public static class WebConfig
  {
    #region Constants

    public const string ConfigSourceAttributeName = "configSource";

    public const string ConfigurationXPath = "/configuration";

    public const string ScVariableXPath = ConfigurationXPath + "/sitecore/sc.variable[@name='{0}']";

    private const string SettingXPath = ConfigurationXPath + "/sitecore/settings/setting[@name='{0}']";

    #endregion

    #region Public Methods

    [CanBeNull]
    public static string GetScVariable([NotNull] XmlDocument webConfig, [NotNull] string variableName)
    {
      Assert.ArgumentNotNull(webConfig, "webConfig");
      Assert.ArgumentNotNull(variableName, "variableName");

      using (new ProfileSection("Get Sitecore variable", typeof(WebConfig)))
      {
        ProfileSection.Argument("webConfig", webConfig);
        ProfileSection.Argument("variableName", variableName);

        try
        {
          XmlElement dataFolderNode = GetScVariableElement(webConfig, variableName);
          if (dataFolderNode != null)
          {
            XmlAttribute valueAttr = dataFolderNode.Attributes["value"];
            if (valueAttr != null)
            {
              var result = valueAttr.Value;

              return ProfileSection.Result(result);
            }
          }
        }
        catch (Exception ex)
        {
          Log.Warn("Cannot get sc variable {0}".FormatWith(variableName), typeof(WebConfig), ex);
        }

        return ProfileSection.Result<string>(null);
      }
    }

    [CanBeNull]
    public static XmlElement GetScVariableElement([NotNull] XmlDocument webConfig, [NotNull] string elementName)
    {
      Assert.ArgumentNotNull(webConfig, "webConfig");
      Assert.ArgumentNotNull(elementName, "elementName");

      return webConfig.SelectSingleNode(ScVariableXPath.FormatWith(elementName)) as XmlElement;
    }

    [NotNull]
    public static XmlDocumentEx GetWebConfig([NotNull] string webRootPath)
    {
      Assert.ArgumentNotNull(webRootPath, "webRootPath");
      string path = GetWebConfigPath(webRootPath);
      FileSystem.FileSystem.Local.File.AssertExists(path, BResources.FileIsMissing.FormatWith(path));

      try
      {
        return XmlDocumentEx.LoadFile(path);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(BResources.XmlFileIsCorrupted.FormatWith(path), ex);
      }
    }

    #endregion

    #region Methods

    #region Public methods
    
    [NotNull]
    public static string GetWebConfigPath([NotNull] string webRootPath)
    {
      Assert.ArgumentNotNull(webRootPath, "webRootPath");

      return Path.Combine(webRootPath, "web.config");
    }

    #endregion

    #region Private methods

    [CanBeNull]
    private static string GetDatabaseName([NotNull] SqlConnectionStringBuilder connectionString)
    {
      Assert.ArgumentNotNull(connectionString, "connectionString");

      return connectionString.InitialCatalog;
    }

    #endregion

    #endregion

    #region Public methods

    public static string GetSitecoreSetting(string name, XmlDocument webConfigResult)
    {
      Assert.ArgumentNotNull(webConfigResult, "webConfigResult");

      XmlElement element = webConfigResult.SelectSingleNode(string.Format(SettingXPath, name)) as XmlElement;
      Assert.IsNotNull(element, string.Format("The \"{0}\" setting is missing in the instance configuration files", name));
      XmlAttribute value = element.Attributes["value"];
      Assert.IsNotNull(value, string.Format("The value attribute of the \"{0}\" setting is missing in the instance configuration files", name));
      var settingValue = value.Value;
      Assert.IsNotNullOrEmpty(settingValue, "settingValue");
      return settingValue;
    }

    #endregion
  }
}