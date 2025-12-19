import * as vscode from 'vscode';
import * as path from 'path';

export function activate(context: vscode.ExtensionContext) {
    console.log('Markdown Live Editor is now active!');

    let disposable = vscode.commands.registerCommand('markdownLiveEditor.editSelection', async () => {
        const editor = vscode.window.activeTextEditor;
        
        if (!editor) {
            vscode.window.showErrorMessage('No active editor found');
            return;
        }

        const selection = editor.selection;
        const selectedText = editor.document.getText(selection);

        if (!selectedText) {
            vscode.window.showErrorMessage('Please select some text first');
            return;
        }

        // Create and show webview panel
        const panel = vscode.window.createWebviewPanel(
            'markdownLiveEditor',
            'Markdown Live Editor',
            vscode.ViewColumn.Beside,
            {
                enableScripts: true,
                retainContextWhenHidden: true,
                localResourceRoots: [vscode.Uri.file(path.join(context.extensionPath, 'media'))]
            }
        );

        // Set initial HTML content
        panel.webview.html = getWebviewContent(selectedText);

        // Handle messages from webview
        panel.webview.onDidReceiveMessage(
            message => {
                switch (message.command) {
                    case 'update':
                        // Update the original document
                        const updatedText = message.text;
                        editor.edit(editBuilder => {
                            editBuilder.replace(selection, updatedText);
                        });
                        vscode.window.showInformationMessage('Markdown updated!');
                        return;
                    case 'close':
                        panel.dispose();
                        return;
                }
            },
            undefined,
            context.subscriptions
        );
    });

    context.subscriptions.push(disposable);
}

function getWebviewContent(initialText: string): string {
    const escapedText = initialText.replace(/\\/g, '\\\\').replace(/`/g, '\\`').replace(/\$/g, '\\$');
    
    return `<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Markdown Live Editor</title>
    <style>
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Helvetica, Arial, sans-serif;
            height: 100vh;
            display: flex;
            flex-direction: column;
            background: var(--vscode-editor-background);
            color: var(--vscode-editor-foreground);
        }

        .toolbar {
            padding: 10px;
            background: var(--vscode-editorGroupHeader-tabsBackground);
            border-bottom: 1px solid var(--vscode-panel-border);
            display: flex;
            gap: 10px;
            align-items: center;
        }

        button {
            padding: 6px 12px;
            background: var(--vscode-button-background);
            color: var(--vscode-button-foreground);
            border: none;
            border-radius: 3px;
            cursor: pointer;
            font-size: 13px;
        }

        button:hover {
            background: var(--vscode-button-hoverBackground);
        }

        .container {
            display: flex;
            flex: 1;
            overflow: hidden;
        }

        .editor-pane, .preview-pane {
            flex: 1;
            display: flex;
            flex-direction: column;
            overflow: hidden;
        }

        .pane-header {
            padding: 8px 12px;
            background: var(--vscode-editorGroupHeader-tabsBackground);
            border-bottom: 1px solid var(--vscode-panel-border);
            font-weight: 600;
            font-size: 12px;
        }

        textarea {
            flex: 1;
            padding: 16px;
            background: var(--vscode-editor-background);
            color: var(--vscode-editor-foreground);
            border: none;
            resize: none;
            font-family: 'Consolas', 'Monaco', monospace;
            font-size: 14px;
            line-height: 1.6;
            outline: none;
        }

        .preview {
            flex: 1;
            padding: 20px;
            overflow-y: auto;
            background: var(--vscode-editor-background);
            border-left: 1px solid var(--vscode-panel-border);
        }

        /* Markdown Preview Styles */
        .preview h1, .preview h2, .preview h3, .preview h4, .preview h5, .preview h6 {
            margin-top: 24px;
            margin-bottom: 16px;
            font-weight: 600;
            line-height: 1.25;
        }

        .preview h1 {
            font-size: 2em;
            border-bottom: 1px solid var(--vscode-panel-border);
            padding-bottom: 0.3em;
        }

        .preview h2 {
            font-size: 1.5em;
            border-bottom: 1px solid var(--vscode-panel-border);
            padding-bottom: 0.3em;
        }

        .preview h3 { font-size: 1.25em; }

        .preview p {
            margin-top: 0;
            margin-bottom: 16px;
        }

        .preview strong {
            font-weight: 600;
        }

        .preview em {
            font-style: italic;
        }

        .preview code {
            background: var(--vscode-textCodeBlock-background);
            padding: 0.2em 0.4em;
            border-radius: 3px;
            font-family: 'Consolas', 'Monaco', monospace;
            font-size: 85%;
        }

        .preview pre {
            background: var(--vscode-textCodeBlock-background);
            padding: 16px;
            overflow: auto;
            border-radius: 3px;
            margin-bottom: 16px;
        }

        .preview pre code {
            background: transparent;
            padding: 0;
        }

        .preview ul, .preview ol {
            padding-left: 2em;
            margin-bottom: 16px;
        }

        .preview li {
            margin-bottom: 0.25em;
        }

        .preview blockquote {
            padding: 0 1em;
            color: var(--vscode-descriptionForeground);
            border-left: 0.25em solid var(--vscode-panel-border);
            margin: 0 0 16px 0;
        }

        .preview a {
            color: var(--vscode-textLink-foreground);
            text-decoration: none;
        }

        .preview a:hover {
            text-decoration: underline;
        }

        .preview mark {
            background-color: #ffff00;
            color: #000;
            padding: 0.2em 0.4em;
            border-radius: 3px;
        }

        .status {
            padding: 4px 12px;
            background: var(--vscode-statusBar-background);
            color: var(--vscode-statusBar-foreground);
            font-size: 12px;
            text-align: center;
        }
    </style>
</head>
<body>
    <div class="toolbar">
        <button onclick="applyFormat('**', '**')"><b>B</b></button>
        <button onclick="applyFormat('*', '*')"><i>I</i></button>
        <button onclick="insertAtLineStart('# ')">H1</button>
        <button onclick="insertAtLineStart('## ')">H2</button>
        <button onclick="insertAtLineStart('### ')">H3</button>
        <button onclick="insertAtLineStart('- ')">• List</button>
        <button onclick="applyFormat('==', '==')">Yellow</button>
        <button onclick="applyFormat('\`', '\`')">Code</button>
        <span style="flex: 1"></span>
        <button onclick="saveChanges()" style="background: var(--vscode-button-background);">?? Save</button>
        <button onclick="closeEditor()">? Close</button>
    </div>

    <div class="container">
        <div class="editor-pane">
            <div class="pane-header">Markdown Editor</div>
            <textarea id="editor" spellcheck="false">\`${escapedText}\`</textarea>
        </div>
        <div class="preview-pane">
            <div class="pane-header">Live Preview</div>
            <div id="preview" class="preview"></div>
        </div>
    </div>

    <div class="status" id="status">Ready • Auto-saving every 2 seconds</div>

    <script src="https://cdn.jsdelivr.net/npm/markdown-it@14.0.0/dist/markdown-it.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/markdown-it-mark@3.0.1/dist/markdown-it-mark.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/markdown-it-attrs@4.1.6/dist/markdown-it-attrs.browser.js"></script>

    <script>
        const vscode = acquireVsCodeApi();
        const editor = document.getElementById('editor');
        const preview = document.getElementById('preview');
        const status = document.getElementById('status');

        // Initialize markdown-it
        const md = window.markdownit({
            html: true,
            linkify: true,
            typographer: true,
            breaks: false
        })
        .use(window.markdownItMark)
        .use(window.markdownItAttrs);

        let autoSaveTimer;
        let lastSavedContent = editor.value;

        // Update preview
        function updatePreview() {
            const text = editor.value;
            try {
                const html = md.render(text);
                preview.innerHTML = html;
                status.textContent = \`Ready • \${text.length} characters • \${text.split('\\n').length} lines\`;
            } catch (e) {
                preview.innerHTML = \`<p style="color: red;">Render Error: \${e.message}</p>\`;
            }
        }

        // Auto-save
        function autoSave() {
            const currentContent = editor.value;
            if (currentContent !== lastSavedContent) {
                vscode.postMessage({
                    command: 'update',
                    text: currentContent
                });
                lastSavedContent = currentContent;
                status.textContent = 'Saved ?';
                setTimeout(() => updatePreview(), 100);
            }
        }

        // Event listeners
        editor.addEventListener('input', () => {
            updatePreview();
            clearTimeout(autoSaveTimer);
            autoSaveTimer = setTimeout(autoSave, 2000);
        });

        editor.addEventListener('keydown', (e) => {
            // Tab key
            if (e.key === 'Tab') {
                e.preventDefault();
                const start = editor.selectionStart;
                const end = editor.selectionEnd;
                editor.value = editor.value.substring(0, start) + '    ' + editor.value.substring(end);
                editor.selectionStart = editor.selectionEnd = start + 4;
            }
            
            // Enter key - add 2 spaces for hard break
            if (e.key === 'Enter') {
                e.preventDefault();
                const start = editor.selectionStart;
                const end = editor.selectionEnd;
                editor.value = editor.value.substring(0, start) + '  \\n' + editor.value.substring(end);
                editor.selectionStart = editor.selectionEnd = start + 3;
            }
        });

        // Formatting functions
        function applyFormat(before, after) {
            const start = editor.selectionStart;
            const end = editor.selectionEnd;
            const selectedText = editor.value.substring(start, end);
            
            if (selectedText) {
                editor.value = editor.value.substring(0, start) + before + selectedText + after + editor.value.substring(end);
                editor.selectionStart = start + before.length;
                editor.selectionEnd = start + before.length + selectedText.length;
            } else {
                editor.value = editor.value.substring(0, start) + before + 'text' + after + editor.value.substring(end);
                editor.selectionStart = start + before.length;
                editor.selectionEnd = start + before.length + 4;
            }
            
            editor.focus();
            updatePreview();
        }

        function insertAtLineStart(prefix) {
            const start = editor.selectionStart;
            const text = editor.value;
            const lineStart = text.lastIndexOf('\\n', start - 1) + 1;
            
            editor.value = text.substring(0, lineStart) + prefix + text.substring(lineStart);
            editor.selectionStart = editor.selectionEnd = start + prefix.length;
            editor.focus();
            updatePreview();
        }

        function saveChanges() {
            vscode.postMessage({
                command: 'update',
                text: editor.value
            });
            lastSavedContent = editor.value;
            status.textContent = 'Saved ?';
        }

        function closeEditor() {
            // Auto-save before closing
            if (editor.value !== lastSavedContent) {
                saveChanges();
            }
            vscode.postMessage({ command: 'close' });
        }

        // Initial preview
        updatePreview();
        editor.focus();
    </script>
</body>
</html>`;
}

export function deactivate() {}
