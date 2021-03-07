using System;
using System.Xml.Serialization;

namespace TcPlayer.Network.Models.Browse
{
    [Serializable]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class EnvelopeBody
    {
        [XmlElement(Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1")]
        public BrowseResponse BrowseResponse
        {
            get;
            set;
        }

        public EnvelopeBody()
        {
            BrowseResponse = new BrowseResponse();
        }
    }


}
