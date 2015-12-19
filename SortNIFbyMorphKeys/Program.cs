using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace SortNIFbyMorphKeys
{

    class Program
    {
        static void Main(string[] args)
        {
            //DirectoryUtils StartProcessing = new DirectoryUtils();
            
            if (args != null && args.Any())
            {
                try 
                {
                    Console.WriteLine("The current directory is {0}", Directory.GetCurrentDirectory());
                    string basePath = Directory.GetCurrentDirectory();
                    ProcessDirectory(args[0], basePath); 
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occurred: '{0}'", e);
                }
            }
            else
            {
                Console.WriteLine("Usage: Drag folder containing meshes/structure to this .exe");
            }
            Console.ReadKey();
        }
        
        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory, string basePath) 
        {
            // Process the list of files found in the directory.
            // GetFiles filter by *.nif
            string [] fileEntries = Directory.GetFiles(targetDirectory, "*.nif");
            foreach(string fileName in fileEntries)
                ProcessFile(fileName, basePath);

            // Recurse into subdirectories of this directory.
            string [] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach(string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory, basePath);
        }

        // Insert logic for processing found files here.
        // Path contains full names so separete meshes by morphkeys here
        // Check morphKey and use buildOupputPath
        // Then use CopyTo(path, morphOutputPath)
        public static void ProcessFile(string path, string basePath) 
        {
            try
            {
                if (path.EndsWith("_0.nif"))
                {
                    string targetFile = buildOutputPath(path, 0, basePath);
                    if (!System.IO.File.Exists(targetFile))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                    }
                    if (!meshIs1stPerson(path))
                    {
                        File.Copy(path, targetFile, true);
                        Console.WriteLine("File processed: {0}", path);
                        Console.WriteLine("File copied to: {0}", targetFile);
                    }
                }
                else
                {
                    string targetFile = buildOutputPath(path, 1, basePath);
                    if (!System.IO.File.Exists(targetFile))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                    }
                    if (!meshIs1stPerson(path))
                    {
                        File.Copy(path, targetFile, true);
                        Console.WriteLine("File processed: {0}", path);
                        Console.WriteLine("File copied to: {0}", targetFile);
                    }
                }
            }
            catch (SystemException e)
            {
                Console.Write(e);
            }
        }
        // Return path to copy files 
        public static string buildOutputPath(string path, int morphKey, string basePath)
        {
            //string basePath = Environment.GetCommandLineArgs().ElementAt(0);
            string fix = @"\";
            int meshDirIndex = path.LastIndexOf(basePath);
            int meshDirIndexStart = meshDirIndex + basePath.Length;
            if (meshDirIndex != -1 & meshDirIndexStart < path.Length){
                string meshDirectory = path.Substring(meshDirIndexStart);
                // Create path: basePath + morphkey + meshDirectory
                // Format basePath \morphkey meshDirectory
                string outputPath = basePath + fix + morphKey.ToString() + meshDirectory;
                Console.WriteLine(basePath);
                Console.WriteLine(outputPath);
                return outputPath;
            }
            else
            {
                Console.WriteLine("Error while creating output path");
                Console.WriteLine(basePath);
                Console.WriteLine(meshDirIndex);
                Console.WriteLine("meshDirIndexStart:");
                Console.WriteLine(meshDirIndexStart);
                return null;
            }
        }
        // Tryto detect stpersonmeshes by filename and return true/false
        public static bool meshIs1stPerson(string path)
        {
            string firstPersonDetector = "1stperson";
            if (Path.GetExtension(path).Equals(".nif") & Path.GetFileName(path).Contains(firstPersonDetector))
            {
                Console.WriteLine("Found 1stpersonmesh: {0} ,skipping/deleting", Path.GetFileName(path));
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
