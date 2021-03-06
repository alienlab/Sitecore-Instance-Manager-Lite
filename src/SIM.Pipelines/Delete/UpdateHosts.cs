﻿namespace SIM.Pipelines.Delete
{
  using SIM.Adapters.WebServer;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class UpdateHosts : DeleteProcessor
  {
    #region Methods

    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Hosts.Remove(args.InstanceHostNames);
    }

    #endregion
  }
}