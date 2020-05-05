using System;
using System.IO;
using PlantUml.Net.InputModes;
using PlantUml.Net.Java;
using static System.Text.Encoding;

namespace PlantUml.Net.Local
{
    internal class LocalPlantUmlRenderer : IPlantUmlRenderer
    {
        private readonly JarRunner jarRunner;
        private readonly LocalCommandProvider commandProvider;
        private readonly InputFactory inputFactory;

        public LocalPlantUmlRenderer(JarRunner jarRunner, LocalCommandProvider commandProvider, InputFactory inputFactory)
        {
            this.jarRunner = jarRunner;
            this.commandProvider = commandProvider;
            this.inputFactory = inputFactory;
        }

        public byte[] Render(string code, OutputFormat outputFormat)
        {
            using (var input = inputFactory.Create(code))
            {
                var command = commandProvider.GetCommand(outputFormat);
                var processResult = jarRunner.RunJarWithInput(input.Input, command, input.Argument);
                if (processResult.ExitCode != 0)
                {
                    var message = UTF8.GetString(processResult.Error);
                    throw new RenderingException(code, message);
                }

                if (processResult.Output?.Length > 0)
                {
                    return processResult.Output;
                }

                var fileInput = input as FileInputResult;

                if (fileInput != null)
                {
                    var expectedOutputFile = fileInput.OutputFile + $".{outputFormat.ToString().ToLowerInvariant()}";
                    if (File.Exists(expectedOutputFile))
                    {
                        var result = File.ReadAllBytes(expectedOutputFile);
                        File.Delete(expectedOutputFile);
                        return result;
                    }
                }

                throw new ArgumentException($"Processed image without errors but no output was found {command} {input.Argument}");
            }
        }

        public Uri RenderAsUri(string code, OutputFormat outputFormat)
        {
            var bytes = Render(code, outputFormat);
            var base64 = System.Convert.ToBase64String(bytes);
            var url = $"data:image/{GetMimeType(outputFormat)};base64,{base64}";
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            {
                Console.WriteLine($"Cannot crate uri for value '{url}'");
                return new Uri($"http://localhost/{url}");
            }

            return new Uri(url);
        }

        private static string GetMimeType(OutputFormat format)
        {
            switch (format)
            {
                case OutputFormat.Png:
                case OutputFormat.Svg:
                    return $"image/{format.ToString().ToLowerInvariant()}";

                case OutputFormat.Eps:
                    return "application/postscript";
                case OutputFormat.Pdf:
                    return "application/pdf";
                case OutputFormat.Vdx:
                    return "application/vnd.visio";
                case OutputFormat.Xmi:
                    return "application/xml";
                case OutputFormat.Scxml:
                    return "application/scxml+xml";
                case OutputFormat.Html:
                    return "text/html";
                case OutputFormat.Ascii:
                case OutputFormat.Ascii_Unicode:
                case OutputFormat.LaTeX:
                    return "text/plain";
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}
