# Markdown Live Editor - VS Code Extension

A powerful Visual Studio Code extension for editing Markdown with live preview.

## Features

- ? **Live Preview** - See your Markdown rendered in real-time
- ? **Split View** - Editor and preview side-by-side
- ? **Auto-Save** - Automatic saving every 2 seconds
- ? **Formatting Toolbar** - Quick access to common Markdown formatting
- ? **Selection Editing** - Edit only selected text from your document
- ? **markdown-it** - Same rendering engine as the main application
- ? **Keyboard Shortcuts** - `Ctrl+Shift+M` (Windows/Linux) or `Cmd+Shift+M` (Mac)

## Usage

### Method 1: Context Menu
1. Select some Markdown text in your editor
2. Right-click and select "Edit Markdown with Live Preview"
3. Edit in the split view
4. Click "Save" or wait for auto-save

### Method 2: Keyboard Shortcut
1. Select text
2. Press `Ctrl+Shift+M` (or `Cmd+Shift+M` on Mac)

### Method 3: Command Palette
1. Select text
2. Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac)
3. Type "Edit Markdown with Live Preview"

## Formatting Toolbar

| Button | Action |
|--------|--------|
| **B** | Bold text (`**text**`) |
| **I** | Italic text (`*text*`) |
| H1 | Heading 1 (`# text`) |
| H2 | Heading 2 (`## text`) |
| H3 | Heading 3 (`### text`) |
| • List | Bullet list (`- item`) |
| Yellow | Yellow highlight (`==text==`) |
| Code | Inline code (`` `code` ``) |

## Keyboard Shortcuts in Editor

- **Tab** - Insert 4 spaces
- **Enter** - Insert line break with 2 spaces (Markdown hard break)
- **Ctrl+S** - Manual save

## Installation

### From Source

1. Clone the repository
2. Navigate to `VSCodeExtension` folder
3. Run `npm install`
4. Run `npm run compile`
5. Press F5 to open Extension Development Host

### From VSIX

1. Build: `vsce package`
2. Install: `code --install-extension markdown-live-editor-1.0.0.vsix`

## Development

### Setup
```bash
cd VSCodeExtension
npm install
```

### Compile
```bash
npm run compile
```

### Watch Mode
```bash
npm run watch
```

### Debug
Press F5 in VS Code to open Extension Development Host

## Building

### Package
```bash
npm install -g vsce
vsce package
```

This creates `markdown-live-editor-1.0.0.vsix`

### Publish
```bash
vsce publish
```

## Integration with Main Application

This extension uses the same rendering engine (markdown-it) and features as the main MarkdownJsonEditor WPF application:

- ? markdown-it 14.0.0
- ? markdown-it-mark (yellow highlighting)
- ? markdown-it-attrs (image attributes)
- ? Same CSS styling
- ? Same hard line break behavior (2 spaces + newline)

## Requirements

- Visual Studio Code 1.80.0 or higher
- Internet connection (for CDN-loaded markdown-it libraries)

## Extension Settings

Currently no custom settings. Future versions may include:

- Auto-save interval
- Preview theme
- Custom CSS

## Known Issues

- None currently

## Release Notes

### 1.0.0

- Initial release
- Live preview with markdown-it
- Auto-save functionality
- Formatting toolbar
- Keyboard shortcuts

## Contributing

This extension is part of the MarkdownJsonEditor solution.

## License

MIT License
