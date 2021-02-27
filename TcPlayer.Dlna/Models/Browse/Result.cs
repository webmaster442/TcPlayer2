using System;
using System.Xml.Serialization;

namespace TcPlayer.Dlna.Models.Browse
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class Result
    {
        [XmlArray("DIDL-Lite", Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/")]
        [XmlArrayItem("container", IsNullable = false)]
        public DIDLLiteContainer[] DIDLLite
        {
            get;
            set;
        }
    }


}
