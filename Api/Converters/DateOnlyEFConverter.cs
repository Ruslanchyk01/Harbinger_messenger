using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Converters
{
    public class DateOnlyEFConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyEFConverter()
            : base(d => d.ToDateTime(TimeOnly.MinValue), dt => DateOnly.FromDateTime(dt))
            {}
    }
}