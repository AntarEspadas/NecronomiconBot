using System;
using System.Collections.Generic;
using System.Text;

namespace NecronomiconBot.Settings
{

    [Serializable]
    public class TypeMissmatchException : NecronomiconException
    {
        public TypeMissmatchException() { }
        public TypeMissmatchException(string message) : base(message) { }
        public TypeMissmatchException(string message, Exception inner) : base(message, inner) { }
        protected TypeMissmatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
