using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerServiceDemo
{

    public class MicroServices
    {
        public static async Task<string> CreateFolder(string folName)
        {
            // Result Message
            string message = "";
            
            // Specify a name for your folder.
            string folderName = folName;
            
            // To create a string that specifies the path to your folder
            string pathString = System.IO.Path.Combine(folderName);
            if (!(Directory.Exists(pathString)))
            {
                Directory.CreateDirectory(pathString);
                message = $"Directory with name {folderName} created successfully";
            }
            else
            {
                message = $"Directory with name {folderName} already exist";
            }
            await Task.CompletedTask;
            return message;
        }
        public static async Task<string> MoveFile(string origin,string destination,string file)
        {
            // Result Message
            string message = "";

            string originPath = $"{origin}\\{file}";
            string destinationPath = $"{destination}\\{file}";

            try
            {
                File.Move(originPath, destinationPath);
                message = $"{originPath} successfully moved to {destinationPath}";

            }
            catch(Exception ex)
            {
                message = $"{originPath} failed to move to {destinationPath}: " + ex.Message;
            }
            await Task.CompletedTask;
            return message;
        }

        public static async Task WriteToFile(string fileName, string content)
        {
            await File.WriteAllTextAsync(fileName, content);
        }
    }
   
}
