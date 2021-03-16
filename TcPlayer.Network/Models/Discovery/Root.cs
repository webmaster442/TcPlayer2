// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace TcPlayer.Network.Modles.Discovery
{
    [XmlRoot(ElementName = "root", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public class Root
    {
        public Root()
        {
            SpecVersion = new SpecVersion();
            Device = new Device();
        }

        [XmlElement(ElementName = "specVersion", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public SpecVersion SpecVersion { get; set; }
        [XmlElement(ElementName = "device", Namespace = "urn:schemas-upnp-org:device-1-0")]
        public Device Device { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string? Xmlns { get; set; }
    }

}
