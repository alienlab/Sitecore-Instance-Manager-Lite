﻿namespace SIM.Tool.Base.Profiles
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  public class Profile : DataObject, ICloneable
  {
    #region Properties

    #region Public properties

    [NotNull]
    public virtual string AdvancedSettings
    {
      get
      {
        return this.GetString("AdvancedSettings") ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.SetValue("AdvancedSettings", value);
      }
    }

    [NotNull]
    public virtual string ConnectionString
    {
      get
      {
        return this.GetString("ConnectionString") ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.SetValue("ConnectionString", value);
      }
    }

    [NotNull]
    public virtual string InstancesFolder
    {
      get
      {
        return this.GetString("InstancesFolder") ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.SetValue("InstancesFolder", value);
      }
    }

    [NotNull]
    public virtual string License
    {
      get
      {
        return this.GetString("License") ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.SetValue("License", value);
      }
    }

    [NotNull]
    public virtual string LocalRepository
    {
      get
      {
        return this.GetString("LocalRepository") ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.SetValue("LocalRepository", value);
      }
    }

    [NotNull]
    public string Plugins
    {
      get
      {
        return this.GetString("Plugins") ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.SetValue("Plugins", value);
      }
    }

    #endregion

    #region Protected methods

    protected void ValidateInstancesFolder()
    {
      Assert.IsNotNullOrEmpty(this.InstancesFolder, "The InstancesFolder is null or empty");
      FileSystem.FileSystem.Local.Directory.AssertExists(this.InstancesFolder, string.Format("The InstancesFolder does not exist ({0})", this.InstancesFolder));
    }

    protected void ValidateLicense()
    {
      Assert.IsNotNullOrEmpty(this.License, "The license file isn't set");
      FileSystem.FileSystem.Local.File.AssertExists(this.License, "The license file doesn't exist");
    }

    #endregion

    #endregion

    #region Implemented Interfaces

    #region ICloneable

    public object Clone()
    {
      return new Profile
      {
        ConnectionString = this.ConnectionString, 
        InstancesFolder = this.InstancesFolder, 
        License = this.License, 
        LocalRepository = this.LocalRepository, 
        Plugins = this.Plugins
      };
    }

    #endregion

    #endregion
  }
}