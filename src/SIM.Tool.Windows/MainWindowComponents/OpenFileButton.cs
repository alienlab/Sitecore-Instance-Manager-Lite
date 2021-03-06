namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.IO;
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Annotations;

  [UsedImplicitly]
  public class OpenFileButton : IMainWindowButton
  {
    #region Fields

    protected readonly string FilePath;

    #endregion

    #region Constructors

    public OpenFileButton(string param)
    {
      this.FilePath = param;
    }

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        string filePath = this.FilePath.StartsWith("/") ? Path.Combine(instance.WebRootPath, this.FilePath.Substring(1)) : this.FilePath;
        FileSystem.FileSystem.Local.File.AssertExists(filePath, "The {0} file of the {1} instance doesn't exist".FormatWith(filePath, instance.Name));

        string editor = WindowsSettings.AppToolsConfigEditor.Value;
        if (!string.IsNullOrEmpty(editor))
        {
          WindowHelper.RunApp(editor, filePath);
        }
        else
        {
          WindowHelper.OpenFile(filePath);
        }
      }
    }

    #endregion
  }
}