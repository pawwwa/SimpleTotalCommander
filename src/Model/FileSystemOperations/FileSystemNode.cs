using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace SimpleTotalCmd.Model.FileSystemOperations
{
    public class FileSystemNode
    {
        public FileSystemNode(FileSystemInfo node)
        {
            Node = node;
        }

        public FileSystemInfo Node{ get; set; }

        public Uri IconUri
        {
            get
            {
                if (Node is FileInfo)
                {
                    return new Uri("pack://application:,,,/Component/Resources/Icons/file_drawn.png");
                }
                else
                {
                    return new Uri("pack://application:,,,/Component/Resources/Icons/folder_drawn.png");
                }
            }
        }

        public string Size
        {
            get 
            {
                if (Node is FileInfo)
                {
                    return (Node as FileInfo).Length.ToString();
                }
                else
                {
                    return "<folder>";
                }
            }
        }
    }
}
