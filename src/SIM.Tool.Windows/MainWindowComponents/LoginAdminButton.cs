namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;

  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  [UsedImplicitly]
  public class LoginAdminButton : IMainWindowButton
  {
    #region Fields

    protected readonly string Browser;
    protected readonly string VirtualPath;
    protected readonly string[] Params;

    #endregion

    #region Constructors

    public LoginAdminButton()
    {
      this.VirtualPath = string.Empty;
      this.Browser = string.Empty;
      this.Params = new string[0];
    }

    public LoginAdminButton([NotNull] string param)
    {
      Assert.ArgumentNotNull(param, "param");

      var par = Parameters.Parse(param);
      this.VirtualPath = par[0];
      this.Browser = par[1];
      this.Params = par.Skip(2);
    }

    #endregion

    #region Public methods

    public bool IsEnabled([CanBeNull] Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");
      
      Assert.IsNotNull(instance, "instance"); 

      InstanceHelperEx.OpenInBrowserAsAdmin(instance, mainWindow, this.VirtualPath, this.Browser, this.Params);
    }

    #endregion
  }
}