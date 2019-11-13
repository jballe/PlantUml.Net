using System.Diagnostics;

namespace PlantUml.Net.Tools
{
    internal class ProcessHelper
    {
        public IProcessResult RunProcessWithInput(string fileName, string arguments, string input, string workingDirectory = null)
        {
            ProcessStartInfo processStartInfo = GetProcessStartInfo(fileName, arguments, workingDirectory);

            using (Process process = Process.Start(processStartInfo))
            {
                if (input != null)
                {
                    process.WriteInput(input);
                }

                return new ProcessResult
                {
                    Output = process.GetOutput(),
                    Error = process.GetError(),
                    ExitCode = process.ExitCode
                };
            }
        }

        private static ProcessStartInfo GetProcessStartInfo(string command, string arguments, string workingDirectory)
        {
            return new ProcessStartInfo(command)
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = arguments,
                WorkingDirectory = workingDirectory ?? System.IO.Directory.GetCurrentDirectory(),
        };
        }
    }
}
