using System;
using System.Xml.Serialization;

namespace TcPlayer.Dlna.Models.Browse
{
   
    [Serializable()]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/")]
    public partial class DIDLLiteItem
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

       
        [XmlElement(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string creator
        {
            get;
            set;
        }

       
        [XmlElement(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string date
        {
            get;
            set;
        }

       
        [XmlElement(Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public string artist
        {
            get;
            set;
        }

       
        [XmlElement(Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public string album
        {
            get;
            set;
        }

       
        [XmlElement(Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public string genre
        {
            get;
            set;
        }

       
        [XmlElement(Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public byte originalTrackNumber
        {
            get;
            set;
        }

       
        [XmlIgnore()]
        public bool originalTrackNumberSpecified
        {
            get;
            set;
        }

       
        public DIDLLiteItemRes res
        {
            get;
            set;
        }

       
        [XmlElement(Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public AlbumArtURI albumArtURI
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
    }
}
