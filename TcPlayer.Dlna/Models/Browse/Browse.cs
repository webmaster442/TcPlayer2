using System.Xml.Serialization;

namespace TcPlayer.Dlna.Models.Browse
{
    [XmlRoot(ElementName = "Browse", Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1")]
	public class Browse
	{
		[XmlElement(ElementName = "ObjectID")]
		public int ObjectID { get; init; }

		[XmlElement(ElementName = "BrowseFlag")]
		public string BrowseFlag => "BrowseDirectChildren";

		[XmlElement(ElementName = "Filter")]
		public string Filter => "*";

		[XmlElement(ElementName = "StartingIndex")]
		public int StartingIndex => 0;

		[XmlElement(ElementName = "RequestedCount")]
		public int RequestedCount => 0;

		[XmlElement(ElementName = "SortCriteria")]
		public string SortCriteria => string.Empty;
	}
}
