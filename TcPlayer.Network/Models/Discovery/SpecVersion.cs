﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace TcPlayer.Network.Modles.Discovery
{
    [XmlRoot(ElementName = "specVersion", Namespace = "urn:schemas-upnp-org:device-1-0")]
		public class SpecVersion
		{
			[XmlElement(ElementName = "major", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string? Major { get; set; }
			[XmlElement(ElementName = "minor", Namespace = "urn:schemas-upnp-org:device-1-0")]
			public string? Minor { get; set; }
		}

}
