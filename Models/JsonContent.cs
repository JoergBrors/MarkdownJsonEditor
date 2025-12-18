using System.Text.Json.Serialization;

namespace MarkdownJsonEditor.Models
{
    public class JsonContent
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        
        [JsonPropertyName("intro")]
        public string? Intro { get; set; }
        
        [JsonPropertyName("meta")]
        public MetaData? Meta { get; set; }
        
        [JsonPropertyName("sections")]
        public List<Section>? Sections { get; set; }
        
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
    
    public class MetaData
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        
        [JsonPropertyName("keywords")]
        public List<string>? Keywords { get; set; }
    }
    
    public class Section
    {
        [JsonPropertyName("markdown")]
        public string? Markdown { get; set; }
    }
}

