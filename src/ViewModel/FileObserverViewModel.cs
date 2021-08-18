using SimpleTotalCmd.Infrastructure;
using SimpleTotalCmd.Model.FileSystemOperations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleTotalCmd.ViewModel
{
    public class FileObserverViewModel : BasePropertyNotify
    {
        private ObservableCollection<FileSystemNode> _fsNodes = new ObservableCollection<FileSystemNode>();
        public ObservableCollection<FileSystemNode> FSNodes { get => _fsNodes; set { _fsNodes = value; Notify(); } }

        private FileSystemNode _selectedNode;
        public FileSystemNode SelectedNode { get => _selectedNode; set { _selectedNode = value; Notify(); } }

        private string _currentPath;
        public string CurrentPath { get => _currentPath; set { _currentPath = value; Notify(); } }

        public IEnumerable<DriveInfo> Drives { get; set; } = FileManager.GetDrives();
        public DriveInfo SelectedDrive { get; set; }

        public bool IsFocused{ get; set; }

        public FileObserverViewModel()
        {
            SelectedDrive = Drives.First();
            InitCommands();
        }

        private void InitCommands()
        {
            Back = new Command(async x =>
            {
                string path;
                if ((path = FileManager.GetParent(CurrentPath)) != null)
                {
                    CurrentPath = path;
                    FSNodes = new ObservableCollection<FileSystemNode>(await FileManager.GetFiles(path));
                }
            });

            Move = new Command(async x =>
            {
                try
                {
                    if (SelectedNode.Node is DirectoryInfo)
                    {
                        CurrentPath = SelectedNode.Node.FullName;
                        FSNodes = new ObservableCollection<FileSystemNode>(await FileManager.GetFiles(SelectedNode.Node.FullName));
                    }
                }
                catch (Exception)
                {
                    CurrentPath = new DirectoryInfo(CurrentPath).Parent.FullName;
                }
            });

            ChangeDisk = new Command(async x => 
            {
                FSNodes = new ObservableCollection<FileSystemNode>(await FileManager.GetFiles(SelectedDrive.Name));
                CurrentPath = SelectedDrive.Name;
            });
        }

        public ICommand Back { get; private set; }
        public ICommand Move { get; private set; }
        public ICommand ChangeDisk { get; private set; }
    }
}
