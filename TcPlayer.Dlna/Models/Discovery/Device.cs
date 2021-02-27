using System.Xml.Serialization;

namespace TcPlayer.Dlna.Modles.Discovery
{
    [XmlRoot(ElementName = "device", Namespace = "urn:schemas-upnp-org:device-1-0")]
		public class Device
		{
			[XmlElement(ElementName = "deviceType", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string DeviceType { get; set; }
			[XmlElement(ElementName = "friendlyName", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string FriendlyName { get; set; }
			[XmlElement(ElementName = "manufacturer", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string Manufacturer { get; set; }
			[XmlElement(ElementName = "manufacturerURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string ManufacturerURL { get; set; }
			[XmlElement(ElementName = "modelDescription", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string ModelDescription { get; set; }
			[XmlElement(ElementName = "modelName", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string ModelName { get; set; }
			[XmlElement(ElementName = "modelNumber", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string ModelNumber { get; set; }
			[XmlElement(ElementName = "modelURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string ModelURL { get; set; }
			[XmlElement(ElementName = "serialNumber", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string SerialNumber { get; set; }
			[XmlElement(ElementName = "UDN", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string UDN { get; set; }
			[XmlElement(ElementName = "X_DLNADOC", Namespace = "urn:schemas-dlna-org:device-1-0")]
			public X_DLNADOC X_DLNADOC { get; set; }
			[XmlElement(ElementName = "presentationURL", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string PresentationURL { get; set; }
			[XmlElement(ElementName = "iconList", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public IconList IconList { get; set; }
			[XmlElement(ElementName = "serviceList", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public ServiceList ServiceList { get; set; }
		}

}
