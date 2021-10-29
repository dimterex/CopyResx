﻿namespace CopyToLocales.ViewModel.Interfaces
{
    using System.Windows.Input;

    public interface INavigationViewModel
    {
        ICommand GoBackCommand { get; }
        ICommand GoForwardCommand { get; }
    }
}
