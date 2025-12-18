# GitHub Actions & Release Anleitung

## ?? Automatische Releases

Dieses Repository verwendet GitHub Actions für automatische Builds und Releases.

## ?? Release erstellen

### Schritt 1: Code committen und pushen
```bash
git add .
git commit -m "Neue Features hinzugefügt"
git push origin main
```

### Schritt 2: Version Tag erstellen
```bash
# Semantic Versioning: MAJOR.MINOR.PATCH
git tag v2.1.0
git push origin v2.1.0
```

**Version-Schema:**
- **MAJOR** (2.x.x): Breaking Changes
- **MINOR** (x.1.x): Neue Features
- **PATCH** (x.x.0): Bugfixes

### Schritt 3: GitHub Action läuft automatisch
Die Action wird automatisch gestartet und:
1. ? Kompiliert die Anwendung
2. ? Erstellt Single-File EXE
3. ? Packt alles in ZIP
4. ? Erstellt GitHub Release
5. ? Lädt ZIP hoch

## ?? Beispiel-Workflow

```bash
# Version 2.1.0 - Neue Features
git add .
git commit -m "feat: Markdown-it Integration, Undo/Redo Support"
git tag v2.1.0
git push origin main
git push origin v2.1.0

# Version 2.1.1 - Bugfix
git add .
git commit -m "fix: Preview scroll position"
git tag v2.1.1
git push origin main
git push origin v2.1.1

# Version 3.0.0 - Breaking Changes
git add .
git commit -m "BREAKING: Neue Editor-API"
git tag v3.0.0
git push origin main
git push origin v3.0.0
```

## ?? Build Status überprüfen

1. Gehe zu **Actions** Tab im Repository
2. Siehe **Release Build** Workflow
3. Grünes Häkchen = Erfolg ?
4. Rotes X = Fehler ?

## ?? Release herunterladen

Nach erfolgreichem Build:
1. Gehe zu **Releases** Tab
2. Klicke auf neueste Version
3. Download: `MarkdownJsonEditor-v2.1.0-win-x64.zip`

## ??? Lokaler Build

### Single-File EXE erstellen
```bash
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

### Ausgabe
```
bin/Release/net8.0-windows/win-x64/publish/MarkdownJsonEditor.exe
```

## ?? Build-Konfiguration

Die Action verwendet folgende Parameter:

```yaml
--runtime win-x64                              # Windows 64-bit
--self-contained true                          # Enthält .NET Runtime
--output ./publish                             # Ausgabe-Ordner
-p:PublishSingleFile=true                      # Single-File EXE
-p:IncludeNativeLibrariesForSelfExtract=true   # Native DLLs einbetten
-p:EnableCompressionInSingleFile=true          # Komprimierung
-p:DebugType=None                              # Keine Debug-Symbole
-p:DebugSymbols=false                          # Keine PDB-Dateien
```

## ??? Tag-Konventionen

### Gültige Tags
- ? `v1.0.0`
- ? `v2.3.1`
- ? `v10.0.0`

### Ungültige Tags
- ? `1.0.0` (fehlendes 'v')
- ? `version-1.0` (falsches Format)
- ? `v1.0` (fehlendes PATCH)

## ?? Changelog

Die Action erstellt automatisch einen Changelog aus den Commit-Messages zwischen den Tags.

**Commit-Message Konventionen:**
```
feat: Neue Features
fix: Bugfixes
docs: Dokumentation
style: Code-Formatierung
refactor: Code-Refactoring
test: Tests
chore: Maintenance
```

## ?? Secrets

Die Action benötigt:
- `GITHUB_TOKEN` (automatisch vorhanden)

Keine zusätzlichen Secrets erforderlich!

## ?? Troubleshooting

### Build schlägt fehl
1. Überprüfe `.csproj` Datei
2. Stelle sicher, dass lokal `dotnet build` funktioniert
3. Überprüfe Logs in GitHub Actions

### Tag funktioniert nicht
```bash
# Tag löschen (lokal)
git tag -d v2.1.0

# Tag löschen (remote)
git push origin :refs/tags/v2.1.0

# Neuen Tag erstellen
git tag v2.1.0
git push origin v2.1.0
```

### Release nicht erstellt
1. Überprüfe ob Tag mit `v` beginnt
2. Überprüfe GitHub Actions Logs
3. Stelle sicher, dass `GITHUB_TOKEN` Permissions hat

## ?? Weitere Informationen

- [Semantic Versioning](https://semver.org/)
- [GitHub Actions Docs](https://docs.github.com/en/actions)
- [.NET Publish Docs](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)

## ?? Schnell-Referenz

```bash
# Release erstellen
git tag v2.1.0 && git push origin v2.1.0

# Tag löschen
git push origin :refs/tags/v2.1.0

# Lokaler Build
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# Build-Status ansehen
# ? GitHub ? Actions Tab
```

## ? Features der Build-Pipeline

- ? Automatischer Build bei Version-Tags
- ? Single-File EXE (keine DLLs)
- ? Self-Contained (.NET Runtime eingebettet)
- ? Komprimiert (~50% kleiner)
- ? ZIP-Archive
- ? Automatischer Changelog
- ? GitHub Release mit Beschreibung
- ? Artifact Upload
- ? Version in EXE-Metadaten

## ?? Versions-Historie (Beispiel)

- **v2.1.0** - markdown-it Integration, Undo/Redo
- **v2.0.0** - JavaScript-basierter Editor
- **v1.1.0** - Enter/Tab Unterstützung
- **v1.0.0** - Initial Release
