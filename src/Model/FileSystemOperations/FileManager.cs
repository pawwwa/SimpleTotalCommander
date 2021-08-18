using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.Collections.Specialized;

namespace SimpleTotalCmd.Model.FileSystemOperations
{
    public static class FileManager
    {
        public static async Task<IEnumerable<FileSystemNode>> GetFiles(string source)
        {
            List<FileSystemNode> nodes = new List<FileSystemNode>();

            foreach (string item in Directory.GetDirectories(source))
            {
                nodes.Add(new FileSystemNode(new DirectoryInfo(item)));
            }

            foreach (string item in Directory.GetFiles(source))
            {
                nodes.Add(new FileSystemNode(new FileInfo(item)));
            }

            return nodes;
        }

        public static string GetParent(string path) => new DirectoryInfo(path).Parent?.FullName;

        public static IEnumerable<DriveInfo> GetDrives() => DriveInfo.GetDrives();

        public static Task MoveItem(FileSystemInfo item, string dest, Action<int> onMove, bool adjust_name = false)
        {
            if (adjust_name) AdjustName(item, ref dest);
            return Task.Run(async ()=> 
            {
                await FileManager.CopyItem(item, dest, onMove, true);
                await FileManager.DeleteItem(item);
            });
        }

        public static Task CopyItem(FileSystemInfo item, string dest, Action<int> onCopy, bool adjust_name = false)
        {
            if (adjust_name) AdjustName(item, ref dest);
            return Task.Run(async () => 
            {
                try
                {
                    if (item is FileInfo)
                    {
                        long bufSize = long.Parse(ConfigurationManager.AppSettings.Get("FileOperationsBufferSize"));

                        FileStream read = new FileStream(item.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, Convert.ToInt32(bufSize / 10), true);
                        FileStream write = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.Write, Convert.ToInt32(bufSize / 10), true);

                        byte[] data = new byte[bufSize];

                        int n = 0;
                        while ((n = read.Read(data, 0, data.Length)) != 0)
                        {
                            onCopy?.Invoke(n);
                            write.Write(data, 0, n);
                        }
                        read.Close();
                        write.Close();
                    }
                    else if (item is DirectoryInfo)
                    {
                        Directory.CreateDirectory(dest);

                        foreach (var directoryItem in Directory.GetDirectories(item.FullName))
                        {
                            DirectoryInfo directory = new DirectoryInfo(directoryItem);
                            await FileManager.CopyItem(directory, dest, onCopy, true);
                        }

                        foreach (var fileItem in Directory.GetFiles(item.FullName))
                        {
                            FileInfo file = new FileInfo(fileItem);
                            await FileManager.CopyItem(file, dest, onCopy, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                
            });
        }

        public static Task DeleteItem(FileSystemInfo item)
        {
            return Task.Run(() => 
            {
                try
                {
                    if (item is FileInfo file)
                    {
                        file.Delete();
                    }
                    else if (item is DirectoryInfo dir)
                    {
                        dir.Delete(true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            });
        }

        public static long GetItemLenth(FileSystemInfo item)
        {
            try
            {
                if (item is FileInfo file)
                {
                    return file.Length;
                }
                else if (item is DirectoryInfo dir)
                {
                    long lenth = 0;

                    foreach (var directoryItem in dir.GetDirectories())
                    {
                        lenth += GetItemLenth(directoryItem);
                    }

                    foreach (var fileItem in dir.GetFiles())
                    {
                        lenth += GetItemLenth(fileItem);
                    }

                    return lenth;
                }

                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 0;
        }

        private static void AdjustName(FileSystemInfo item, ref string dest_name)
        {
            dest_name += $@"\{item.Name}";
        }
    }
}
