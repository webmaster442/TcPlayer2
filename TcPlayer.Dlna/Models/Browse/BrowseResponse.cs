using System;
using System.Xml.Serialization;

namespace TcPlayer.Dlna.Models.Browse
{    
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1")]
    [XmlRoot(Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1", IsNullable = false)]
    public partial class BrowseResponse
    {
        [XmlElement(Namespace = "")]
        public Result Result
        {
            get;
            set;
        }

        
        [XmlElement(Namespace = "")]
        public byte NumberReturned
        {
            get;
            set;
        }

        
        [XmlElement(Namespace = "")]
        public byte TotalMatches
        {
            get;
            set;
        }

        
        [XmlElement(Namespace = "")]
        public byte UpdateID
        {
            get;
            set;
        }
    }
}
