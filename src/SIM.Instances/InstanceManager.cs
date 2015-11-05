namespace SIM.Instances
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Microsoft.Web.Administration;
  using SIM.Adapters.WebServer;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  public static class InstanceManager
  {
    #region Fields
    
    private static IEnumerable<Instance> instances;
    private static string instancesFolder;

    #endregion

    #region Properties

    #region Delegates

    public static event EventHandler InstancesListUpdated;

    #endregion

    #region Public properties

    [CanBeNull]
    public static IEnumerable<Instance> Instances
    {
      get
      {
        return instances;
      }

      private set
      {
        instances = value;
        OnInstancesListUpdated();
      }
    }

    #endregion

    #endregion

    #region Public Methods

    #region Public methods

    [CanBeNull]
    public static Instance GetInstance([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");
      Log.Debug("InstanceManager:GetInstance('{0}')".FormatWith(name));

      Initialize();
      if (Instances == null)
      {
        return null;
      }

      return Instances.SingleOrDefault(i => i.Name.EqualsIgnoreCase(name));
    }

    public static void Initialize()
    {
      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("Initialize instance manager", typeof(InstanceManager)))
      {
        var sites = GetOperableSites(context);
        Instances = sites.Select(x => new Instance((int)x.Id)).ToArray();
      }
    }

    public static void InitializeWithSoftListRefresh()
    {
      using (new ProfileSection("Initialize with soft list refresh", typeof(InstanceManager)))
      {
        // Add check that this isn't an initial initialization
        if (Instances == null)
        {
          Initialize();
        }
      }
    }

    #endregion

    #region Private methods

    #endregion

    #endregion

    #region Methods

    [NotNull]
    private static Instance GetInstance([NotNull] Site site)
    {
      Assert.ArgumentNotNull(site, "site");

      int id = (Int32)site.Id;
      Log.Debug("InstanceManager:GetInstance(Site: {0})".FormatWith(site.Id));
      return new Instance(id);
    }

    private static IEnumerable<Site> GetOperableSites([NotNull] WebServerManager.WebServerContext context)
    { 
      Assert.IsNotNull(context, "Context cannot be null");

      using (new ProfileSection("Getting operable sites", typeof(InstanceManager)))
      {
        ProfileSection.Argument("context", context);
        
        IEnumerable<Site> sites = context.Sites;

        return ProfileSection.Result(sites);
      }
    }
    
    private static bool IsSitecore([CanBeNull] Instance instance)
    {
      return instance != null && instance.IsSitecore;
    }

    private static void OnInstancesListUpdated()
    {
      EventHandler handler = InstancesListUpdated;
      if (handler != null)
      {
        handler(null, EventArgs.Empty);
      }
    }

    #endregion

    #region Public methods

    public static Instance GetInstance(long id)
    {
      using (new ProfileSection("Get instance by id", typeof(InstanceManager)))
      {
        ProfileSection.Argument("id", id);

        var instance = new Instance((int)id);

        return ProfileSection.Result(instance);
      }
    }

    #endregion

    #region Nested type: Settings
    #endregion
  }
}