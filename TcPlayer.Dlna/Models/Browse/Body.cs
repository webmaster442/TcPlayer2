using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TcPlayer.Dlna.Models.Browse
{
	[XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
	public class Body
	{
		[XmlElement(ElementName = "Browse", Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1")]
		public Browse Browse { get; set; }
	}
}
