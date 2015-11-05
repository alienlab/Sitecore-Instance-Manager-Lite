#region Usings

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

using SIM.Tool.Base;
using SIM.Tool.Base.Runtime;
using SIM.Tool.Windows;

using Sitecore.Diagnostics;
using Sitecore.Diagnostics.Annotations;

#endregion

// ReSharper disable HeuristicUnreachableCode
// ReSharper disable CSharpWarnings::CS0162
namespace SIM.Tool
{
  public partial class App
  {
    #region Fields

    public static readonly int APP_DUPLICATE_EXIT_CODE = -8;
    public static readonly int APP_NO_MAIN_WINDOW = -44;
    public static readonly int APP_PIPELINES_ERROR = -22;
    private static readonly string AppLogsMessage = "The application will be suspended, look at the " + Log.LogFilePath + " log file to find out what has happened";

    #endregion

    #region Methods

    #region Public methods

    public static string GetLicensePath()
    {
      return Path.Combine(ApplicationManager.DataFolder, "license.xml");
    }

    public static string GetRepositoryPath()
    {
      string path = Path.Combine(ApplicationManager.DataFolder, "Repository");
      FileSystem.FileSystem.Local.Directory.Ensure(path);
      return path;
    }

    #endregion

    #region Protected methods

    protected override void OnExit(ExitEventArgs e)
    {
      LifeManager.ReleaseSingleInstanceLock();
      base.OnExit(e);
    }

    protected override void OnStartup([CanBeNull] StartupEventArgs e)
    {
      base.OnStartup(e);

      // Ensure we are running only one instance of process
      if (!App.AcquireSingleInstanceLock())
      {
        try
        {
          // Change exit code to some uniqueue value to recognize reason of the app closing
          LifeManager.ShutdownApplication(APP_DUPLICATE_EXIT_CODE);
        }
        catch (Exception ex)
        {
          Log.Error("An unhandled error occurred during shutting down", this, ex);
        }

        return;
      }

      try
      {
        // If this is restart, wait until the master instance exists.
        LifeManager.WaitUntilOriginalInstanceExits(e.Args);

        // Capture UI sync context. It will allow to invoke delegats on UI thread in more elegant way (rather than use Dispatcher directly).
        LifeManager.UISynchronizationContext = SynchronizationContext.Current;
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("An unhandled error occurred during LifeManager work", true, ex);
      }

      App.LogMainInfo();

      // Application is closing when it doesn't have any window instance therefore it's 
      // required to create MainWindow before creating the initial configuration dialog
      var main = App.CreateMainWindow();
      if (main == null)
      {
        LifeManager.ShutdownApplication(APP_NO_MAIN_WINDOW);
        return;
      }

      // Clean up garbage
      App.DeleteTempFolders();

      // Show main window
      try
      {
        main.Initialize();
        WindowHelper.ShowDialog(main, null);
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("Main window caused unhandled exception", true, ex, typeof(App));
      }
    }

    #endregion

    #region Private methods

    private static bool AcquireSingleInstanceLock()
    {
      using (new ProfileSection("Acquire single instance lock", typeof(App)))
      {
        try
        {
          return LifeManager.AcquireSingleInstanceLock();
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Error occurred during acquiring single instance lock", true, ex);

          return true;
        }
      }
    }

    private static MainWindow CreateMainWindow()
    {
      try
      {
        return new MainWindow();
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("The main window thrown an exception during creation. " + AppLogsMessage, true, ex, typeof(App));
        return null;
      }
    }

    private static void DeleteTempFolders()
    {
      try
      {
        FileSystem.FileSystem.Local.Directory.DeleteTempFolders();
      }
      catch (Exception ex)
      {
        Log.Error("Deleting temp folders caused an exception", typeof(App), ex);
      }
    }

    private static void LogMainInfo()
    {
      try
      {
        Log.Info("**********************************************************************", typeof(App));
        Log.Info("**********************************************************************", typeof(App));
        Log.Info("Sitecore Instance Manager Lite started", typeof(App));
        Log.Info("Version: " + ApplicationManager.AppVersion, typeof(App));
        Log.Info("Revison: " + ApplicationManager.AppRevision, typeof(App));
        Log.Info("Label: " + ApplicationManager.AppLabel, typeof(App));
        var nativeArgs = Environment.GetCommandLineArgs();
        string[] commandLineArgs = nativeArgs.Skip(1).ToArray();
        string argsToLog = commandLineArgs.Length > 0 ? string.Join("|", commandLineArgs) : "<NO ARGUMENTS>";
        Log.Info("Executable: " + (nativeArgs.FirstOrDefault() ?? string.Empty), typeof(App));
        Log.Info("Arguments: " + argsToLog, typeof(App));
        Log.Info("Directory: " + Environment.CurrentDirectory, typeof(App));
        Log.Info("**********************************************************************", typeof(App));
        Log.Info("**********************************************************************", typeof(App));
      }
      catch
      {
        Debug.WriteLine("Error during log main info");
      }
    }

    private static T Safe<T>([NotNull] Func<T> func, [NotNull] string label)
    {
      Assert.ArgumentNotNull(func, "func");
      Assert.ArgumentNotNull(label, "label");

      try
      {
        return func();
      }
      catch (Exception ex)
      {
        Log.Error("Failed to process " + label, typeof(App), ex);

        return default(T);
      }
    }

    #endregion

    #endregion
  }
}