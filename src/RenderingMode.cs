namespace PlantUml.Net
{
    public enum RenderingMode
    {
        /// <summary>
        /// Remote rendering mode uses the PlantUml hosted service to render diagrams.
        /// </summary>
        Remote,

        /// <summary>
        /// Local rendering mode uses a local copy of PlantUml to render diagrams.
        /// </summary>
        Local,

        /// <summary>
        /// Encode with local copy of PlantUml and then generate url for hosted service to render diagrams.
        /// </summary>
        LocalEncode,
    }
}
