using System;
using System.Collections.Generic;
using System.Text;

namespace NecronomiconBot.Settings
{

    [Serializable]
    public class SettingNotFoundException : NecronomiconException
    {
        public SettingNotFoundException() { }
        public SettingNotFoundException(string message) : base(message) { }
        public SettingNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected SettingNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
