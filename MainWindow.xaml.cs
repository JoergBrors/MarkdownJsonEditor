using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using MarkdownJsonEditor.Services;
using Microsoft.Web.WebView2.Core;
using System.Text.Json;
using System.Windows.Threading;

namespace MarkdownJsonEditor
{
    public partial class MainWindow : Window
    {
        private readonly MarkdownService _markdownService;
        private readonly JsonService _jsonService;
        private readonly ImageService _imageService;
        private readonly EditorService _editorService;
        private bool _isWebViewInitialized;
        private bool _isEditorInitialized;
        private readonly DispatcherTimer _updateTimer;
        private string _lastEditorText = "";

        public MainWindow()
        {
            InitializeComponent();
            _markdownService = new MarkdownService();
            _jsonService = new JsonService();
            _imageService = new ImageService();
            _editorService = new EditorService();
            _isWebViewInitialized = false;
            _isEditorInitialized = false;
            
            // Timer für regelmäßige Updates (300ms)
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _updateTimer.Tick += UpdateTimer_Tick;
            
            Loaded += MainWindow_Loaded;
        }

        private async void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (!_isWebViewInitialized || !_isEditorInitialized) return;
            
            try
            {
                var currentText = await GetEditorText();
                
                // Nur updaten wenn sich der Text geändert hat
                if (currentText != _lastEditorText)
                {
                    System.Diagnostics.Debug.WriteLine($"Text changed, updating preview. Length: {currentText?.Length ?? 0}");
                    _lastEditorText = currentText ?? "";
                    await UpdatePreviewWithText(currentText ?? "");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Timer tick error: {ex.Message}");
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initialize Preview
                await PreviewBrowser.EnsureCoreWebView2Async();
                _isWebViewInitialized = true;
                
                var previewHtml = _markdownService.GetInitialPreviewHtml();
                PreviewBrowser.NavigateToString(previewHtml);
                
                PreviewBrowser.NavigationCompleted += async (s, args) =>
                {
                    System.Diagnostics.Debug.WriteLine("Preview loaded");
                    await Task.Delay(500);
                    
                    // Teste ob updateMarkdown verfügbar ist
                    try
                    {
                        var testResult = await PreviewBrowser.ExecuteScriptAsync("typeof window.updateMarkdown");
                        System.Diagnostics.Debug.WriteLine($"updateMarkdown type: {testResult}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Check error: {ex.Message}");
                    }
                };
                
                // Initialize Editor
                await EditorBrowser.EnsureCoreWebView2Async();
                EditorBrowser.NavigateToString(_editorService.GetEditorHtml());
                
                EditorBrowser.NavigationCompleted += async (s, args) =>
                {
                    if (!_isEditorInitialized)
                    {
                        _isEditorInitialized = true;
                        UpdateStatus("Bereit");
                        
                        // Starte den Update-Timer
                        _updateTimer.Start();
                        System.Diagnostics.Debug.WriteLine("Update timer started");
                        
                        // Initiales Update
                        await Task.Delay(600);
                        var initialText = await GetEditorText();
                        _lastEditorText = initialText ?? "";
                        await UpdatePreviewWithText(_lastEditorText);
                    }
                };
                
                UpdateStatus("Initialisiere...");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WebView2 Error: {ex.Message}\n\nStellen Sie sicher, dass WebView2 Runtime installiert ist.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<string> GetEditorText()
        {
            if (!_isEditorInitialized) return string.Empty;
            
            try
            {
                var result = await EditorBrowser.ExecuteScriptAsync("getText()");
                var text = JsonSerializer.Deserialize<string>(result) ?? string.Empty;
                return text;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetEditorText Error: {ex.Message}");
                return string.Empty;
            }
        }

        private async Task UpdatePreviewWithText(string text)
        {
            if (!_isWebViewInitialized) return;
            
            try
            {
                var escapedText = JsonSerializer.Serialize(text);
                var script = $"window.updateMarkdown({escapedText})";
                
                var result = await PreviewBrowser.ExecuteScriptAsync(script);
                System.Diagnostics.Debug.WriteLine($"Preview updated, result: {result}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdatePreviewWithText error: {ex.Message}");
            }
        }

        private async void SetEditorText(string text)
        {
            if (!_isEditorInitialized)
            {
                await Task.Delay(100);
                if (!_isEditorInitialized)
                {
                    MessageBox.Show("Editor noch nicht bereit.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            
            try
            {
                var escapedText = JsonSerializer.Serialize(text);
                await EditorBrowser.ExecuteScriptAsync($"setText({escapedText})");
                _lastEditorText = text;
                
                await Task.Delay(100);
                await UpdatePreviewWithText(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadJsonButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files|*.json|All files|*.*",
                Title = "JSON Datei öffnen"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Lade IMMER alle Sections
                    var sections = _jsonService.LoadSectionsFromFile(openFileDialog.FileName);
                    
                    System.Diagnostics.Debug.WriteLine($"[LoadJSON] Found {sections.Count} sections");
                    foreach (var section in sections)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - {section.Title}: {section.Content.Length} chars");
                    }
                    
                    if (sections.Count > 1)
                    {
                        // Zeige IMMER Section Navigator bei mehreren Sections
                        var navigator = new SectionNavigatorWindow(sections);
                        navigator.Owner = this;
                        
                        if (navigator.ShowDialog() == true)
                        {
                            if (navigator.SelectedSectionIndex == -2)
                            {
                                // Combine all sections
                                var combined = string.Join("\n\n---\n\n", sections.Select(s => $"<!-- {s.Title} -->\n{s.Content}"));
                                SetEditorText(combined);
                                UpdateStatus($"Geladen: Alle {sections.Count} Sections kombiniert");
                            }
                            else if (navigator.SelectedSectionIndex >= 0)
                            {
                                // Load ONLY selected section
                                var selectedSection = sections[navigator.SelectedSectionIndex];
                                SetEditorText(selectedSection.Content);
                                UpdateStatus($"Geladen: {selectedSection.Title} (Section {navigator.SelectedSectionIndex + 1}/{sections.Count})");
                            }
                        }
                    }
                    else if (sections.Count == 1)
                    {
                        // Nur eine Section, direkt laden
                        SetEditorText(sections[0].Content);
                        UpdateStatus($"Geladen: {sections[0].Title}");
                    }
                    else
                    {
                        // Fallback wenn keine Sections erkannt wurden
                        var content = _jsonService.LoadFromFile(openFileDialog.FileName);
                        SetEditorText(content);
                        UpdateStatus($"Geladen: {System.IO.Path.GetFileName(openFileDialog.FileName)}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    System.Diagnostics.Debug.WriteLine($"LoadJSON Error: {ex}");
                }
            }
        }

        private async void ImportClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Clipboard.ContainsText())
                {
                    MessageBox.Show("Zwischenablage enthält keinen Text.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var clipboardText = Clipboard.GetText();
                
                if (string.IsNullOrWhiteSpace(clipboardText))
                {
                    MessageBox.Show("Zwischenablage ist leer.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                System.Diagnostics.Debug.WriteLine("=== CLIPBOARD IMPORT ===");
                System.Diagnostics.Debug.WriteLine($"Clipboard length: {clipboardText.Length}");
                System.Diagnostics.Debug.WriteLine($"First 100 chars: {clipboardText.Substring(0, Math.Min(100, clipboardText.Length))}");
                System.Diagnostics.Debug.WriteLine($"Contains literal \\n: {clipboardText.Contains("\\n")}");

                // Verarbeite Zwischenablage-Inhalt
                var content = _jsonService.LoadFromClipboard(clipboardText);
                
                // WICHTIG: Auch wenn es kein JSON ist, normalisiere die Zeilenumbrüche
                // Das behandelt den Fall wenn jemand einen String mit \n direkt einfügt
                if (!clipboardText.TrimStart().StartsWith("{") && !clipboardText.TrimStart().StartsWith("["))
                {
                    // Kein JSON, aber könnte literale \n enthalten
                    if (content.Contains("\\n"))
                    {
                        System.Diagnostics.Debug.WriteLine("Detected literal \\n in plain text, normalizing...");
                        content = content.Replace("\\n\\n", "\n\n")
                                       .Replace("\\n", "\n")
                                       .Replace("\\t", "\t");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Extracted content length: {content.Length}");
                System.Diagnostics.Debug.WriteLine($"After normalization - First 100 chars: {content.Substring(0, Math.Min(100, content.Length))}");
                System.Diagnostics.Debug.WriteLine("=== END IMPORT ===");

                if (string.IsNullOrWhiteSpace(content))
                {
                    MessageBox.Show("Kein gültiger Inhalt in der Zwischenablage gefunden.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Frage ob bestehender Inhalt ersetzt oder angehängt werden soll
                var currentText = await GetEditorText();
                if (!string.IsNullOrWhiteSpace(currentText))
                {
                    var result = MessageBox.Show(
                        "Möchten Sie den bestehenden Inhalt ersetzen?\n\n" +
                        "Ja = Ersetzen\n" +
                        "Nein = Anhängen\n" +
                        "Abbrechen = Abbrechen",
                        "Import",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        // Anhängen
                        content = currentText + "\n\n" + content;
                    }
                }

                SetEditorText(content);
                
                // Zeige Info über importierten Inhalt
                var lines = content.Split('\n').Length;
                var isJson = clipboardText.TrimStart().StartsWith("{") || clipboardText.TrimStart().StartsWith("[");
                var hadLiteralNewlines = clipboardText.Contains("\\n");
                var importType = isJson ? "JSON" : (hadLiteralNewlines ? "Text (mit \\n konvertiert)" : "Text");
                
                UpdateStatus($"Importiert: {importType}, {lines} Zeilen");
                MessageBox.Show(
                    $"Import erfolgreich!\n\n" +
                    $"Typ: {importType}\n" +
                    $"Zeichen: {content.Length}\n" +
                    $"Zeilen: {lines}",
                    "Import erfolgreich",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Import: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Import error: {ex}");
            }
        }

        private async void InsertImageButton_Click(object sender, RoutedEventArgs e)
        {
            var imagePath = _imageService.SelectImage();
            if (!string.IsNullOrEmpty(imagePath))
            {
                var result = MessageBox.Show("Möchten Sie Bildausrichtung festlegen?", "Bildoptionen", MessageBoxButton.YesNo);
                string? align = null;
                string? width = null;
                if (result == MessageBoxResult.Yes)
                {
                    align = "right";
                    width = "40%";
                }
                var markdown = _imageService.GenerateMarkdownImage(imagePath, "Image", align, width);
                await InsertText(markdown);
                UpdateStatus("Bild eingefügt");
            }
        }

        private async void ExportJsonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var text = await GetEditorText();
                
                // Debug: Zeige den rohen Text
                System.Diagnostics.Debug.WriteLine("=== EXPORT DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Text length: {text.Length}");
                System.Diagnostics.Debug.WriteLine($"Line breaks (\\n): {text.Count(c => c == '\n')}");
                System.Diagnostics.Debug.WriteLine($"Line breaks (\\r\\n): {text.Split(new[] { "\r\n" }, StringSplitOptions.None).Length - 1}");
                System.Diagnostics.Debug.WriteLine($"First 200 chars: {text.Substring(0, Math.Min(200, text.Length))}");
                
                // Ersetze CRLF durch LF für konsistente Ausgabe
                text = text.Replace("\r\n", "\n");
                
                var jsonString = _jsonService.ExportAsJsonString(text);
                
                System.Diagnostics.Debug.WriteLine($"JSON length: {jsonString.Length}");
                System.Diagnostics.Debug.WriteLine($"JSON contains \\n: {jsonString.Contains("\\n")}");
                System.Diagnostics.Debug.WriteLine($"JSON first 200 chars: {jsonString.Substring(0, Math.Min(200, jsonString.Length))}");
                System.Diagnostics.Debug.WriteLine("=== END DEBUG ===");
                
                Clipboard.SetText(jsonString);
                UpdateStatus("JSON in Zwischenablage kopiert");
                
                // Zeige Info mit Statistik
                var lines = text.Split('\n').Length;
                MessageBox.Show($"JSON exportiert\n\nZeichen: {text.Length}\nZeilen: {lines}\n\nDer JSON-String wurde in die Zwischenablage kopiert.", "Export erfolgreich", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Inhalt löschen?", "Bestätigung", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SetEditorText(string.Empty);
                UpdateStatus("Gelöscht");
            }
        }

        private async void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isEditorInitialized) return;
            try
            {
                await EditorBrowser.ExecuteScriptAsync("performUndo()");
                UpdateStatus("Rückgängig");
            }
            catch { }
        }

        private async void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isEditorInitialized) return;
            try
            {
                await EditorBrowser.ExecuteScriptAsync("performRedo()");
                UpdateStatus("Wiederholen");
            }
            catch { }
        }

        private async void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            await WrapSelection("**", "**");
            UpdateStatus("Fett");
        }

        private async void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            await WrapSelection("*", "*");
            UpdateStatus("Kursiv");
        }

        private async void H1Button_Click(object sender, RoutedEventArgs e)
        {
            await InsertAtLineStart("# ");
            UpdateStatus("H1");
        }

        private async void H2Button_Click(object sender, RoutedEventArgs e)
        {
            await InsertAtLineStart("## ");
            UpdateStatus("H2");
        }

        private async void H3Button_Click(object sender, RoutedEventArgs e)
        {
            await InsertAtLineStart("### ");
            UpdateStatus("H3");
        }

        private async void ListButton_Click(object sender, RoutedEventArgs e)
        {
            await InsertAtLineStart("- ");
            UpdateStatus("Liste");
        }

        private async void NumberedListButton_Click(object sender, RoutedEventArgs e)
        {
            await InsertAtLineStart("1. ");
            UpdateStatus("Nummeriert");
        }

        private async void QuoteButton_Click(object sender, RoutedEventArgs e)
        {
            await InsertAtLineStart("> ");
            UpdateStatus("Zitat");
        }

        private async void LinkButton_Click(object sender, RoutedEventArgs e)
        {
            await WrapSelection("[", "](https://url.de)");
            UpdateStatus("Link");
        }

        private async void CodeButton_Click(object sender, RoutedEventArgs e)
        {
            await WrapSelection("`", "`");
            UpdateStatus("Code");
        }

        private async void HighlightYellowButton_Click(object sender, RoutedEventArgs e)
        {
            await WrapSelection("==", "==");
            UpdateStatus("Gelb");
        }

        private async void HighlightGreenButton_Click(object sender, RoutedEventArgs e)
        {
            await WrapSelection("<span style='background-color: #90EE90'>", "</span>");
            UpdateStatus("Grün");
        }

        private async void HighlightRedButton_Click(object sender, RoutedEventArgs e)
        {
            await WrapSelection("<span style='background-color: #FFB6C1'>", "</span>");
            UpdateStatus("Rot");
        }

        private async void HighlightBlueButton_Click(object sender, RoutedEventArgs e)
        {
            await WrapSelection("<span style='background-color: #ADD8E6'>", "</span>");
            UpdateStatus("Blau");
        }

        private async Task WrapSelection(string before, string after)
        {
            if (!_isEditorInitialized) return;
            
            try
            {
                var beforeJson = JsonSerializer.Serialize(before);
                var afterJson = JsonSerializer.Serialize(after);
                await EditorBrowser.ExecuteScriptAsync($"wrapSelection({beforeJson}, {afterJson})");
            }
            catch { }
        }

        private async Task InsertAtLineStart(string prefix)
        {
            if (!_isEditorInitialized) return;
            
            try
            {
                var prefixJson = JsonSerializer.Serialize(prefix);
                await EditorBrowser.ExecuteScriptAsync($"insertAtLineStart({prefixJson})");
            }
            catch { }
        }

        private async Task InsertText(string text)
        {
            if (!_isEditorInitialized) return;
            
            try
            {
                var textJson = JsonSerializer.Serialize(text);
                await EditorBrowser.ExecuteScriptAsync($"insertText({textJson})");
            }
            catch { }
        }

        private void UpdateStatus(string message)
        {
            StatusText.Text = message;
        }
        
        protected override void OnClosed(EventArgs e)
        {
            _updateTimer?.Stop();
            base.OnClosed(e);
        }
    }

    public class EditorMessage
    {
        public string? type { get; set; }
        public string? content { get; set; }
    }
}

