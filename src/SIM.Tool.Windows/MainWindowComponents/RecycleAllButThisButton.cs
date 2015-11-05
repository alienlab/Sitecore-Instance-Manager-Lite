namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Linq;
  using System.Windows;
  using SIM.Instances;
  
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  using SIM.Tool.Base;

  [UsedImplicitly]
  public class RecycleAllButThisButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      var instances = InstanceManager.Instances;
      Assert.IsNotNull(instances, "instances");

      var otherInstances = instances.Where(x => x.ID != instance.ID);
      foreach (var otherInstance in otherInstances)
      {
        try
        {
          if (otherInstance == null)
          {
            continue;
          }

          Log.Info("Recycling instance " + otherInstance, this);
          otherInstance.Recycle();
        }
        catch (Exception ex)
        {
          Log.Warn("An error occurred", this, ex);
        }
      }
    }

    #endregion
  }
}