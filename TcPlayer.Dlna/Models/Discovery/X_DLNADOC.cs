using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TcPlayer.Dlna.Modles.Discovery
{

    [XmlRoot(ElementName = "X_DLNADOC", Namespace = "urn:schemas-dlna-org:device-1-0")]
    public class X_DLNADOC
    {
        [XmlAttribute(AttributeName = "dlna", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string? Dlna { get; set; }
        [XmlText]
        public string? Text { get; set; }
    }
}
