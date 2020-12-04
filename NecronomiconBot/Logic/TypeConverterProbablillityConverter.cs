using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NecronomiconBot.Logic
{
    class TypeConverterProbablillityConverter : TypeConverter
    {
        private static readonly Regex  regex = new Regex(@"^\s*(\d+)\b\s*(?:in|\/)\s*\b(\d+)\s*$");
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is null)
            {
                return null;
            }
            if(value is string)
            {
                string stringValue = (string)value;
                if (float.TryParse(stringValue, out var result))
                {
                    return new Probablility(result);
                }
                var groups = regex.Match(stringValue).Groups;
                if (groups.Count > 0)
                {
                    return new Probablility(float.Parse(groups[1].Value) / float.Parse(groups[2].Value) * 100);
                }
                else
                {
                    return new Probablility(float.Parse(stringValue));
                }
                
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((Probablility)value).ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            if (value is string)
            {
                string stringValue = (string)value;
                return float.TryParse(stringValue, out _) || regex.IsMatch(stringValue);
            }
            return base.IsValid(context, value);
        }
    }
}
