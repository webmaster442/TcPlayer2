using System;
using System.Xml.Serialization;

namespace TcPlayer.Dlna.Models.Browse
{
    
    [Serializable()]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/")]
    public partial class DIDLLiteItemRes
    {
        
        [XmlAttribute]
        public long size
        {
            get;
            set;
        }

        
        [XmlAttribute]
        public string duration
        {
            get;
            set;
        }

        
        [XmlAttribute]
        public uint bitrate
        {
            get;
            set;
        }

        
        [XmlAttribute]
        public ushort sampleFrequency
        {
            get;
            set;
        }

        
        [XmlAttribute]
        public byte nrAudioChannels
        {
            get;
            set;
        }

        
        [XmlAttribute]
        public string protocolInfo
        {
            get;
            set;
        }

        
        [XmlAttribute]
        public string resolution
        {
            get;
            set;
        }

        
        [XmlText]
        public string Value
        {
            get;
            set;
        }
    }
}
