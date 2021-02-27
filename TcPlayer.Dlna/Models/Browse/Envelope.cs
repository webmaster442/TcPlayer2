using System;
using System.Xml.Serialization;

namespace TcPlayer.Dlna.Models.Browse
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class Envelope
    {
        public EnvelopeBody Body
        {
            get;
            set;
        }

        
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string encodingStyle
        {
            get;
            set;
        }
    }


}
