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
            
            _exportOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Default
            };
        }

        public string LoadFromFile(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            return ExtractContent(jsonString);
        }

        public List<MarkdownSection> LoadSectionsFromFile(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            return ExtractSections(jsonString);
        }

        public string LoadFromClipboard(string clipboardText)
        {
            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                return string.Empty;
            }

            if (clipboardText.TrimStart().StartsWith("{") || clipboardText.TrimStart().StartsWith("["))
            {
                try
                {
                    var content = ExtractContent(clipboardText);
                    // Normalisiere Zeilenumbrüche: Ersetze literale \n durch echte Zeilenumbrüche
                    return NormalizeLineBreaks(content);
                }
                catch
                {
                    return clipboardText;
                }
            }

            return clipboardText;
        }

        private string NormalizeLineBreaks(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            // Ersetze literale \n\n durch echte doppelte Zeilenumbrüche (Absätze)
            text = text.Replace("\\n\\n", "\n\n");
            
            // Ersetze verbleibende literale \n durch echte Zeilenumbrüche
            text = text.Replace("\\n", "\n");
            
            // Ersetze literale \t durch echte Tabs
            text = text.Replace("\\t", "\t");
            
            return text;
        }

        public List<MarkdownSection> ExtractSections(string jsonString)
        {
            var sections = new List<MarkdownSection>();
            
            try
            {
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    // Durchsuche rekursiv alle Properties
                    ExtractSectionsRecursive(root, "", sections);
                }
                else if (root.ValueKind == JsonValueKind.Array)
                {
                    // Root ist Array (z.B. slides)
                    int index = 0;
                    foreach (var item in root.EnumerateArray())
                    {
                        ExtractSectionsRecursive(item, $"[{index}]", sections);
                        index++;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[ExtractSections] Found {sections.Count} sections:");
                foreach (var section in sections)
                {
                    var preview = section.Content.Length > 50 ? section.Content.Substring(0, 50) + "..." : section.Content;
                    System.Diagnostics.Debug.WriteLine($"  - {section.Title}: {preview}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExtractSections] Error: {ex.Message}");
            }

            return sections;
        }

        private void ExtractSectionsRecursive(JsonElement element, string path, List<MarkdownSection> sections, int depth = 0)
        {
            // Maximale Tiefe um Endlosschleifen zu vermeiden
            if (depth > 20) return;

            if (element.ValueKind == JsonValueKind.Object)
            {
                // Spezialbehandlung für "sections" Array
                if (element.TryGetProperty("sections", out var sectionsArray) && sectionsArray.ValueKind == JsonValueKind.Array)
                {
                    int index = 0;
                    foreach (var item in sectionsArray.EnumerateArray())
                    {
                        ExtractSectionsRecursive(item, $"sections[{index}]", sections, depth + 1);
                        index++;
                    }
                    return; // Nicht weiter durchsuchen wenn sections gefunden
                }

                // Spezialbehandlung für "slides" Array
                if (element.TryGetProperty("slides", out var slidesArray) && slidesArray.ValueKind == JsonValueKind.Array)
                {
                    int index = 0;
                    foreach (var item in slidesArray.EnumerateArray())
                    {
                        ExtractSectionsRecursive(item, $"slides[{index}]", sections, depth + 1);
                        index++;
                    }
                    return;
                }

                // Durchsuche alle Properties
                foreach (var prop in element.EnumerateObject())
                {
                    var propName = prop.Name;
                    var currentPath = string.IsNullOrEmpty(path) ? propName : $"{path}.{propName}";

                    if (prop.Value.ValueKind == JsonValueKind.String)
                    {
                        var text = prop.Value.GetString();
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            // Prüfe ob es Markdown-ähnlicher Content ist (länger als 20 Zeichen oder enthält Markdown-Syntax)
                            var isMarkdownLike = text.Length > 20 || 
                                                text.Contains("##") || 
                                                text.Contains("**") || 
                                                text.Contains("- ") ||
                                                text.Contains("\n");

                            // Erstelle Section für relevante Texte
                            if (isMarkdownLike || IsMarkdownProperty(propName))
                            {
                                sections.Add(new MarkdownSection
                                {
                                    Title = currentPath,
                                    Content = NormalizeLineBreaks(text)
                                });
                            }
                        }
                    }
                    else if (prop.Value.ValueKind == JsonValueKind.Object)
                    {
                        // Rekursiv in Objekte
                        ExtractSectionsRecursive(prop.Value, currentPath, sections, depth + 1);
                    }
                    else if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        // Arrays durchsuchen
                        int arrayIndex = 0;
                        foreach (var arrayItem in prop.Value.EnumerateArray())
                        {
                            if (arrayItem.ValueKind == JsonValueKind.Object)
                            {
                                ExtractSectionsRecursive(arrayItem, $"{currentPath}[{arrayIndex}]", sections, depth + 1);
                            }
                            else if (arrayItem.ValueKind == JsonValueKind.String)
                            {
                                var text = arrayItem.GetString();
                                if (!string.IsNullOrWhiteSpace(text) && text.Length > 20)
                                {
                                    sections.Add(new MarkdownSection
                                    {
                                        Title = $"{currentPath}[{arrayIndex}]",
                                        Content = NormalizeLineBreaks(text)
                                    });
                                }
                            }
                            arrayIndex++;
                        }
                    }
                }
            }
        }

        private bool IsMarkdownProperty(string propertyName)
        {
            var markdownIndicators = new[] { "markdown", "description", "content", "text", "body", "intro", "en", "de", "fr", "es" };
            var lowerName = propertyName.ToLower();
            return markdownIndicators.Any(indicator => lowerName.Contains(indicator));
        }

        public string ExtractContent(string jsonString)
        {
            try
            {
                var content = JsonSerializer.Deserialize<JsonContent>(jsonString);
                if (content == null) return jsonString;

                var sb = new StringBuilder();

                if (!string.IsNullOrEmpty(content.Title))
                {
                    sb.AppendLine($"# {content.Title}");
                    sb.AppendLine();
                }

                if (!string.IsNullOrEmpty(content.Intro))
                {
                    sb.AppendLine(content.Intro);
                    sb.AppendLine();
                }

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

                if (sb.Length == 0 && !string.IsNullOrEmpty(content.Content))
                {
                    return NormalizeLineBreaks(content.Content);
                }

                var result = sb.Length > 0 ? sb.ToString() : TryExtractFromGenericJson(jsonString);
                return NormalizeLineBreaks(result);
            }
            catch
            {
                return NormalizeLineBreaks(TryExtractFromGenericJson(jsonString));
            }
        }

        private string TryExtractFromGenericJson(string jsonString)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.String)
                {
                    return root.GetString() ?? string.Empty;
                }

                var sb = new StringBuilder();
                
                if (root.ValueKind == JsonValueKind.Object)
                {
                    var searchProperties = new[] { "markdown", "content", "text", "en", "de", "body", "description" };
                    
                    foreach (var prop in searchProperties)
                    {
                        if (root.TryGetProperty(prop, out var element))
                        {
                            if (element.ValueKind == JsonValueKind.String)
                            {
                                var text = element.GetString();
                                if (!string.IsNullOrEmpty(text))
                                {
                                    sb.AppendLine(text);
                                    sb.AppendLine();
                                }
                            }
                        }
                    }

                    if (sb.Length == 0)
                    {
                        ExtractAllStrings(root, sb);
                    }
                }
                else if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in root.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            sb.AppendLine(item.GetString());
                            sb.AppendLine();
                        }
                        else if (item.ValueKind == JsonValueKind.Object)
                        {
                            ExtractAllStrings(item, sb);
                        }
                    }
                }

                return sb.Length > 0 ? sb.ToString().TrimEnd() : jsonString;
            }
            catch
            {
                return jsonString;
            }
        }

        private void ExtractAllStrings(JsonElement element, StringBuilder sb, int depth = 0)
        {
            if (depth > 10) return;

            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    var propName = property.Name.ToLower();
                    var isMarkdownProperty = propName.Contains("markdown") || 
                                           propName.Contains("content") || 
                                           propName.Contains("text") ||
                                           propName == "en" || 
                                           propName == "de" ||
                                           propName.Contains("body") ||
                                           propName.Contains("description");

                    if (property.Value.ValueKind == JsonValueKind.String)
                    {
                        var text = property.Value.GetString();
                        if (!string.IsNullOrEmpty(text) && text.Length > 10)
                        {
                            if (isMarkdownProperty)
                            {
                                sb.AppendLine(text);
                                sb.AppendLine();
                            }
                            else if (sb.Length == 0)
                            {
                                sb.AppendLine(text);
                                sb.AppendLine();
                            }
                        }
                    }
                    else if (property.Value.ValueKind == JsonValueKind.Object || 
                             property.Value.ValueKind == JsonValueKind.Array)
                    {
                        ExtractAllStrings(property.Value, sb, depth + 1);
                    }
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    ExtractAllStrings(item, sb, depth + 1);
                }
            }
        }

        public string ExportAsJsonString(string markdown)
        {
            var jsonString = JsonSerializer.Serialize(markdown, _exportOptions);
            
            System.Diagnostics.Debug.WriteLine($"[JsonService] Exported string length: {jsonString.Length}");
            System.Diagnostics.Debug.WriteLine($"[JsonService] Contains escaped newlines: {jsonString.Contains("\\n")}");
            
            return jsonString;
        }
    }

    public class MarkdownSection
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}


