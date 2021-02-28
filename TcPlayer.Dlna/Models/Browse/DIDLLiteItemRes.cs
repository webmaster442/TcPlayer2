using System;
using System.Xml.Serialization;

namespace TcPlayer.Dlna.Models.Browse
{
    
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/")]
    public class DIDLLiteItemRes
    {
        
        [XmlAttribute(AttributeName = "size")]
        public long Size
        {
            get;
            set;
        }

        
        [XmlAttribute(AttributeName = "duration")]
        public string Duration
        {
            get;
            set;
        }

        
        [XmlAttribute(AttributeName = "bitrate")]
        public uint Bitrate
        {
            get;
            set;
        }

        
        [XmlAttribute(AttributeName = "sampleFrequency")]
        public int SampleFrequency
        {
            get;
            set;
        }

        
        [XmlAttribute(AttributeName = "nrAudioChannels")]
        public byte NrAudioChannels
        {
            get;
            set;
        }

        
        [XmlAttribute(AttributeName = "protocolInfo")]
        public string ProtocolInfo
        {
            get;
            set;
        }

        
        [XmlAttribute(AttributeName = "resolution")]
        public string Resolution
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
