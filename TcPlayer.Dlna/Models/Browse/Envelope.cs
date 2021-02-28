using System;
using System.Xml.Serialization;

namespace TcPlayer.Dlna.Models.Browse
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public class Envelope
    {
        public EnvelopeBody Body
        {
            get;
            set;
        }

        
        [XmlAttribute(AttributeName = "encodingStyle", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string EncodingStyle
        {
            get;
            set;
        }
    }


}
