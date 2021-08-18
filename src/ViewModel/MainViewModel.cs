using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SimpleTotalCmd.Infrastructure;
using SimpleTotalCmd.Model.FileSystemOperations;

namespace SimpleTotalCmd.ViewModel
{
    class MainViewModel : BasePropertyNotify
    {
        public FileObserverViewModel Left { get; set; } = new FileObserverViewModel();
        public FileObserverViewModel Right { get; set; } = new FileObserverViewModel();

        private long pb_min = 0;
        public long PB_MinValue { get => pb_min; set { pb_min = value; Notify(); } }

        private long pb_max = 1;
        public long PB_MaxValue { get => pb_max; set { pb_max = value; Notify(); } }

        private long pb_current = 0;
        public long PB_CurrentValue { get => pb_current; set { pb_current = value; Notify(); } }

        public MainViewModel()
        {
            InitProps();
            InitCommands();
        }

        private async void InitProps()
        {
            string initPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Left.FSNodes = new ObservableCollection<FileSystemNode>(await FileManager.GetFiles(initPath));
            Left.CurrentPath = initPath;
            Right.FSNodes = new ObservableCollection<FileSystemNode>(await FileManager.GetFiles(initPath));
            Right.CurrentPath = initPath;
        }

        private void InitCommands()
        {
            Copy = new Command(async x =>
            {
                SelectedSideValidation();
                FileObserverViewModel instance = Left.IsFocused ? Left : Right;
                FileObserverViewModel opposite = instance == Left ? Right : Left;

                if (instance.SelectedNode != null)
                {
                    PB_MaxValue = FileManager.GetItemLenth(instance.SelectedNode.Node);

                    await FileManager.CopyItem(instance.SelectedNode.Node, opposite.CurrentPath, new Action<int>(x =>
                    {
                        PB_CurrentValue += x;
                    }), true);

                    UpdateNoeds();

                    PB_CurrentValue = 0;
                    PB_MaxValue = 1;
                }
            });

            Delete = new Command( async x => 
            {
                SelectedSideValidation();
                FileObserverViewModel instance = Left.IsFocused ? Left : Right;
                if (instance.SelectedNode != null)
                {
                    await FileManager.DeleteItem(instance.SelectedNode.Node);

                    UpdateNoeds();
                }
            });

            Move = new Command(async x =>
            {
                SelectedSideValidation();
                FileObserverViewModel instance = Left.IsFocused ? Left : Right;
                FileObserverViewModel opposite = instance == Left ? Right : Left;
                if (instance.SelectedNode != null)
                {
                    PB_MaxValue = FileManager.GetItemLenth(instance.SelectedNode.Node);

                    await FileManager.MoveItem(instance.SelectedNode.Node, opposite.CurrentPath, new Action<int>(x =>
                    {
                        PB_CurrentValue += x;
                    }), false);

                    UpdateNoeds();

                    PB_CurrentValue = 0;
                    PB_MaxValue = 1;
                }
            });
        }

        private void SelectedSideValidation()
        {
            if (Left.IsFocused == Right.IsFocused)
            {
                if (Left.SelectedNode != null)
                {
                    Right.IsFocused = false;
                }
                else if(Right.SelectedNode != null)
                {
                    Left.IsFocused = false;
                }
            }
        }

        private async void UpdateNoeds()
        {
            Left.FSNodes = new ObservableCollection<FileSystemNode>(await FileManager.GetFiles(Left.CurrentPath));
            Right.FSNodes = new ObservableCollection<FileSystemNode>(await FileManager.GetFiles(Right.CurrentPath));
        }

        public ICommand Copy { get; private set; }
        public ICommand Delete { get; private set; }
        public ICommand Move { get; private set; }
    }
}
