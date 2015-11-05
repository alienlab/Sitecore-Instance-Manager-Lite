namespace SIM.Instances
{
  #region

  using System.IO;
  using System.Linq;
  using System.Xml;
  using SIM.Adapters.WebServer;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #endregion

  public sealed class InstanceConfiguration
  {
    #region Fields

    [NotNull]
    private readonly Instance instance;

    #endregion

    #region Constructors

    public InstanceConfiguration([NotNull] Instance instance)
    {
      Assert.ArgumentNotNull(instance, "instance");

      this.instance = instance;
    }

    #endregion

    #region Properties

    #region Public properties

    #endregion

    #endregion

    #region Methods

    private static XmlElementEx GetConnectionStringsElement(XmlDocumentEx webConfig)
    {
      var webRootPath = Path.GetDirectoryName(webConfig.FilePath);
      XmlElement configurationNode = webConfig.SelectSingleNode(WebConfig.ConfigurationXPath) as XmlElement;
      Assert.IsNotNull(configurationNode, 
        "The {0} element is missing in the {1} file".FormatWith("/configuration", webConfig.FilePath));
      XmlElement webConfigConnectionStrings = configurationNode.SelectSingleNode("connectionStrings") as XmlElement;
      Assert.IsNotNull(webConfigConnectionStrings, 
        "The web.config file doesn't contain the /configuration/connectionStrings node");
      XmlAttribute configSourceAttribute = webConfigConnectionStrings.Attributes[WebConfig.ConfigSourceAttributeName];
      if (configSourceAttribute != null)
      {
        string configSourceValue = configSourceAttribute.Value;
        if (!string.IsNullOrEmpty(configSourceValue) && !string.IsNullOrEmpty(webRootPath))
        {
          string filePath = Path.Combine(webRootPath, configSourceValue);
          if (FileSystem.FileSystem.Local.File.Exists(filePath))
          {
            XmlDocumentEx connectionStringsConfig = XmlDocumentEx.LoadFile(filePath);
            XmlElement connectionStrings = connectionStringsConfig.SelectSingleNode("/connectionStrings") as XmlElement;
            if (connectionStrings != null)
            {
              return new XmlElementEx(connectionStrings, connectionStringsConfig);
            }
          }
        }
      }

      return new XmlElementEx(webConfigConnectionStrings, webConfig);
    }

    [NotNull]
    private XmlElementEx GetConnectionStringsElement()
    {
      XmlDocumentEx webConfig = this.instance.GetWebConfig();
      Assert.IsNotNull(webConfig, "webConfig");

      return GetConnectionStringsElement(webConfig);
    }

    #endregion

    #region Public methods
    #endregion
  }
}