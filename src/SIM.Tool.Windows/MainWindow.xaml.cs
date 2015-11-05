namespace SIM.Tool.Windows
{
  using System;
  using System.ComponentModel;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Threading;
  using System.Xml;

  using SIM.Tool.Base;

  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  public partial class MainWindow
  {
    #region Fields

    [NotNull]
    public static MainWindow Instance;

    private readonly Timer timer;
    private IMainWindowButton doubleClickHandler;

    #endregion

    #region Constructors

    public MainWindow()
    {
      this.InitializeComponent();

      using (new ProfileSection("Main window ctor", typeof(MainWindow)))
      {
        Instance = this;
        this.Title = string.Format(this.Title, ApplicationManager.AppShortVersion, ApplicationManager.AppLabel);

        this.timer =
          new System.Threading.Timer(
            obj => this.Dispatcher.Invoke(new Action(() => this.Search(null, null)), DispatcherPriority.Render));
      }
    }

    #endregion

    #region Methods

    #region Protected properties

    protected IMainWindowButton DoubleClickHandler
    {
      get
      {
        return this.doubleClickHandler ?? (this.doubleClickHandler = (IMainWindowButton)WindowsSettings.AppUiMainWindowDoubleClick.Value.With(x => CreateInstance(x)));
      }
    }
    public static object CreateInstance(XmlElement element)
    {
      var typeFullName = element.GetAttribute("type");
      if (string.IsNullOrEmpty(typeFullName))
      {
        return null;
      }

      var param = element.GetAttribute("param");
      return CreateInstance(typeFullName, element.Name, param.EmptyToNull());
    }

    public static object CreateInstance(string typeFullName, string reference = null, string param = null)
    {
      var type = GetType(typeFullName, reference);

      if (!string.IsNullOrEmpty(param))
      {
        return ReflectionUtil.CreateObject(type, param);
      }

      return ReflectionUtil.CreateObject(type);
    }

    public static Type GetType(string typeFullName, string reference = null)
    {
      // locate type within already loaded assemblies
      Type type = Type.GetType(typeFullName);

      // type was not found so we need to load necessary assemlby
      if (type == null)
      {
        var arr = typeFullName.Split(',');
        Assert.IsTrue(arr.Length <= 2,
          string.IsNullOrEmpty(reference) ? "Wrong type identifier (no comma), must be like Namespace.Type, Assembly" : "The type attribute value of the <{0}> element has wrong format".FormatWith(reference));

        // format: type, assembly
        var typeName = arr[0].Trim();
        var assemblyName = arr[1].Trim();

        // find the assembly file by specified assembly name
        var assembly =
          AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(ass => ass.FullName.StartsWith(assemblyName + ","));
        Assert.IsNotNull(assembly, "Cannot find the {0} assembly".FormatWith(assemblyName));

        type = assembly.GetType(typeName);
        Assert.IsNotNull(type, "Cannot locate the {0} type within the {1} assembly".FormatWith(typeFullName, assemblyName));
      }

      return type;
    }

    #endregion

    #region Private methods

    private static string GetCookie()
    {
      var path = Path.Combine(ApplicationManager.TempFolder, "cookie.txt");
      if (!FileSystem.FileSystem.Local.File.Exists(path))
      {
        var cookie = Guid.NewGuid().ToString().Replace("{", string.Empty).Replace("}", string.Empty).Replace("-", string.Empty);
        FileSystem.FileSystem.Local.File.WriteAllText(path, cookie);

        return cookie;
      }

      return FileSystem.FileSystem.Local.File.ReadAllText(path);
    }
    
    private void AppPoolRecycleClick(object sender, RoutedEventArgs e)
    {
      try
      {
        {
          MainWindowHelper.AppPoolRecycle();
        }
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void AppPoolStartClick(object sender, RoutedEventArgs e)
    {
      try
      {
        {
          MainWindowHelper.AppPoolStart();
        }
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void AppPoolStopClick(object sender, RoutedEventArgs e)
    {
      try
      {
        MainWindowHelper.AppPoolStop();
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void HandleError(Exception exception)
    {
      WindowHelper.HandleError(exception.Message, true, exception);
    }

    private void InstanceSelected([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      try
      {
        MainWindowHelper.OnInstanceSelected();
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void ItemsTreeViewKeyPressed([CanBeNull] object sender, [NotNull] KeyEventArgs e)
    {
      try
      {
        Assert.ArgumentNotNull(e, "e");

        if (e.Handled)
        {
          return;
        }

        e.Handled = true;
        Key key = e.Key;
        switch (key)
        {
          case Key.Escape:
            {
              this.SearchTextBox.Text = string.Empty;
              MainWindowHelper.Search();

              return;
            }

          case Key.F3:
            {
              this.InstanceList.ContextMenu.IsOpen = true;
              return;
            }

          case Key.F5:
            {
              this.RefreshInstances();
              return;
            }

          case Key.C:
            {
              if ((Keyboard.IsKeyToggled(Key.LeftCtrl) | Keyboard.IsKeyToggled(Key.RightCtrl)) && MainWindowHelper.SelectedInstance != null)
              {
                System.Windows.Clipboard.SetText(MainWindowHelper.SelectedInstance.Name);
                return;
              }

              break;
            }
        }

        e.Handled = false;
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void ItemsTreeViewMouseDoubleClick([CanBeNull] object sender, [CanBeNull] MouseButtonEventArgs e)
    {
      using (new ProfileSection("Main window tree item double click", this))
      {
        try
        {
          {
            if (this.DoubleClickHandler.IsEnabled(this, MainWindowHelper.SelectedInstance))
            {
              this.DoubleClickHandler.OnClick(this, MainWindowHelper.SelectedInstance);
            }
          }
        }
        catch (Exception ex)
        {
          this.HandleError(ex);
        }
      }
    }

    private void ItemsTreeViewMouseRightClick([CanBeNull] object sender, [NotNull] MouseButtonEventArgs e)
    {
      using (new ProfileSection("Main window tree view right click", this))
      {
        try
        {
          Assert.ArgumentNotNull(e, "e");

          WindowHelper.FocusClickedNode(e);
        }
        catch (Exception ex)
        {
          this.HandleError(ex);
        }
      }
    }

    private void OpenProgramLogs(object sender, RoutedEventArgs e)
    {
      try
      {
        MainWindowHelper.OpenProgramLogs();
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void RefreshInstances()
    {
      try
      {
        MainWindowHelper.RefreshInstances();
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void Search([CanBeNull] object sender, [CanBeNull] EventArgs e)
    {
      try
      {
        {
          MainWindowHelper.Search();
        }
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void SearchTextBoxKeyPressed([CanBeNull] object sender, [NotNull] KeyEventArgs e)
    {
      try
      {
        Assert.ArgumentNotNull(e, "e");

        if (e.Handled)
        {
          return;
        }

        e.Handled = true;
        Key key = e.Key;
        switch (key)
        {
          case Key.Escape:
            {
              if (string.IsNullOrEmpty(this.SearchTextBox.Text))
              {
                // this.WindowState = WindowState.Minimized;
              }

              this.SearchTextBox.Text = string.Empty;
              {
                MainWindowHelper.Search();
              }

              return;
            }

          case Key.Enter:
            {
              {
                MainWindowHelper.Search();
              }

              return;
            }

          case Key.F5:
            {
              this.RefreshInstances();
              return;
            }

          default:
            {
              if (WindowsSettings.AppInstanceSearchEnabled.Value)
              {
                this.timer.Change(TimeSpan.FromMilliseconds(WindowsSettings.AppInstanceSearchTimeout.Value), TimeSpan.FromMilliseconds(-1));
              }

              e.Handled = false;
              return;
            }
        }
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    private void WindowLoaded(object sender, EventArgs eventArgs)
    {
      try
      {
        {
          MainWindowHelper.Initialize();
        }
      }
      catch (Exception ex)
      {
        this.HandleError(ex);
      }
    }

    #endregion

    #endregion

    #region Public methods

    public void Initialize()
    {
      using (new ProfileSection("Initializing main window", this))
      {
        var appDocument = XmlDocumentEx.LoadFile("App.xml");
        MainWindowHelper.InitializeRibbon(appDocument);
        MainWindowHelper.InitializeContextMenu(appDocument);
      }
    }

    #endregion

    #region Private methods

    private void WindowClosing(object sender, CancelEventArgs e)
    {
      using (new ProfileSection("Closing main window", this))
      {
        try
        {
          ApplicationManager.RaiseAttemptToClose(e);
        }
        catch (Exception ex)
        {
          Log.Error("Err", this, ex);
        }
      }
    }

    #endregion
  }
}