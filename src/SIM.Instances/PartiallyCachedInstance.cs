namespace SIM.Instances
{
  using System;
  using System.IO;
  using System.Xml;
  using Sitecore.Diagnostics.Annotations;

  public sealed class PartiallyCachedInstance : Instance, IDisposable
  {
    #region Instance fields

    [CanBeNull]
    private readonly FileSystemWatcher appConfigWatcher;

    [CanBeNull]
    private readonly FileSystemWatcher webConfigWatcher;

    private string licencePath;

    [CanBeNull]
    private string modulesNamesCache;

    private string name;
    private string productFullName;

    [CanBeNull]
    private XmlDocument webConfigResultCache;

    private string webRootPath;

    #endregion

    #region Constructors
    
    #endregion

    #region Public Properties

    public override string LicencePath
    {
      get
      {
        return this.licencePath ?? (this.licencePath = base.LicencePath);
      }
    }

    public override string Name
    {
      get
      {
        return this.name ?? (this.name = base.Name);
      }
    }

    public override string WebRootPath
    {
      get
      {
        return this.webRootPath ?? (this.webRootPath = base.WebRootPath);
      }
    }

    #endregion

    #region Public properties

    public override string ModulesNames
    {
      get
      {
        return this.modulesNamesCache ?? (this.modulesNamesCache = base.ModulesNames);
      }
    }

    #endregion

    #region Public methods

    public void Dispose()
    {
      if (this.appConfigWatcher != null)
      {
        this.appConfigWatcher.EnableRaisingEvents = false;
      }

      if (this.webConfigWatcher != null)
      {
        this.webConfigWatcher.EnableRaisingEvents = false;
      }
    }

    public override XmlDocument GetWebResultConfig(bool normalize = false)
    {
      return this.webConfigResultCache ?? (this.webConfigResultCache = base.GetWebResultConfig(normalize));
    }

    #endregion

    #region Private methods

    private void ClearCache([CanBeNull] object sender, [CanBeNull] FileSystemEventArgs fileSystemEventArgs)
    {
      this.webConfigResultCache = null;
      this.modulesNamesCache = null;
    }

    #endregion
  }
}