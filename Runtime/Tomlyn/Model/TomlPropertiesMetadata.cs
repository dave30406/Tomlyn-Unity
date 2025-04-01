// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Tomlyn.Syntax;

namespace Tomlyn.Model
{
    [DebuggerDisplay("_properties")]
    public class TomlPropertiesMetadata
    {
        private readonly Dictionary<string, TomlPropertyMetadata> _properties;

        public TomlPropertiesMetadata()
        {
            _properties = new Dictionary<string, TomlPropertyMetadata>();
        }

        public void Clear()
        {
            _properties.Clear();
        }

        public bool ContainsProperty(string propertyKey) => _properties.ContainsKey(propertyKey);

        public bool TryGetProperty(string propertyKey, [NotNullWhen(true)] out TomlPropertyMetadata? propertyMetadata)
        {
            return _properties.TryGetValue(propertyKey, out propertyMetadata);
        }

        public void SetProperty(string propertyKey, TomlPropertyMetadata propertyMetadata)
        {
            _properties[propertyKey] = propertyMetadata;
        }
    }

    public class TomlPropertyMetadata
    {
        /// <summary>
        /// Gets the leading trivia attached to this node. Might be null if no leading trivias.
        /// </summary>
        public List<TomlSyntaxTriviaMetadata>? LeadingTrivia { get; set; }

        public TomlPropertyDisplayKind DisplayKind { get; set; }

        /// <summary>
        /// Gets the trailing trivia attached to this node. Might be null if no trailing trivias.
        /// </summary>
        public List<TomlSyntaxTriviaMetadata>? TrailingTrivia { get; set; }

        /// <summary>
        /// Gets the trailing trivia attached to this node. Might be null if no trailing trivias.
        /// </summary>
        public List<TomlSyntaxTriviaMetadata>? TrailingTriviaAfterEndOfLine { get; set; }

        public SourceSpan Span {get; set;}
    }

    public struct TomlSyntaxTriviaMetadata
    {
        public TomlSyntaxTriviaMetadata(TokenKind kind, string? text)
        {
            Kind = kind;
            Text = text;
        }

        public TokenKind Kind { get; }
        public string? Text { get; }

        public static implicit operator TomlSyntaxTriviaMetadata(SyntaxTrivia trivia)
        {
            return new TomlSyntaxTriviaMetadata(trivia.Kind, trivia.Text);
        }

        public override bool Equals(object? obj)
        {
            return obj is TomlSyntaxTriviaMetadata other &&
                   Kind == other.Kind &&
                   Text == other.Text;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Kind, Text);
        }

        public void Deconstruct(out TokenKind kind, out string? text)
        {
            kind = Kind;
            text = Text;
        }

        public static bool operator ==(TomlSyntaxTriviaMetadata left, TomlSyntaxTriviaMetadata right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TomlSyntaxTriviaMetadata left, TomlSyntaxTriviaMetadata right)
        {
            return !(left == right);
        }
    }

    public enum TomlPropertyDisplayKind
    {
        Default = 0,

        OffsetDateTimeByZ,
        OffsetDateTimeByNumber,
        LocalDateTime,
        LocalDate,
        LocalTime,

        IntegerHexadecimal,

        IntegerOctal,

        IntegerBinary,

        StringMulti,

        StringLiteral,

        StringLiteralMulti,

        InlineTable,
        NoInline
    }
}