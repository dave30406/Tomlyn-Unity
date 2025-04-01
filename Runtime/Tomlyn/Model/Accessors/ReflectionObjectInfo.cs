using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tomlyn.Model.Accessors;

internal readonly struct ReflectionObjectInfo
{
    public ReflectionObjectInfo(ReflectionObjectKind kind)
    {
        Debug.Assert(kind == ReflectionObjectKind.Primitive || kind == ReflectionObjectKind.Object || kind == ReflectionObjectKind.Struct);
        Kind = kind;
        GenericArgument1 = null;
        GenericArgument2 = null;
    }

    public ReflectionObjectInfo(ReflectionObjectKind kind, Type genericArgument1, Type? genericArgument2 = null)
    {
        Kind = kind;
        GenericArgument1 = genericArgument1;
        GenericArgument2 = genericArgument2;
    }

    public readonly ReflectionObjectKind Kind;

    public readonly Type? GenericArgument1;

    public readonly Type? GenericArgument2;

    public static ReflectionObjectInfo Get(Type type)
    {
        if (type == typeof(string) || type.IsPrimitive || type == typeof(TomlDateTime) || type == typeof(DateTime) || type == typeof(DateTimeOffset) || type.IsEnum
#if NET6_0_OR_GREATER
            || type == typeof(DateOnly) || type == typeof(TimeOnly)
#endif
            )
        {
            return new ReflectionObjectInfo(ReflectionObjectKind.Primitive);
        }

        if (type.IsValueType)
        {
            var nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullableUnderlyingType is null)
            {
                return new ReflectionObjectInfo(ReflectionObjectKind.Struct);
            }

            return new ReflectionObjectInfo(nullableUnderlyingType.IsPrimitive ? ReflectionObjectKind.NullablePrimitive : ReflectionObjectKind.NullableStruct, nullableUnderlyingType);
        }

        var interfaces = type.GetInterfaces();
        Type? collectionType = null;
        foreach (var i in interfaces)
        {
            if (i.IsGenericType)
            {
                // Match in priority IDictionary<,>
                var genericTypeDefinition = i.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(IDictionary<,>))
                {
                    var genericArguments = i.GetGenericArguments();
                    return new ReflectionObjectInfo(ReflectionObjectKind.Dictionary, genericArguments[0], genericArguments[1]);
                }

                if (collectionType is null && genericTypeDefinition == typeof(ICollection<>))
                {
                    collectionType = i;
                }
            }
        }

        // Otherwise if we have a collection, match it
        if (collectionType is not null)
        {
            var genericArguments = collectionType.GetGenericArguments();
            return new ReflectionObjectInfo(ReflectionObjectKind.Collection, genericArguments[0]);
        }

        return new ReflectionObjectInfo(ReflectionObjectKind.Object);
    }
}