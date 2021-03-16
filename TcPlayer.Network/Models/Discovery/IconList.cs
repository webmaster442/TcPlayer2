// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Serialization;

namespace TcPlayer.Network.Modles.Discovery
{
    [XmlRoot(ElementName = "iconList", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public class IconList
    {
        [XmlElement(ElementName = "icon", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public List<Icon> Icon { get; set; }

        public IconList()
        {
            Icon = new List<Icon>();
        }
    }

}
