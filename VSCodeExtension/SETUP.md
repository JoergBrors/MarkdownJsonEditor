# VS Code Extension Setup & Usage Guide

## ?? Was ist das?

Ein **Visual Studio Code Extension**, das markierten Markdown-Text in einem Split-View-Editor mit Live-Preview öffnet - perfekt integriert in Ihre Solution!

## ?? Projekt-Struktur

```
MarkdownJsonEditor/
??? MarkdownJsonEditor.csproj         # WPF Application
??? VSCodeExtension/                   # VS Code Extension (neu)
?   ??? package.json                   # Extension Manifest
?   ??? tsconfig.json                  # TypeScript Config
?   ??? src/
?   ?   ??? extension.ts               # Extension Code
?   ??? README.md                      # Extension Docs
?   ??? .vscodeignore                  # Publishing excludes
??? MarkdownJsonEditor.code-workspace  # VS Code Workspace (neu)
??? .vscode/
    ??? launch.json                    # Debug Configs (neu)
    ??? tasks.json                     # Build Tasks (neu)
```

## ?? Installation & Setup

### Schritt 1: Dependencies installieren

```bash
cd VSCodeExtension
npm install
```

Dies installiert:
- TypeScript Compiler
- VS Code Extension API
- markdown-it und Plugins
- ESLint

### Schritt 2: Kompilieren

```bash
npm run compile
```

Oder für Watch-Mode:
```bash
npm run watch
```

### Schritt 3: Extension testen

1. **In VS Code**: Öffne die Solution
2. **F5 drücken** ? Startet Extension Development Host
3. **Neues Fenster öffnet sich** mit der Extension geladen
4. **Markdown-Text markieren**
5. **Rechtsklick** ? "Edit Markdown with Live Preview"
6. **Oder**: `Ctrl+Shift+M` drücken

## ?? Features

### 1. **Live Preview**
- Markdown wird **sofort** gerendert
- Gleiche Engine wie WPF-App (markdown-it)
- Split-View: Editor links, Preview rechts

### 2. **Auto-Save**
- Speichert automatisch alle 2 Sekunden
- Zeigt Status in der Statusbar
- Manuelles Speichern mit "Save" Button

### 3. **Formatting Toolbar**
```
[B] [I] [H1] [H2] [H3] [• List] [Yellow] [Code] [?? Save] [? Close]
```

### 4. **Keyboard Shortcuts**

| Shortcut | Aktion |
|----------|--------|
| `Ctrl+Shift+M` | Extension öffnen |
| `Tab` | 4 Spaces einfügen |
| `Enter` | 2 Spaces + Newline (hard break) |
| `Ctrl+S` | Manuell speichern |

### 5. **Selection Editing**
- Markiere Text in **jedem** File
- Extension öffnet nur den markierten Teil
- Beim Speichern wird Original aktualisiert

## ?? Verwendung

### Szenario 1: JSON mit Markdown bearbeiten

```json
{
  "description": "## Alte Version\n\nMit **altem** Text"
}
```

1. Markiere den Wert von `"description"`
2. `Ctrl+Shift+M` drücken
3. Im Editor erscheint:
```markdown
## Alte Version

Mit **altem** Text
```
4. Bearbeiten mit Live-Preview
5. Speichern ? JSON wird aktualisiert

### Szenario 2: Markdown-Datei bearbeiten

1. Öffne `README.md`
2. Markiere einen Abschnitt
3. `Ctrl+Shift+M`
4. Bearbeiten mit Toolbar
5. Auto-Save aktualisiert Original

### Szenario 3: Code-Kommentare

```csharp
// ## TODO
// 
// - [ ] Feature 1
// - [ ] Feature 2
```

1. Markiere die Kommentare
2. Extension öffnen
3. Als Markdown bearbeiten
4. Zurück als Kommentare

## ?? Development

### VS Code Workspace öffnen

```bash
code MarkdownJsonEditor.code-workspace
```

Dies öffnet **beide** Projekte:
- MarkdownJsonEditor (WPF)
- VSCodeExtension

### Extension entwickeln

1. **Watch-Mode starten**:
```bash
cd VSCodeExtension
npm run watch
```

2. **F5 drücken** ? Extension Development Host

3. **Änderungen machen** in `src/extension.ts`

4. **Reload** im Dev Host: `Ctrl+R`

### Debuggen

- **Breakpoints** setzen in `extension.ts`
- **F5** drücken
- **Extension verwenden** ? Breakpoint wird getroffen
- **Debug Console** zeigt Ausgaben

## ?? Publishing

### Lokal installieren

```bash
cd VSCodeExtension
npm install -g vsce
vsce package
code --install-extension markdown-live-editor-1.0.0.vsix
```

### Im VS Code Marketplace veröffentlichen

1. **Account erstellen**: https://marketplace.visualstudio.com/
2. **Personal Access Token** erstellen
3. **Publisher registrieren**
4. **Veröffentlichen**:
```bash
vsce publish
```

## ?? Integration mit WPF-App

Beide Anwendungen teilen:
- ? **markdown-it 14.0.0** - Gleicher Renderer
- ? **markdown-it-mark** - Gelbe Markierungen
- ? **markdown-it-attrs** - Bild-Attribute
- ? **Gleiche CSS-Styles** - Identisches Look & Feel
- ? **Gleiche Shortcuts** - Tab, Enter, etc.
- ? **Hard Line Breaks** - 2 Spaces + Newline

## ?? Use Cases

### 1. **Schnelle Markdown-Edits in JSON-Files**
- Öffne große JSON mit Markdown
- Markiere nur relevanten Teil
- Bearbeite mit Preview
- Speichere direkt zurück

### 2. **Markdown-Snippets testen**
- Erstelle Markdown in beliebigem File
- Teste Rendering sofort
- Kopiere zu WPF-App

### 3. **Dokumentation schreiben**
- Schreibe README mit Live-Preview
- Sehe sofort wie es aussieht
- Keine externen Tools nötig

### 4. **Multi-Language Content**
```json
{
  "en": "Text EN",
  "de": "Text DE"
}
```
- Markiere einzelne Sprache
- Bearbeite isoliert
- Keine Verwechslungen

## ?? Vergleich WPF App vs. VS Code Extension

| Feature | WPF App | VS Code Extension |
|---------|---------|-------------------|
| **Platform** | Windows only | Cross-platform |
| **UI** | Native WPF | Web-based |
| **JSON Import** | ? Full featured | ? Selection only |
| **Section Navigator** | ? | ? |
| **Live Preview** | ? | ? |
| **Auto-Save** | Manual | ? Automatic |
| **Toolbar** | ? Full | ? Basic |
| **Integration** | Standalone | ? VS Code |

## ?? Weitere Ressourcen

- **VS Code Extension API**: https://code.visualstudio.com/api
- **Extension Guides**: https://code.visualstudio.com/api/get-started/your-first-extension
- **Publishing**: https://code.visualstudio.com/api/working-with-extensions/publishing-extension
- **markdown-it**: https://markdown-it.github.io/

## ?? Troubleshooting

### Extension lädt nicht

```bash
cd VSCodeExtension
npm install
npm run compile
```

### TypeScript Errors

```bash
npm install --save-dev @types/vscode@latest
```

### Extension Development Host startet nicht

- Stelle sicher, dass VS Code >= 1.80.0
- Überprüfe `package.json` engines
- Öffne Output ? Extension Host

## ?? Fertig!

Die VS Code Extension ist jetzt teil Ihrer Solution und kann verwendet werden um markierten Markdown-Text mit Live-Preview zu bearbeiten!

**Nächste Schritte:**
1. `cd VSCodeExtension && npm install`
2. `npm run compile`
3. **F5** drücken in VS Code
4. Markdown markieren
5. `Ctrl+Shift+M` drücken
6. Loslegen! ??
