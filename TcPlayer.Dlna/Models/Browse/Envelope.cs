using System.Xml.Serialization;

namespace TcPlayer.Dlna.Models.Browse
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
	public class Envelope
	{
		[XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
		public Body Body { get; set; }

		[XmlAttribute(AttributeName = "encodingStyle", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
		public string EncodingStyle => "http://schemas.xmlsoap.org/soap/encoding/";
	}
}
