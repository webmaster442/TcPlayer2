using System.Xml.Serialization;

namespace TcPlayer.Dlna.Modles.Discovery
{
    [XmlRoot(ElementName = "root", Namespace = "urn:schemas-upnp-org:device-1-0")]
		public class Root
		{
			[XmlElement(ElementName = "specVersion", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public SpecVersion SpecVersion { get; set; }
			[XmlElement(ElementName = "device", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public Device Device { get; set; }
			[XmlAttribute(AttributeName = "xmlns")]
			public string Xmlns { get; set; }
		}

}
