// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Xml.Serialization;

namespace TcPlayer.Network.Models.Browse
{
   
    [Serializable()]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/")]
    public class DIDLLiteItem
    {
        public DIDLLiteItem()
        {
            Title = string.Empty;
            Class = string.Empty;
            Id = string.Empty;
            ParentID = string.Empty;
            AlbumArtURI = new AlbumArtURI();
            Res = new DIDLLiteItemRes();
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

       
        [XmlElement(ElementName = "creator", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string? Creator
        {
            get;
            set;
        }

       
        [XmlElement(ElementName = "date", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string? Date
        {
            get;
            set;
        }

       
        [XmlElement(ElementName = "artist", Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public string? Artist
        {
            get;
            set;
        }

       
        [XmlElement(ElementName = "album", Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public string? Album
        {
            get;
            set;
        }

       
        [XmlElement(ElementName = "genre", Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public string? Genre
        {
            get;
            set;
        }

       
        [XmlElement(ElementName = "originalTrackNumber", Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public byte OriginalTrackNumber
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


        [XmlElement(ElementName = "res")]
        public DIDLLiteItemRes Res
        {
            get;
            set;
        }

       
        [XmlElement(ElementName = "albumArtURI", Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
        public AlbumArtURI AlbumArtURI
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
    }
}
