using System.Collections.Generic;
using System.Xml.Serialization;

namespace TcPlayer.Dlna.Modles.Discovery
{
    [XmlRoot(ElementName = "serviceList", Namespace = "urn:schemas-upnp-org:device-1-0")]
		public class ServiceList
		{
			[XmlElement(ElementName = "service", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public List<Service> Service { get; set; }
		}

}
