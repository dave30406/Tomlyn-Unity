// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Tomlyn;
using System;

/// <summary>
/// A datetime value that can represent a TOML
/// - An offset DateTime with Zero offset or Number offset.
/// - A local DateTime.
/// - A local Date.
/// - A local Time.
/// </summary>
/// <param name="DateTime">The datetime offset.</param>
/// <param name="SecondPrecision">The precision of milliseconds.</param>
/// <param name="Kind">The kind of datetime offset.</param>
public record struct TomlDateTime(DateTimeOffset DateTime, int SecondPrecision, TomlDateTimeKind Kind) : IConvertible
{
    public TomlDateTime(int year, int month, int day) : this(new DateTimeOffset(new DateTime(year, month, day)), 0, TomlDateTimeKind.LocalDate)
    {
    }

    public TomlDateTime(DateTime datetime) : this(new DateTimeOffset(datetime), 0, TomlDateTimeKind.LocalDateTime)
    {
    }

    public override string ToString() => ((IConvertible)this).ToString(CultureInfo.InvariantCulture);

    [ExcludeFromCodeCoverage]
    TypeCode IConvertible.GetTypeCode()
    {
        return TypeCode.Object;
    }

    [ExcludeFromCodeCoverage]
    bool IConvertible.ToBoolean(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    byte IConvertible.ToByte(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    char IConvertible.ToChar(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    DateTime IConvertible.ToDateTime(IFormatProvider? provider)
    {
        return DateTime.DateTime;
    }

    [ExcludeFromCodeCoverage]
    decimal IConvertible.ToDecimal(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    double IConvertible.ToDouble(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    short IConvertible.ToInt16(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    int IConvertible.ToInt32(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    long IConvertible.ToInt64(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    sbyte IConvertible.ToSByte(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    float IConvertible.ToSingle(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    string IConvertible.ToString(IFormatProvider? provider)
    {
        switch (Kind)
        {
            case TomlDateTimeKind.LocalDateTime:
                if (DateTime.Millisecond == 0)
                    return DateTime.ToString("yyyy-MM-dd'T'HH:mm:ss", provider);
                return DateTime.ToString($"yyyy-MM-dd'T'HH:mm:ss.{GetFormatPrecision(SecondPrecision)}", provider);
            case TomlDateTimeKind.LocalDate:
                return DateTime.ToString("yyyy-MM-dd", provider);
            case TomlDateTimeKind.LocalTime:
                if (DateTime.Millisecond == 0)
                    return DateTime.ToString("HH:mm:ss", provider);
                return DateTime.ToString($"HH:mm:ss.{GetFormatPrecision(SecondPrecision)}", provider);
            case TomlDateTimeKind.OffsetDateTimeByNumber:
            {
                var time = DateTime.ToLocalTime();
                if (time.Millisecond == 0)
                    return time.ToString("yyyy-MM-dd'T'HH:mm:sszzz", provider);
                return time.ToString($"yyyy-MM-dd'T'HH:mm:ss.{GetFormatPrecision(SecondPrecision)}zzz", provider);
            }
            case TomlDateTimeKind.OffsetDateTimeByZ:
            default:
            {
                var time = DateTime.ToUniversalTime();
                if (time.Millisecond == 0)
                    return time.ToString("yyyy-MM-dd'T'HH:mm:ssZ", provider);
                return time.ToString($"yyyy-MM-dd'T'HH:mm:ss.{GetFormatPrecision(SecondPrecision)}Z", provider);
            }
        }
    }

    object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
    {
        if (conversionType == typeof(DateTime))
        {
            return DateTime.DateTime;
        }

        if (conversionType == typeof(DateTimeOffset))
        {
            return DateTime;
        }

#if NET6_0_OR_GREATER
        if (conversionType == typeof(DateOnly))
        {
            return DateOnly.FromDateTime(DateTime.DateTime);
        }

        if (conversionType == typeof(TimeOnly))
        {
            return TimeOnly.FromDateTime(DateTime.DateTime);
        }
#endif

        throw new InvalidCastException($"Unable to convert {nameof(TomlDateTime)} to destination type {conversionType.FullName}");
    }

    [ExcludeFromCodeCoverage]
    ushort IConvertible.ToUInt16(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    uint IConvertible.ToUInt32(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    ulong IConvertible.ToUInt64(IFormatProvider? provider)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Gets the string formatter for the specified precision.
    /// </summary>
    /// <param name="precision">A precision number from 1 to 7. Other numbers will return the default `fff` formatter.</param>
    /// <returns>The string formatter for the specified precision.</returns>
    public static string GetFormatPrecision(int precision)
    {
        switch (precision)
        {
            case 1: return "f";
            case 2: return "ff";
            case 3: return "fff";
            case 4: return "ffff";
            case 5: return "fffff";
            case 6: return "ffffff";
            case 7: return "fffffff";
            default:
                return "fff";
        }
    }

    /// <summary>
    /// Converts a datetime to TomlDateTime.
    /// </summary>
    public static implicit operator TomlDateTime(DateTime dateTime)
    {
        return new TomlDateTime(dateTime, 0, TomlDateTimeKind.LocalDateTime);
    }
}