// ------------------------------------------------------------------------------------------------
// Copyright (c) 2021 Ruzsinszki Gábor
// This is free software under the terms of the MIT License. https://opensource.org/licenses/MIT
// ------------------------------------------------------------------------------------------------

using System;
using System.Xml.Serialization;

namespace TcPlayer.Network.Models.Browse
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public class Envelope
    {
        public Envelope()
        {
            Body = new EnvelopeBody();
        }

        public EnvelopeBody Body
        {
            get;
            set;
        }

        
        [XmlAttribute(AttributeName = "encodingStyle", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string? EncodingStyle
        {
            get;
            set;
        }
    }


}
