using System.IO;
using System.Text;
using System.Text.Json;
using MarkdownJsonEditor.Models;

namespace MarkdownJsonEditor.Services
{
    public class JsonService
    {
        private readonly JsonSerializerOptions _options;
        private readonly JsonSerializerOptions _exportOptions;

        public JsonService()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            // Separate Optionen für Export mit korrektem Escaping
            _exportOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Default // Standard Escaping für \n, \r, \t etc.
            };
        }

        public string LoadFromFile(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            return ExtractContent(jsonString);
        }

        public string ExtractContent(string jsonString)
        {
            try
            {
                var content = JsonSerializer.Deserialize<JsonContent>(jsonString);
                if (content == null) return jsonString;

                var sb = new StringBuilder();

                // Extract title
                if (!string.IsNullOrEmpty(content.Title))
                {
                    sb.AppendLine($"# {content.Title}");
                    sb.AppendLine();
                }

                // Extract intro
                if (!string.IsNullOrEmpty(content.Intro))
                {
                    sb.AppendLine(content.Intro);
                    sb.AppendLine();
                }

                // Extract markdown from sections
                if (content.Sections != null && content.Sections.Any())
                {
                    foreach (var section in content.Sections)
                    {
                        if (!string.IsNullOrEmpty(section.Markdown))
                        {
                            sb.AppendLine(section.Markdown);
                        }
                    }
                }

                // Fallback to simple content property
                if (sb.Length == 0 && !string.IsNullOrEmpty(content.Content))
                {
                    return content.Content;
                }

                return sb.Length > 0 ? sb.ToString() : jsonString;
            }
            catch
            {
                // If parsing fails, try legacy format
                try
                {
                    using var doc = JsonDocument.Parse(jsonString);
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.String)
                    {
                        return root.GetString() ?? string.Empty;
                    }
                    if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("content", out var contentElement))
                    {
                        return contentElement.GetString() ?? string.Empty;
                    }
                }
                catch { }

                return jsonString;
            }
        }

        public string ExportAsJsonString(string markdown)
        {
            // Verwende exportOptions mit korrektem Escaping für \n
            var jsonString = JsonSerializer.Serialize(markdown, _exportOptions);
            
            System.Diagnostics.Debug.WriteLine($"[JsonService] Exported string length: {jsonString.Length}");
            System.Diagnostics.Debug.WriteLine($"[JsonService] Contains escaped newlines: {jsonString.Contains("\\n")}");
            
            return jsonString;
        }
    }
}


