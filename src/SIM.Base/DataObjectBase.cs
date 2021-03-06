﻿using System.ComponentModel;
using Sitecore.Diagnostics;
using Sitecore.Diagnostics.Annotations;

namespace SIM
{
  #region

  

  #endregion

  public class DataObjectBase : INotifyPropertyChanged
  {
    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Methods

    protected void NotifyPropertyChanged([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");

      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(name));
      }
    }

    #endregion
  }
}