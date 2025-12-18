# Markdown JSON WYSIWYG Editor

Ein professioneller Windows Desktop Editor für Markdown mit JSON-Integration und **markdown-it** Rendering.

## 🎉 Version 1.0.0 - Production Ready!

### ✨ Hauptmerkmale
- **markdown-it JavaScript-basierte Preview** - Identisch mit Edge Browser
- **JavaScript-basierter Editor** (WebView2) 
- **Vollständiges Undo/Redo** mit Strg+Z und Strg+Y
- **Automatische Zeilenumbrüche** - Enter fügt 2 Leerzeichen + Newline ein
- **Perfekte Tab-Unterstützung** - Tabs werden korrekt eingefügt (4 Spaces)
- **Live-Vorschau** mit markdown-it von CDN (300ms Polling)
- **Scroll-Preservation** - Preview behält Position beim Editieren
- **JSON Import/Export** mit korrektem Escaping
- **Signalfarben** - Farbige Markierungen (Gelb, Grün, Rot, Blau)
- **Single-File Distribution** - Nur eine EXE, keine Installation nötig

## 📦 Download

**[⬇️ MarkdownEditor.zip herunterladen](https://github.com/JoergBrors/MarkdownJsonEditor/releases/latest/download/MarkdownEditor.zip)**

Oder gehe zu: [Releases](https://github.com/JoergBrors/MarkdownJsonEditor/releases)

## 🚀 Schnellstart

1. **MarkdownEditor.zip** herunterladen
2. ZIP entpacken
3. **MarkdownEditor.exe** ausführen
4. Sofort loslegen! ✨

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
    breaks: false        // Standard Markdown (2 Spaces = hard break)
}
```

### Editor-Features
- **WebView2-basierter Editor** mit JavaScript
- **Monospace Font** (Consolas)
- **Tab-Größe**: 4 Zeichen
- **Undo/Redo Stack**: bis zu 100 Schritte
- **Automatisches Encoding/Decoding**
- **Polling-basiertes Update** (300ms) für zuverlässige Preview

### Tastenkombinationen im Editor
- **Tab** - Tab-Zeichen einfügen (4 Spaces) ✅
- **Enter** - Neue Zeile mit 2 Leerzeichen + Newline ✅
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
- **Export JSON** - Kopiert JSON-String in Zwischenablage (mit korrektem \n Escaping)
- **Clear** - Löscht aktuellen Inhalt
- **Undo** - Rückgängig
- **Redo** - Wiederholen

## Technologie
- **WPF** / .NET 8
- **WebView2** für Editor und Preview
- **markdown-it 14.0.0** (via CDN)
- **markdown-it-mark** (via CDN)
- **markdown-it-attrs** (via CDN)
- **System.Text.Json** für Serialisierung

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
- Tab-Handling (4 Spaces)
- Enter-Handling (2 Spaces + \n)
- Message Passing zu C# (via Polling)
- API für Textmanipulation
```

### C# Backend
```csharp
- WebView2 Management
- DispatcherTimer für Polling (300ms)
- JSON Service mit korrektem Escaping
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
6. **Export** - Als JSON-String exportieren (mit \n Escaping)

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

### Hard Line Breaks
```markdown
Zeile 1  
Zeile 2  
Zeile 3

(2 Leerzeichen am Zeilenende + Enter)
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
- **Windows 10/11** (64-bit)
- **.NET 8 Runtime** (eingebettet in Single-File EXE)
- **WebView2 Runtime** (meist vorinstalliert)
- **Internetverbindung** (für markdown-it CDN beim ersten Laden)

## Entwicklung

### GitHub Actions
Das Projekt nutzt GitHub Actions für automatische Releases:

```bash
# Release erstellen
git tag v1.0.0
git push origin v1.0.0

# Action erstellt automatisch:
# - Single-File EXE
# - MarkdownEditor.zip
# - GitHub Release
```

Siehe: [RELEASE.md](RELEASE.md) für Details

### Branches
- **master** - Stable Production Branch
- **develop** - Development Branch (optional)

## Changelog

### Version 1.0.0 (2024-12-XX) - Production Ready! 🎉
- ✅ **Stabile Production Version**
- ✅ markdown-it für Preview statt Markdig
- ✅ JavaScript-basierter Editor mit WebView2
- ✅ Undo/Redo mit Strg+Z / Strg+Y
- ✅ Automatische Zeilenumbrüche (2 Spaces + \n)
- ✅ Perfekte Tab-Unterstützung (4 Spaces)
- ✅ Polling-basiertes Update (300ms) statt Events
- ✅ JSON Export mit korrektem \n Escaping
- ✅ GitHub Actions für automatische Releases
- ✅ Single-File EXE Distribution
- ✅ CDN-basiert (keine lokalen Dependencies)
- ✅ 100% Edge-Browser kompatibel

### Version 0.0.2 - JavaScript Editor
- ✅ WebView2-basierter Editor
- ✅ Undo/Redo mit Stack
- ✅ Tab & Enter Unterstützung
- ✅ Message Passing zwischen JS und C#

### Version 0.0.1 - Initial Release
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
| markdown-it-mark | 3.0.1 | `==Markierungen==` |
| markdown-it-attrs | 4.1.6 | `{align=right}` Attribute |

## CI/CD Pipeline

### Automatische Builds
- Bei jedem Push auf master/develop
- Pull Requests werden getestet
- Build-Status sichtbar in Actions Tab

### Automatische Releases
- Bei Version-Tags (v1.0.0, v2.0.0, etc.)
- Erstellt Single-File EXE
- ZIP-Archive
- GitHub Release mit Changelog
- 90 Tage Artifact-Retention

## Links

- **Repository**: [GitHub](https://github.com/JoergBrors/MarkdownJsonEditor)
- **Releases**: [Download](https://github.com/JoergBrors/MarkdownJsonEditor/releases)
- **Issues**: [Bug Reports](https://github.com/JoergBrors/MarkdownJsonEditor/issues)
- [markdown-it GitHub](https://github.com/markdown-it/markdown-it)
- [markdown-it Demo](https://markdown-it.github.io/)
- [WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/)

## Lizenz

MIT License - Siehe [LICENSE](LICENSE) für Details

## Mitwirkende

Entwickelt mit .NET 8, WPF, WebView2 und markdown-it

## Support

Bei Fragen oder Problemen:
1. [Issue erstellen](https://github.com/JoergBrors/MarkdownJsonEditor/issues)
2. [Releases prüfen](https://github.com/JoergBrors/MarkdownJsonEditor/releases)
3. [README.md lesen](README.md)
4. [RELEASE.md konsultieren](RELEASE.md)

---

**Version**: 1.0.0  
**Status**: ✅ Production Ready  
**Letzte Aktualisierung**: Dezember 2024

