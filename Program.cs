using System;
using System.IO;
using System.Reflection;

// Usage: File/Folder rename D:\Workspaces\AppBlox eInteraction Enjector.AppBlox true *.*
//        Content rename     D:\Workspaces\AppBlox eInteraction Enjector.AppBlox

namespace VSRename
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Version vrs = Environment.Version;
            Console.WriteLine("*************************************************************************");
            Console.WriteLine("Program:\t\tEnjector Software - ToolBlox - FileSystem - Rename");
            Console.WriteLine("Version:\t\t" + Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine(".NET Framework Version:\t" + vrs);
            Console.WriteLine("\nCopyright 2014 Enjector Software Ltd.");
            Console.WriteLine("*************************************************************************\n");
            Console.WriteLine("Starting ...");

            if (args.Length != 3 && args.Length != 5)
            {
                Console.WriteLine("This renames folders and files with an option to rename contents of the file");
                Console.WriteLine(
                    "Usage VSRename <path> <find> <replace with> [<true - modify file contents only> <extension>]");
                return;
            }

            string path = args[0];
            string find = args[1];
            string replace_with = args[2];
            bool modify_only = false;
            string modify_extension = null;
            if (args.Length == 5)
            {
                modify_only = args[3] == "true";
                modify_extension = args[4];
            }

            if (modify_only == false)
            {
                RenameFiles(path, find, replace_with);
            }
            else
            {
                RenameContent(path, modify_extension, find, replace_with);
            }
        }

        private static void RenameFiles(string path, string find, string replace_with)
        {
            Console.Write(".");
            if (path.Contains(".svn") || path.Contains(".nuget") || path.Contains("packages") || path.Contains("obj") || path.Contains("bin")) return;

            var dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string file_name = file.FullName;
                if (file_name.Contains(find))
                {
                    Console.WriteLine();
                    Console.WriteLine("Renaming {0} -> {1}", file_name, file_name.Replace(find, replace_with));
                    try
                    {
                        File.Move(file_name, file_name.Replace(find, replace_with));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ERROR: " + ex.Message);
                    }
                }
            }

            DirectoryInfo[] child_dirs = dir.GetDirectories();
            foreach (DirectoryInfo child_dir in child_dirs)
            {
                string child_dir_name = child_dir.FullName;
                if (child_dir_name.Contains(find))
                {
                    string child_dir_name_new = child_dir_name.Replace(find, replace_with);
                    Console.WriteLine(@"Renaming {0}\ -> {1}\", child_dir_name, child_dir_name_new);
                    try
                    {
                        Console.WriteLine();
                        Directory.Move(child_dir_name, child_dir_name.Replace(find, replace_with));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ERROR: " + ex.Message);
                    }
                    child_dir_name = child_dir_name_new;
                }
                RenameFiles(child_dir_name + @"\", find, replace_with);
            }
        }

        private static void RenameContent(string path, string modify_extension, string find, string replace_with)
        {
            Console.Write(".");
            if (path.Contains(".svn") || path.Contains(".nuget") || path.Contains("packages") || path.Contains("obj") || path.Contains("bin")) return;

            var dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles("*." + modify_extension);
            foreach (FileInfo file in files)
            {
                string file_name = file.FullName;
                if (file_name.Contains(".bak_") == false)
                {
                    string content = File.ReadAllText(file_name);
                    if (content.Contains(find))
                    {
                        Console.WriteLine();
                        Console.WriteLine(@"Renaming Contents of {0}", file_name);

                        while (content.Contains(find))
                        {
                            content = content.Replace(find, replace_with);
                        }
                        try
                        {
                            File.Move(file_name, file_name + ".bak_" + file.LastWriteTime.Ticks);
                            File.WriteAllText(file_name, content);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine();
                            Console.WriteLine("ERROR: " + ex.Message);
                        }
                    }
                }
            }

            DirectoryInfo[] child_dirs = dir.GetDirectories();
            foreach (DirectoryInfo child_dir in child_dirs)
            {
                string child_dir_name = child_dir.FullName;
                RenameContent(child_dir_name + @"\", modify_extension, find, replace_with);
            }
        }
    }
}