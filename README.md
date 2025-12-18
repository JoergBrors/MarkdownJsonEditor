# Markdown JSON WYSIWYG Editor

Ein professioneller Windows Desktop Editor für Markdown mit JSON-Integration und **markdown-it** Rendering.

## 🎉 Version 2.1 - markdown-it Integration!

### ✨ Hauptmerkmale
- **markdown-it JavaScript-basierte Preview** - Identisch mit Edge Browser
- **JavaScript-basierter Editor** (WebView2) 
- **Vollständiges Undo/Redo** mit Strg+Z und Strg+Y
- **Perfekte Tab-Unterstützung** - Tabs werden korrekt eingefügt
- **Korrekte Zeilenumbrüche** - Enter funktioniert wie erwartet
- **Live-Vorschau** mit markdown-it von CDN
- **Scroll-Preservation** - Preview behält Position beim Editieren
- **Signalfarben** - Farbige Markierungen
- JSON Import/Export (einzeiliger, escaped Output)

## Features

### Preview mit markdown-it
- **markdown-it 14.0.0** - Modernster Markdown-Parser
- **markdown-it-mark** - Gelbe Markierungen (`==text==`)
- **markdown-it-attrs** - Attribute für Bilder (`{align=right}`)
- **100% Edge-Browser kompatibel**
- CDN-basiert (keine lokalen Dependencies)

### markdown-it Konfiguration
```javascript
{
    html: true,          // HTML-Tags erlauben
    linkify: true,       // URLs automatisch verlinken
    typographer: true,   // Typografische Ersetzungen
    breaks: true         // Zeilenumbrüche als <br>
}
```

### Editor
- **WebView2-basierter Editor** mit JavaScript
- Monospace Font (Consolas)
- Tab-Größe: 4 Zeichen
- Undo/Redo Stack (bis zu 100 Schritte)
- Automatisches Encoding/Decoding

### Tastenkombinationen im Editor
- **Tab** - Tab-Zeichen einfügen ✅
- **Enter** - Neue Zeile (CRLF) ✅
- **Ctrl+Z** - Rückgängig ✅
- **Ctrl+Y** - Wiederholen ✅
- **Ctrl+Shift+Z** - Wiederholen (Alternative) ✅

### Formatierungs-Toolbar

#### Text-Formatierung
- **B** - Fett (`**text**`)
- **I** - Kursiv (`*text*`)
- **H1, H2, H3** - Überschriften

#### Listen & Zitate
- **List** - Aufzählung (`- item`)
- **1. List** - Nummerierte Liste (`1. item`)
- **Quote** - Zitat (`> text`)

#### Links & Medien
- **Link** - Link einfügen (`[text](url)`)
- **Image** - Bild mit Dialog auswählen
- **Code** - Inline-Code oder Code-Block

#### Signalfarben
- **Yellow** - Gelbe Markierung (`==text==`)
- **Green** - Grüne Markierung (HTML)
- **Red** - Rote Markierung (HTML)
- **Blue** - Blaue Markierung (HTML)

### File Operations
- **JSON laden** - Lädt JSON und extrahiert Markdown
- **Export JSON** - Kopiert JSON-String in Zwischenablage
- **Clear** - Löscht aktuellen Inhalt
- **Undo** - Rückgängig
- **Redo** - Wiederholen

## Technologie
- WPF / .NET 8
- WebView2 für Editor und Preview
- **markdown-it 14.0.0** (via CDN)
- **markdown-it-mark** (via CDN)
- **markdown-it-attrs** (via CDN)
- System.Text.Json für Serialisierung

## Architektur

### Preview (markdown-it)
```javascript
- Client-side Markdown zu HTML
- markdown-it mit Plugins
- GitHub-ähnliches Styling
- Scroll-Position Preservation
- Identisch mit Edge-Browser
```

### Editor (JavaScript)
```javascript
- Textarea mit voller Kontrolle
- Undo/Redo Stack Management
- Tab-Handling
- Message Passing zu C#
- API für Textmanipulation
```

### C# Backend
```csharp
- WebView2 Management
- JSON Service
- Image Service
- Editor Service
- MarkdownService (HTML Wrapper)
- Async/Await Pattern
```

## Installation & Build

### Visual Studio
1. Öffnen Sie `MarkdownJsonEditor.sln`
2. **F5** oder **Ctrl+F5** zum Starten
3. **Build** -> **Build Solution** (Ctrl+Shift+B)

### Kommandozeile
```bash
# Build
dotnet build

# Run
dotnet run

# Publish (Single-File EXE)
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

## Verwendung

1. **JSON laden** - Lädt JSON mit komplexer Struktur
2. **Im Editor schreiben** - Mit voller Undo/Redo Unterstützung
3. **Tab & Enter** - Funktionieren perfekt!
4. **Formatierung** - Nutzen Sie die Toolbar
5. **Preview prüfen** - Identisch mit Edge-Browser
6. **Export** - Als JSON-String exportieren

## Test-Dateien

- **test-dsgvo.json** - DSGVO-Beispiel mit komplexer JSON-Struktur
- **test-enter-scroll.md** - Test für Editor-Funktionen

## JSON-Strukturen

### Einfache Struktur
```json
{
  "content": "# Titel\n\nText"
}
```

### Komplexe Struktur
```json
{
  "title": "DSGVO",
  "intro": "Datenschutzhinweise",
  "sections": [
    {
      "markdown": "## Überschrift\n\nText mit\nZeilenumbrüchen\n\tund Tabs"
    }
  ]
}
```

## Markdown-Syntax

### Standard (markdown-it)
```markdown
# Überschrift 1
## Überschrift 2
### Überschrift 3

**Fett** *Kursiv*

- Liste
1. Nummerierte Liste

> Zitat

[Link](https://url.de)
![Bild](image.jpg)

`code` oder ```code block```
```

### Erweitert (markdown-it-mark)
```markdown
==Gelb markiert==
```

### HTML (markdown-it erlaubt HTML)
```html
<span style='background-color: #90EE90'>Grün</span>
<span style='background-color: #FFB6C1'>Rot</span>
<span style='background-color: #ADD8E6'>Blau</span>
```

### Bilder mit Attributen (markdown-it-attrs)
```markdown
![Bild](img.jpg){ align=right width=40% }
```

## Systemanforderungen
- Windows 10/11
- .NET 8 Runtime
- WebView2 Runtime (meist vorinstalliert)
- Internetverbindung (für markdown-it CDN beim ersten Laden)

## Changelog

### Version 2.1 (Aktuell) - markdown-it Integration
- ✅ **NEU**: markdown-it für Preview statt Markdig
- ✅ **100% Edge-Browser kompatibel**
- ✅ markdown-it-mark Plugin für `==highlights==`
- ✅ markdown-it-attrs Plugin für Bild-Attribute
- ✅ CDN-basiert (keine lokalen Dependencies)
- ✅ Markdig entfernt (kleinere Binary)
- ✅ Client-side Rendering (schneller)

### Version 2.0 - JavaScript Editor
- ✅ WebView2-basierter Editor
- ✅ Undo/Redo mit Strg+Z / Strg+Y
- ✅ Perfekte Tab-Unterstützung
- ✅ Korrekte Zeilenumbrüche
- ✅ JavaScript API für alle Editier-Operationen
- ✅ Message Passing zwischen JS und C#

### Version 1.1 - Verbesserte TextBox
- Enter-Taste für neue Zeilen
- Tab-Unterstützung
- Scroll-Preservation im Preview

### Version 1.0 - Initial Release
- Basis Markdown Editor
- JSON Import/Export
- Live-Vorschau

## Vorteile von markdown-it

✅ **Standard-Konform** - CommonMark + GFM
✅ **Erweiterbar** - Viele Plugins verfügbar
✅ **Schnell** - Optimiert für Performance
✅ **Edge-Browser kompatibel** - Identisches Rendering
✅ **Aktiv gewartet** - Große Community
✅ **CDN-basiert** - Keine lokalen Dependencies

## markdown-it Plugins verwendet

| Plugin | Version | Zweck |
|--------|---------|-------|
| markdown-it | 14.0.0 | Basis Markdown-Parser |
| markdown-it-mark | 4.0.0 | `==Markierungen==` |
| markdown-it-attrs | 4.1.6 | `{align=right}` Attribute |

## Links

- [markdown-it GitHub](https://github.com/markdown-it/markdown-it)
- [markdown-it Demo](https://markdown-it.github.io/)
- [WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/)

## Mitwirkende
Entwickelt mit .NET 8, WPF, WebView2 und markdown-it

