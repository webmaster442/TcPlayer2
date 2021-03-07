using System;
using System.Xml.Serialization;

namespace TcPlayer.Network.Models.Browse
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Result
    {
        public Result()
        {
            DIDLLite = new DIDLLite();
        }

        [XmlElement("DIDL-Lite", Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/")]
        public DIDLLite DIDLLite
        {
            get;
            set;
        }
    }
}
