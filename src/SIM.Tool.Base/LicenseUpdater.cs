namespace SIM.Tool.Base
{
  using System;
  using System.Windows;
  using System.Windows.Forms;
  using SIM.Instances;

  public static class LicenseUpdater
  {
    #region Constants

    private const string Title = "Update license";

    #endregion

    #region Public methods

    public static void Update(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        const string current = "Selected instance";
        const string all = "All instances";
        var options = new[]
        {
          current, all
        };

        var result = WindowHelper.AskForSelection(Title, null, "You have selected \"{0}\" Sitecore instance. \nWould you like to update the license file only there?".FormatWith(instance.Name), options, mainWindow);

        if (result == null)
        {
          return;
        }

        if (result == all)
        {
          instance = null;
        }
      }
      var openDialog = new OpenFileDialog
      {
        Filter = @"License files|*.xml"
      };

      if (openDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
      {
        return;
      }

      var filePath = openDialog.FileName;

      WindowHelper.LongRunningTask(() => DoUpdateLicense(filePath, instance), "Updating license...", mainWindow);
    }

    #endregion

    #region Private methods

    private static void DoUpdateLicense(string licenseFilePath, Instance instance)
    {
      if (instance != null)
      {
        try
        {
          FileSystem.FileSystem.Local.File.Copy(licenseFilePath, instance.LicencePath, true);
        }
        catch (Exception ex)
        {
          Log.Error(ex.Message, typeof(LicenseUpdater), ex);
        }
      }
      else
      {
        foreach (Instance inst in InstanceManager.Instances)
        {
          try
          {
            FileSystem.FileSystem.Local.File.Copy(licenseFilePath, inst.LicencePath, true);
          }
          catch (Exception ex)
          {
            Log.Error(ex.Message, typeof(LicenseUpdater), ex);
          }
        }
      }
    }

    #endregion
  }
}