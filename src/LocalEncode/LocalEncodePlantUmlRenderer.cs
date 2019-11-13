using System;
using System.Net;
using System.Net.Http;
using PlantUml.Net.InputModes;
using PlantUml.Net.Java;
using PlantUml.Net.Remote;

namespace PlantUml.Net.LocalEncode
{
    internal class LocalEncodePlantUmlRenderer : IPlantUmlRenderer
    {
        private readonly JarRunner _jarRunner;
        private readonly UrlFormatMap _urlFormatMap;
        private readonly InputFactory _inputFactory;

        internal LocalEncodePlantUmlRenderer(JarRunner jarRunner, UrlFormatMap urlFormatMap, InputFactory inputFactory)
        {
            _jarRunner = jarRunner;
            _urlFormatMap = urlFormatMap;
            _inputFactory = inputFactory;
        }

        public byte[] Render(string code, OutputFormat outputFormat)
        {
            var uri = RenderAsUri(code, outputFormat);
            
            // Ok this is copied from RemoteRenderer, consider alternatives
            using (HttpClient httpClient = new HttpClient())
            {
                var task = httpClient.GetAsync(uri.ToString());
                var result = task.ConfigureAwait(false).GetAwaiter().GetResult();

                if (result.IsSuccessStatusCode)
                {
                    return result.Content.ReadAsByteArrayAsync().Result;
                }

                if (result.StatusCode == HttpStatusCode.BadRequest)
                {
                    var messages = result.Headers.GetValues("X-PlantUML-Diagram-Error");
                    throw new RenderingException(code, string.Join(Environment.NewLine, messages));
                }

                throw new HttpRequestException(result.ReasonPhrase);
            }
        }

        public Uri RenderAsUri(string code, OutputFormat outputFormat)
        {
            var component = Encode(code);
            var url = _urlFormatMap.GetRenderUrl(component, outputFormat);
            return new Uri(url);
        }

        private string Encode(string code)
        {
            using (var input = _inputFactory.Create(code))
            {

                var processResult = _jarRunner.RunJarWithInput(input.Input, "-computeurl", input.Argument);
                if (processResult.ExitCode != 0)
                {
                    var message = System.Text.Encoding.UTF8.GetString(processResult.Error);
                    Console.WriteLine($"WARN {message}");
                    throw new RenderingException(code, message);
                }

                var result = System.Text.Encoding.UTF8.GetString(processResult.Output);
                return result;
            }
        }
    }
}
