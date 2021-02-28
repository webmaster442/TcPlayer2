using System.Xml.Serialization;

namespace TcPlayer.Dlna.Modles.Discovery
{
    [XmlRoot(ElementName = "specVersion", Namespace = "urn:schemas-upnp-org:device-1-0")]
		public class SpecVersion
		{
			[XmlElement(ElementName = "major", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string? Major { get; set; }
			[XmlElement(ElementName = "minor", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string? Minor { get; set; }
		}

}
