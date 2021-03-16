// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TcPlayer.Network.Modles.Discovery
{

    [XmlRoot(ElementName = "X_DLNADOC", Namespace = "urn:schemas-dlna-org:device-1-0")]
    public class X_DLNADOC
    {
        [XmlAttribute(AttributeName = "dlna", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string? Dlna { get; set; }
        [XmlText]
        public string? Text { get; set; }
    }
}
