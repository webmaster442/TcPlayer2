using System.Xml.Serialization;

namespace TcPlayer.Network.Modles.Discovery
{
    [XmlRoot(ElementName = "service", Namespace = "urn:schemas-upnp-org:device-1-0")]
		public class Service
		{
			[XmlElement(ElementName = "serviceType", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string? ServiceType { get; set; }
			[XmlElement(ElementName = "serviceId", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string? ServiceId { get; set; }
			[XmlElement(ElementName = "controlURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string? ControlURL { get; set; }
			[XmlElement(ElementName = "eventSubURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string? EventSubURL { get; set; }
			[XmlElement(ElementName = "SCPDURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string? SCPDURL { get; set; }
		}

}
