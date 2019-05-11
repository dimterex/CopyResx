using System.Windows.Input;

namespace CopyToLocales.ViewModel
{
    public interface INavigation
    {
        ICommand GoBackCommand { get; }
        ICommand GoForwardkCommand { get; }
    }
}
