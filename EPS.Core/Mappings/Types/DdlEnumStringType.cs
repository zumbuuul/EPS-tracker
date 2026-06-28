using System;
using NHibernate.Type;

namespace app.Mappings.Types
{
    public class DdlEnumStringType<TEnum> : EnumStringType where TEnum : struct
    {
        public DdlEnumStringType()
            : base(typeof(TEnum), 50)
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("DdlEnumStringType can only be used with enum types.");
            }
        }
    }
}
