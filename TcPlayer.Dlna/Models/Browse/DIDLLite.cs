﻿using System;
using System.Xml.Serialization;

namespace TcPlayer.Dlna.Models.Browse
{
    [Serializable()]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/")]
    [XmlRoot("DIDL-Lite", Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/", IsNullable = false)]
    public class DIDLLite
    {
        public DIDLLite()
        {
            Items = Array.Empty<object>();
        }

        [XmlElement("container", typeof(DIDLLiteContainer))]
        [XmlElement("item", typeof(DIDLLiteItem))]
        public object[] Items
        {
            get;
            set;
        }
    }


}
