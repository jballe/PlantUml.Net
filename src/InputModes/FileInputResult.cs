using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PlantUml.Net.InputModes
{
    public class FileInputResult : IInputResult
    {
        public FileInputResult(string filePath, string code)
        {
            FilePath = filePath;
            var fileName = Path.GetFileName(filePath);
            var extension = FilePath.Split('.').Last();
            OutputFile = FilePath.Replace($".{extension}", string.Empty);
            Argument = $" -filename \"{fileName}\" {FilePath}";

            File.WriteAllText(filePath, code, Encoding.UTF8);
        }


        public void Dispose()
        {
            try
            {
                DeleteFile(FilePath);
                var fi = new FileInfo(OutputFile);
                var outputFiles = Directory.GetFiles(fi.DirectoryName, fi.Name + ".*");
                foreach (var f in outputFiles)
                {
                    DeleteFile(Path.Combine(fi.DirectoryName, f));
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error while cleaning up input: " + exc);
            }
        }

        private void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Could not delete file {path} due to {exc.Message}");
            }
        }

        public string FilePath { get; }
        public string OutputFile { get; }
        public string Input { get; } = null;
        public string Argument { get; }
    }
}
