﻿namespace SIM.Tool.Windows.UserControls.Download8
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.Pipelines.Download8;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;
  using Sitecore.Diagnsotics.InformationService.Client.Model;

  public class DownloadWizardArgs : WizardArgs
  {
    #region Fields

    [NotNull]
    private readonly List<ProductDownload8InCheckbox> products = new List<ProductDownload8InCheckbox>();

    #endregion

    #region Constructors

    [UsedImplicitly]
    public DownloadWizardArgs()
    {
    }

    public DownloadWizardArgs([NotNull] string username, [NotNull] string password)
    {
      Assert.ArgumentNotNull(username, "username");
      Assert.ArgumentNotNull(password, "password");
      this.UserName = username;
      this.Password = password;
    }

    #endregion

    #region Public properties

    public string Cookies { get; set; }

    public ReadOnlyCollection<Uri> Links { get; set; }

    public string Password { get; set; }

    [NotNull]
    public List<ProductDownload8InCheckbox> Products
    {
      get
      {
        return this.products;
      }
    }

    [CanBeNull]
    public IRelease[] Releases { get; set; }

    public string UserName { get; set; }

    #endregion

    #region Public methods

    [NotNull]
    public override ProcessorArgs ToProcessorArgs()
    {
      return new Download8Args(this.Cookies, this.Links, ProfileManager.Profile.LocalRepository);
    }

    #endregion
  }
}