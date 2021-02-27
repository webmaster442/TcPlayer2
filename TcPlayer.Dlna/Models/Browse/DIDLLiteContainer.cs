using System;
using System.Xml.Serialization;

namespace TcPlayer.Dlna.Models.Browse
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/")]
    public partial class DIDLLiteContainer
    {
        [XmlElement(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string title
        {
            get;
            set;
        }


        [XmlElement(Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public string @class
        {
            get;
            set;
        }

        [XmlElement(Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public long storageUsed
        {
            get;
            set;
        }


        [XmlAttribute()]
        public string id
        {
            get;
            set;
        }


        [XmlAttribute()]
        public string parentID
        {
            get;
            set;
        }


        [XmlAttribute()]
        public byte restricted
        {
            get;
            set;
        }

        [XmlAttribute()]
        public byte searchable
        {
            get;
            set;
        }

        [XmlAttribute()]
        public int childCount
        {
            get;
            set;
        }
    }
}
