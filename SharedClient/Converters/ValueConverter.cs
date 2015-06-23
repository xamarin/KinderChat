using System;

namespace KinderChat.Converters
{
    public abstract class ValueConverter<TOriginalValue, TConvertedValue>
    {
        public abstract TConvertedValue Convert(TOriginalValue value);

        public virtual TOriginalValue ConvertBack(TConvertedValue value) 
        {
            throw new NotSupportedException();
        }
    }
}
