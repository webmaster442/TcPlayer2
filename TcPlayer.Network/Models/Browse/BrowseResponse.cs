// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Xml.Serialization;

namespace TcPlayer.Network.Models.Browse
{    
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1")]
    [XmlRoot(Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1", IsNullable = false)]
    public class BrowseResponse
    {
        public BrowseResponse()
        {
            Result = new Result();
        }

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
