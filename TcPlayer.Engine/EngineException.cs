using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TcPlayer.Engine
{
    [Serializable]
    public sealed class EngineException : Exception
    {
        public EngineException()
        {
        }

        public EngineException(string message) : base(message)
        {
        }

        public EngineException(string message, Exception innerException) : base(message, innerException)
        {
        }

        private EngineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
