namespace TcPlayer.Engine.Models
{
    public record ItunesXmlDbOptions
    {
        /// <summary>
        /// Exclude tracks that don't exist on the user's system
        /// Default value is false
        /// </summary>
        public bool ExcludeNonExistingFiles { get; init; }

        /// <summary>
        /// Enable or Disable paralel track parsing.
        /// Default value is true
        /// </summary>
        public bool ParalelParsingEnabled { get; init; }

        /// <summary>
        /// Creates a new instance of ITunesXmlDbOptions
        /// </summary>
        public ItunesXmlDbOptions()
        {
            ParalelParsingEnabled = true;
            ExcludeNonExistingFiles = false;
        }
    }
}
