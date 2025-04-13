//复制自9.0.4

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace BoysheO.Extensions.Configuration.Reloadable.Json
{
    public sealed class JsonConfigurationFileParser
    {
        private JsonConfigurationFileParser()
        {
        }

        private readonly Dictionary<string, string?> _data =
            new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        private readonly Stack<string> _paths = new Stack<string>();

        public static IDictionary<string, string?> Parse(Stream input)
            => new JsonConfigurationFileParser().ParseStream(input);

        private Dictionary<string, string?> ParseStream(Stream input)
        {
            var jsonDocumentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            using (var reader = new StreamReader(input))
            using (JsonDocument doc = JsonDocument.Parse(reader.ReadToEnd(), jsonDocumentOptions))
            {
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new FormatException(
                        $"Top-level JSON element must be an object. Instead, '{doc.RootElement.ValueKind}' was found.");
                }

                VisitObjectElement(doc.RootElement);
            }

            return _data;
        }

        private void VisitObjectElement(JsonElement element)
        {
            var isEmpty = true;

            foreach (JsonProperty property in element.EnumerateObject())
            {
                isEmpty = false;
                EnterContext(property.Name);
                VisitValue(property.Value);
                ExitContext();
            }

            SetNullIfElementIsEmpty(isEmpty);
        }

        private void VisitArrayElement(JsonElement element)
        {
            int index = 0;

            foreach (JsonElement arrayElement in element.EnumerateArray())
            {
                EnterContext(index.ToString());
                VisitValue(arrayElement);
                ExitContext();
                index++;
            }

            SetNullIfElementIsEmpty(isEmpty: index == 0);
        }

        private void SetNullIfElementIsEmpty(bool isEmpty)
        {
            if (isEmpty && _paths.Count > 0)
            {
                _data[_paths.Peek()] = null;
            }
        }

        private void VisitValue(JsonElement value)
        {
            Debug.Assert(_paths.Count > 0);

            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                    VisitObjectElement(value);
                    break;

                case JsonValueKind.Array:
                    VisitArrayElement(value);
                    break;

                case JsonValueKind.Number:
                case JsonValueKind.String:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    string key = _paths.Peek();
                    if (_data.ContainsKey(key))
                    {
                        throw new FormatException($"A duplicate key '{key}' was found.");
                    }

                    _data[key] = value.ToString();
                    break;

                default:
                    throw new FormatException($"Unsupported JSON token '{value.ValueKind}' was found.");
            }
        }

        private void EnterContext(string context) =>
            _paths.Push(_paths.Count > 0 ? _paths.Peek() + ConfigurationPath.KeyDelimiter + context : context);

        private void ExitContext() => _paths.Pop();
    }
}