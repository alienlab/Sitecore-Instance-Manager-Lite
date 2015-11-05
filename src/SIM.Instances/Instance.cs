namespace SIM.Instances
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using System.Xml.Schema;
  using System.Xml.Serialization;
  using Microsoft.Web.Administration;

  using SIM.Adapters.WebServer;
  using SIM.Instances.RuntimeSettings;

  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  [Serializable]
  public class Instance : Website, IXmlSerializable
  {
    #region Static constants, fields, properties and methods

    #region Fields

    public static int fld;
    private static int fld22;

    #endregion

    #region Public methods

    public static void DoSmth()
    {
    }

    #endregion

    #endregion

    #region Instance fields

    protected RuntimeSettingsAccessor runtimeSettingsAccessor;
    private InstanceConfiguration configuration;

    #endregion

    #region Constructors

    public Instance([NotNull] int id)
      : base(id)
    {
    }


    protected Instance()
    {
    }

    #endregion

    #region Non-public Properties

    [NotNull]
    protected virtual RuntimeSettingsAccessor RuntimeSettingsAccessor
    {
      get
      {
        return this.runtimeSettingsAccessor ??
               (this.runtimeSettingsAccessor = RuntimeSettingsManager.GetRealtimeSettingsAccessor(this));
      }
    }

    #endregion

    #region Public Properties

    #region Public properties

    [NotNull]
    public virtual InstanceConfiguration Configuration
    {
      get
      {
        return this.configuration ?? (this.configuration = new InstanceConfiguration(this));
      }
    }

    public virtual string CurrentLogFilePath
    {
      get
      {
        return this.GetCurrentLogFilePath();
      }
    }

    public virtual string DataFolderPath
    {
      get
      {
        return this.GetDataFolderPath();
      }
    }

    [CanBeNull]
    public virtual string IndexesFolderPath
    {
      get
      {
        try
        {
          var indexFolder = this.RuntimeSettingsAccessor.GetSitecoreSettingValue("IndexFolder");
          Assert.IsNotNull(indexFolder, "The <setting name=\"IndexFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(indexFolder, "The <setting name=\"IndexFolder\" value=\"...\" /> value is empty string");
          return this.MapPath(indexFolder);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(this.DataFolderPath, "Indexes");
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error("Cannot get indexes folder of " + this.WebRootPath, this, ex);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get indexes folder of " + this.WebRootPath, ex);
        }
      }
    }

    public virtual bool IsSitecore
    {
      get
      {
        return this.GetIsSitecore();
      }
    }

    [CanBeNull]
    public virtual string LicencePath
    {
      get
      {
        try
        {
          var licenseFile = this.RuntimeSettingsAccessor.GetSitecoreSettingValue("LicenseFile");
          Assert.IsNotNull(licenseFile, "The <setting name=\"LicenseFile\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(licenseFile, "The <setting name=\"LicenseFile\" value=\"...\" /> value is empty string");
          return this.MapPath(licenseFile);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(this.DataFolderPath, "license.xml");
          if (FileSystem.FileSystem.Local.File.Exists(rootData))
          {
            Log.Error("Cannot get license file of " + this.WebRootPath, this, ex);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get license file of " + this.WebRootPath, ex);
        }
      }
    }

    [NotNull]
    public virtual string LogsFolderPath
    {
      get
      {
        return this.GetLogsFolderPath();
      }
    }

    [CanBeNull]
    public virtual string PackagesFolderPath
    {
      get
      {
        try
        {
          var packagePath = this.RuntimeSettingsAccessor.GetSitecoreSettingValue("PackagePath");
          Assert.IsNotNull(packagePath, "The <setting name=\"PackagePath\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(packagePath, "The <setting name=\"PackagePath\" value=\"...\" /> value is empty string");
          return this.MapPath(packagePath);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(this.DataFolderPath, "Packages");
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error("Cannot get packages folder of " + this.WebRootPath, this, ex);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get packages folder of " + this.WebRootPath, ex);
        }
      }
    }
    
    [CanBeNull]
    public virtual string SerializationFolderPath
    {
      get
      {
        try
        {
          var serializationFolder = this.RuntimeSettingsAccessor.GetSitecoreSettingValue("SerializationFolder");
          Assert.IsNotNull(serializationFolder, "The <setting name=\"dataserializationFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(serializationFolder, "The <setting name=\"dataserializationFolder\" value=\"...\" /> value is empty string");
          return this.MapPath(serializationFolder);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(this.DataFolderPath, "Serialization");
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error("Cannot get serialization folder of " + this.WebRootPath, this, ex);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get serialization folder of " + this.WebRootPath, ex);
        }
      }
    }

    public InstanceState State
    {
      get
      {
        if (this.IsDisabled)
        {
          return InstanceState.Disabled;
        }

        if (this.ApplicationPoolState == ObjectState.Stopped || this.ApplicationPoolState == ObjectState.Stopping)
        {
          return InstanceState.Stopped;
        }

        if (this.ProcessIds.Any())
        {
          return InstanceState.Running;
        }

        return InstanceState.Ready;
      }
    }

    public virtual bool SupportsCaching
    {
      get
      {
        return false;
      }
    }

    public virtual string TempFolderPath
    {
      get
      {
        return this.GetTempFolderPath();
      }
    }

    #endregion

    #endregion

    #region IXmlSerializable Members

    public virtual XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }

    public virtual void ReadXml(XmlReader reader)
    {
      throw new NotImplementedException();
    }

    public virtual void WriteXml(XmlWriter writer)
    {
      foreach (var property in this.GetType().GetProperties())
      {
        object value = property.GetValue(this, new object[0]);
        var xml = value as XmlDocument;
        if (xml != null)
        {
          writer.WriteNode(new XmlNodeReader(XmlDocumentEx.LoadXml("<Instance>" + xml.OuterXml + "</Instance>")), false);
          continue;
        }

        writer.WriteElementString(property.Name, string.Empty, (value ?? string.Empty).ToString());
      }

      ;
    }

    #endregion

    #region Public Methods

    #region Public properties

    public virtual string DisplayName
    {
      get
      {
        return string.Format("{0}", base.ToString());
      }
    }

    public bool IsDisabled
    {
      get
      {
        return this.Name.ToLowerInvariant().EndsWith("_disabled");
      }

      set
      {
        var name = this.Name.TrimEnd("_disabled");
        using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Website({0}).Name".FormatWith(this.ID)))
        {
          context.Sites[name].Name = name + "_disabled";
          context.CommitChanges();
        }
      }
    }

    [UsedImplicitly]
    public virtual string ModulesNames
    {
      get
      {
        // TODO: replace with modules detector implemenation
        var modulesNames = Directory.GetFiles(this.PackagesFolderPath, "*.zip").Select(x => Path.GetFileNameWithoutExtension(x.TrimStart("Sitecore ")));
        return (string.Join(", ", modulesNames) + (File.Exists(Path.Combine(this.WebRootPath, "App_Config\\Include\\Sitecore.Analytics.config")) ? ", DMS" : string.Empty)).TrimStart(" ,".ToCharArray());
      }
    }

    #endregion

    #region Public methods

    [NotNull]
    public virtual XmlDocument GetShowconfig(bool normalize = false)
    {
      using (new ProfileSection("Get showconfig.xml config", this))
      {
        ProfileSection.Argument("normalize", normalize);

        return this.RuntimeSettingsAccessor.GetShowconfig(normalize);
      }
    }
    public virtual XmlDocument GetWebResultConfig(bool normalize = false)
    {
      using (new ProfileSection("Get web.config.result.xml config", this))
      {
        ProfileSection.Argument("normalize", normalize);

        return this.RuntimeSettingsAccessor.GetWebConfigResult(normalize);
      }
    }

    public override string ToString()
    {
      return this.DisplayName;
    }

    #endregion

    #endregion

    #region Non-public Methods

    #region Public methods

    public virtual string GetCurrentLogFilePath()
    {
      using (new ProfileSection("Get current log file path", this))
      {
        var logs = this.LogsFolderPath;
        var files = FileSystem.FileSystem.Local.Directory.GetFiles(logs, "log*.txt").OrderBy(FileSystem.FileSystem.Local.File.GetCreationTimeUtc);
        string lastOrDefault = files.LastOrDefault();

        return ProfileSection.Result(lastOrDefault);
      }
    }

    #endregion

    #region Protected methods

    [NotNull]
    protected virtual string GetDataFolderPath()
    {
      using (new ProfileSection("Get data folder path", this))
      {
        try
        {
          string dataFolder = this.RuntimeSettingsAccessor.GetScVariableValue("dataFolder");
          Assert.IsNotNull(dataFolder, "The <sc.variable name=\"dataFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(dataFolder, "The <sc.variable name=\"dataFolder\" value=\"...\" /> element value is empty string");

          return this.MapPath(dataFolder);
        }
        catch (Exception ex)
        {
          var rootData = Path.Combine(Path.GetDirectoryName(this.WebRootPath), "Data");
          if (FileSystem.FileSystem.Local.Directory.Exists(rootData))
          {
            Log.Error("Cannot get data folder of " + this.WebRootPath, this, ex);

            return rootData;
          }

          throw new InvalidOperationException("Cannot get data folder of " + this.WebRootPath, ex);
        }
      }
    }

    protected virtual bool GetIsSitecore()
    {
      try
      {
        return FileSystem.FileSystem.Local.File.Exists(Path.Combine(this.WebRootPath, ""));
      }
      catch (Exception ex)
      {
        Log.Warn("An error occurred during checking if it is sitecore", this, ex);

        return false;
      }
    }

    protected virtual string GetLogsFolderPath()
    {
      using (new ProfileSection("Get log folder path", this))
      {
        try
        {
          string dataFolder = this.RuntimeSettingsAccessor.GetSitecoreSettingValue("LogFolder");
          var result = this.MapPath(dataFolder);

          return ProfileSection.Result(result);
        }
        catch (Exception ex)
        {
          var dataLogs = Path.Combine(this.DataFolderPath, "logs");
          if (FileSystem.FileSystem.Local.Directory.Exists(dataLogs))
          {
            Log.Error("Cannot get logs folder of " + this.WebRootPath, this, ex);

            return dataLogs;
          }

          throw new InvalidOperationException("Cannot get logs folder of " + this.WebRootPath, ex);
        }
      }
    }

    [NotNull]
    protected virtual string GetTempFolderPath()
    {
      using (new ProfileSection("Get temp folder path", this))
      {
        try
        {
          string tempFolder = this.RuntimeSettingsAccessor.GetScVariableValue("tempFolder");
          Assert.IsNotNull(tempFolder, "The <sc.variable name=\"tempFolder\" value=\"...\" /> element is not presented in the web.config file");
          Assert.IsNotNullOrEmpty(tempFolder, "The <sc.variable name=\"tempFolder\" value=\"...\" /> element value is empty string");

          var result = this.MapPath(tempFolder);

          return ProfileSection.Result(result);
        }
        catch (Exception ex)
        {
          var websiteTemp = Path.Combine(this.WebRootPath, "temp");
          if (FileSystem.FileSystem.Local.Directory.Exists(websiteTemp))
          {
            Log.Error("Cannot get temp folder of " + this.WebRootPath, this, ex);

            return websiteTemp;
          }

          throw new InvalidOperationException("Cannot get temp folder of " + this.WebRootPath, ex);
        }
      }
    }

    protected virtual string MapPath(string virtualPath)
    {
      return FileSystem.FileSystem.Local.Directory.MapPath(virtualPath, this.WebRootPath);
    }

    #endregion

    #endregion
  }
}