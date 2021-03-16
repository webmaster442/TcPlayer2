// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Xml.Serialization;

namespace TcPlayer.Network.Models.Browse
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/")]
    public class DIDLLiteContainer
    {
        public DIDLLiteContainer()
        {
            Title = string.Empty;
            Class = string.Empty;
            Id = string.Empty;
            ParentID = string.Empty;
        }

        [XmlElement(ElementName = "title", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Title
        {
            get;
            set;
        }


        [XmlElement(ElementName = "class", Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public string Class
        {
            get;
            set;
        }

        [XmlElement(ElementName = "storageUsed", Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public long StorageUsed
        {
            get;
            set;
        }


        [XmlAttribute(AttributeName = "id")]
        public string Id
        {
            get;
            set;
        }


        [XmlAttribute(AttributeName = "parentID")]
        public string ParentID
        {
            get;
            set;
        }


        [XmlAttribute(AttributeName = "restricted")]
        public byte Restricted
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "searchable")]
        public byte Searchable
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "childCount")]
        public int ChildCount
        {
            get;
            set;
        }
    }
}
