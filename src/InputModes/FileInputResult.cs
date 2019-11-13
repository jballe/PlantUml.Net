using System.IO;
using System.Text;

namespace PlantUml.Net.InputModes
{
    public class FileInputResult : IInputResult
    {
        public FileInputResult(string filePath, string code)
        {
            Argument = filePath;
            File.WriteAllText(filePath, code, Encoding.UTF8);
        }
        public void Dispose()
        {
            File.Delete(Argument);
        }

        public string Input { get; } = null;
        public string Argument { get; }
    }
}
