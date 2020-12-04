using System;
using System.Collections.Generic;
using System.Text;

namespace NecronomiconBot
{

    [Serializable]
    public class NecronomiconException : Exception
    {
        public NecronomiconException() { }
        public NecronomiconException(string message) : base(message) { }
        public NecronomiconException(string message, Exception inner) : base(message, inner) { }
        protected NecronomiconException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
