namespace MarkdownJsonEditor.Services
{
    public class EditorService
    {
        public string GetEditorHtml()
        {
            return @"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        html, body {
            height: 100%;
            overflow: hidden;
        }
        
        #editor {
            width: 100%;
            height: 100%;
            padding: 10px;
            font-family: 'Consolas', 'Monaco', monospace;
            font-size: 14px;
            line-height: 1.6;
            border: none;
            outline: none;
            resize: none;
            background: #fff;
            color: #333;
            white-space: pre;
            overflow-wrap: normal;
            overflow-x: auto;
            overflow-y: auto;
            tab-size: 4;
        }
        
        #editor:focus {
            outline: none;
        }
    </style>
</head>
<body>
    <textarea id='editor' spellcheck='false'></textarea>
    <script>
        const editor = document.getElementById('editor');
        
        // Undo/Redo Stack
        let undoStack = [];
        let redoStack = [];
        let lastValue = '';
        let isUndoRedo = false;
        
        function saveState() {
            if (isUndoRedo) return;
            
            const currentValue = editor.value;
            if (currentValue !== lastValue) {
                undoStack.push(lastValue);
                if (undoStack.length > 100) {
                    undoStack.shift();
                }
                redoStack = [];
                lastValue = currentValue;
            }
        }
        
        function undo() {
            if (undoStack.length === 0) return false;
            
            isUndoRedo = true;
            redoStack.push(editor.value);
            editor.value = undoStack.pop();
            lastValue = editor.value;
            isUndoRedo = false;
            
            notifyChange();
            return true;
        }
        
        function redo() {
            if (redoStack.length === 0) return false;
            
            isUndoRedo = true;
            undoStack.push(editor.value);
            editor.value = redoStack.pop();
            lastValue = editor.value;
            isUndoRedo = false;
            
            notifyChange();
            return true;
        }
        
        function notifyChange() {
            if (window.chrome && window.chrome.webview) {
                window.chrome.webview.postMessage({
                    type: 'textChanged',
                    content: editor.value
                });
            }
        }
        
        editor.addEventListener('input', function() {
            saveState();
            notifyChange();
        });
        
        editor.addEventListener('keydown', function(e) {
            // Tab-Taste
            if (e.key === 'Tab') {
                e.preventDefault();
                const start = editor.selectionStart;
                const end = editor.selectionEnd;
                editor.value = editor.value.substring(0, start) + '\t' + editor.value.substring(end);
                editor.selectionStart = editor.selectionEnd = start + 1;
                saveState();
                notifyChange();
            }
            
            // Enter-Taste: Füge 2 Leerzeichen + Newline ein
            if (e.key === 'Enter') {
                e.preventDefault();
                const start = editor.selectionStart;
                const end = editor.selectionEnd;
                
                // Füge 2 Leerzeichen + Zeilenumbruch ein (Markdown hard line break)
                const lineBreak = '  \n';
                editor.value = editor.value.substring(0, start) + lineBreak + editor.value.substring(end);
                editor.selectionStart = editor.selectionEnd = start + lineBreak.length;
                
                saveState();
                notifyChange();
            }
            
            // Ctrl+Z (Undo)
            if (e.ctrlKey && e.key === 'z' && !e.shiftKey) {
                e.preventDefault();
                undo();
            }
            
            // Ctrl+Y oder Ctrl+Shift+Z (Redo)
            if ((e.ctrlKey && e.key === 'y') || (e.ctrlKey && e.shiftKey && e.key === 'z')) {
                e.preventDefault();
                redo();
            }
        });
        
        lastValue = editor.value;
        
        // API für C#
        window.getText = function() {
            return editor.value;
        };
        
        window.setText = function(text) {
            isUndoRedo = true;
            editor.value = text;
            lastValue = text;
            undoStack = [];
            redoStack = [];
            isUndoRedo = false;
        };
        
        window.insertText = function(text) {
            const start = editor.selectionStart;
            const end = editor.selectionEnd;
            editor.value = editor.value.substring(0, start) + text + editor.value.substring(end);
            editor.selectionStart = editor.selectionEnd = start + text.length;
            editor.focus();
            saveState();
            notifyChange();
        };
        
        window.wrapSelection = function(before, after) {
            const start = editor.selectionStart;
            const end = editor.selectionEnd;
            const selectedText = editor.value.substring(start, end);
            
            if (selectedText) {
                editor.value = editor.value.substring(0, start) + before + selectedText + after + editor.value.substring(end);
                editor.selectionStart = start + before.length;
                editor.selectionEnd = start + before.length + selectedText.length;
            } else {
                editor.value = editor.value.substring(0, start) + before + 'Text' + after + editor.value.substring(end);
                editor.selectionStart = start + before.length;
                editor.selectionEnd = start + before.length + 4;
            }
            
            editor.focus();
            saveState();
            notifyChange();
        };
        
        window.insertAtLineStart = function(prefix) {
            const start = editor.selectionStart;
            const text = editor.value;
            const lineStart = text.lastIndexOf('\n', start - 1) + 1;
            
            editor.value = text.substring(0, lineStart) + prefix + text.substring(lineStart);
            editor.selectionStart = editor.selectionEnd = start + prefix.length;
            editor.focus();
            saveState();
            notifyChange();
        };
        
        window.performUndo = undo;
        window.performRedo = redo;
        
        console.log('Editor initialized with Markdown line break support (2 spaces + newline)');
    </script>
</body>
</html>";
        }
    }
}
