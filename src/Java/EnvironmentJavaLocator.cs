using System;
using System.IO;

namespace PlantUml.Net.Java
{
    internal class EnvironmentJavaLocator : IJavaLocator
    {
        public string GetJavaInstallationPath()
        {
            string javaHome = Environment.GetEnvironmentVariable("JAVA_HOME")?.Trim('"');
            if (string.IsNullOrEmpty(javaHome))
            {
                throw new InvalidOperationException("No JAVA_HOME env variable defined");
            }

            var path = Path.Combine(javaHome, "bin", "java.exe");
            if (!File.Exists(path))
            {
                throw new InvalidOperationException("No java found at " + path);
            }

            return path;
        }
    }
}
