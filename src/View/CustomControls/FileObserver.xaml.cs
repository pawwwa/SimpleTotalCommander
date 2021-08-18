using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SimpleTotalCmd.Model.FileSystemOperations;
using SimpleTotalCmd.Infrastructure;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SimpleTotalCmd.ViewModel;

namespace SimpleTotalCmd.View.CustomControls
{
    public partial class FileObserver : UserControl
    {
        public static readonly FileObserverViewModel FileObserverViewModel = new FileObserverViewModel();

        public FileObserver()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void FileObserverControl_LostFocus(object sender, RoutedEventArgs e)
        {
            (this.DataContext as FileObserverViewModel).IsFocused = false;
        }

        private void FileObserverControl_GotFocus(object sender, RoutedEventArgs e)
        {
            (this.DataContext as FileObserverViewModel).IsFocused = true;
        }
    }
}
