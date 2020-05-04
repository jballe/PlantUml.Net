using System;

namespace PlantUml.Net
{
    public class PlantUmlSettings
    {
        /// <summary>
        /// Path to java.exe.
        /// By default this will be obtained from the JAVA_HOME environment variable.
        /// </summary>
        public string JavaPath { get; set; }

        /// <summary>
        /// Url pointing to remote PlantUml server.
        /// Defaults to http://www.plantuml.com/plantuml/
        /// </summary>
        public string RemoteUrl { get; set; }

        /// <summary>
        /// Path to plantuml.jar.
        /// Defaults to working directory.
        /// </summary>
        public string LocalPlantUmlPath { get; set; }

        /// <summary>
        /// Path to dot.exe.
        /// Required for Local rendering mode.
        /// </summary>
        public string LocalGraphvizDotPath { get; set; }

        /// <summary>
        /// Local or Remote rendering mode.
        /// Defaults to Remote.
        /// </summary>
        public RenderingMode RenderingMode { get; set; }

        public InputMode InputMode { get; set; }

        public PlantUmlSettings()
        {
            RenderingMode = RenderingMode.Remote;
            RemoteUrl = "https://www.plantuml.com/plantuml/";
            LocalPlantUmlPath = Environment.GetEnvironmentVariable("PLANTUML_JAR") ?? "plantuml.jar";
            LocalGraphvizDotPath = Environment.GetEnvironmentVariable("GRAPHVIZ_DOT");
        }
    }
}
