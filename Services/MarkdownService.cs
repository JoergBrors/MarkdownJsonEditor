namespace MarkdownJsonEditor.Services
{
    public class MarkdownService
    {
        public string GetInitialPreviewHtml()
        {
            var css = GetCss();
            var script = GetMarkdownItScript();
            
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <style>{css}</style>
</head>
<body>
    <div id='preview'>Lade...</div>
    <script src='https://cdn.jsdelivr.net/npm/markdown-it@14.0.0/dist/markdown-it.min.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/markdown-it-mark@3.0.1/dist/markdown-it-mark.min.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/markdown-it-attrs@4.1.6/dist/markdown-it-attrs.browser.js'></script>
    <script>
        {script}
    </script>
</body>
</html>";
        }

        private string GetMarkdownItScript()
        {
            return @"
                console.log('Initializing markdown-it preview...');
                
                let md = null;
                let isInitialized = false;
                
                window.addEventListener('load', function() {
                    try {
                        if (typeof window.markdownit === 'undefined') {
                            throw new Error('markdown-it nicht geladen');
                        }
                        
                        console.log('markdown-it loaded');
                        
                        md = window.markdownit({
                            html: true,
                            linkify: true,
                            typographer: true,
                            breaks: false  // WICHTIG: false für Standard Markdown (2 Spaces = hard break)
                        });
                        
                        if (typeof window.markdownItMark !== 'undefined') {
                            md.use(window.markdownItMark);
                            console.log('markdown-it-mark plugin enabled');
                        }
                        
                        if (typeof window.markdownItAttrs !== 'undefined') {
                            md.use(window.markdownItAttrs);
                            console.log('markdown-it-attrs plugin enabled');
                        }
                        
                        isInitialized = true;
                        console.log('markdown-it initialization complete - Standard mode (2 spaces = hard break)');
                        
                        document.getElementById('preview').innerHTML = '<p style=""color:#999;font-style:italic"">Bereit für Inhalt...</p>';
                        
                    } catch(e) {
                        console.error('Initialization Error:', e);
                        document.getElementById('preview').innerHTML = '<h3>Initialization Error:</h3><p>' + e.message + '</p>';
                    }
                });
                
                window.updateMarkdown = function(text) {
                    console.log('updateMarkdown called, text length:', text ? text.length : 0);
                    
                    if (!isInitialized) {
                        console.warn('markdown-it not initialized yet');
                        setTimeout(function() { window.updateMarkdown(text); }, 100);
                        return false;
                    }
                    
                    const preview = document.getElementById('preview');
                    if (!preview) {
                        console.error('Preview element not found');
                        return false;
                    }
                    
                    const oldScrollPos = window.pageYOffset;
                    
                    try {
                        if (!text || text.length === 0) {
                            preview.innerHTML = '<p style=""color:#999;font-style:italic"">Beginnen Sie mit dem Schreiben...</p>';
                        } else {
                            const html = md.render(text);
                            preview.innerHTML = html;
                            
                            // Debug: Zeige ob hard breaks erkannt wurden
                            const brCount = (html.match(/<br>/g) || []).length;
                            console.log('Rendered HTML contains', brCount, 'hard line breaks (<br>)');
                        }
                        
                        console.log('Markdown rendered successfully');
                        
                        setTimeout(() => {
                            if (oldScrollPos > 0) {
                                window.scrollTo(0, oldScrollPos);
                            }
                        }, 10);
                        
                        return true;
                    } catch(e) {
                        console.error('Render error:', e);
                        preview.innerHTML = '<h3>Render Error:</h3><p>' + e.message + '</p><pre>' + e.stack + '</pre>';
                        return false;
                    }
                };
                
                window.testUpdate = function() {
                    return window.updateMarkdown('Zeile 1  \\nZeile 2  \\nZeile 3');
                };
                
                window.getStatus = function() {
                    return {
                        initialized: isInitialized,
                        markdownit: typeof window.markdownit !== 'undefined',
                        mark: typeof window.markdownItMark !== 'undefined',
                        attrs: typeof window.markdownItAttrs !== 'undefined',
                        breaksMode: false
                    };
                };
            ";
        }

        private string GetCss()
        {
            return @"
                * {
                    box-sizing: border-box;
                }
                
                html {
                    scroll-behavior: auto;
                }
                
                body { 
                    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Helvetica, Arial, sans-serif; 
                    line-height: 1.6; 
                    padding: 20px; 
                    max-width: 900px; 
                    margin: 0 auto; 
                    color: #333; 
                    background: #fff;
                }
                
                #preview {
                    min-height: 100vh;
                }
                
                h1, h2, h3, h4, h5, h6 { 
                    margin-top: 24px; 
                    margin-bottom: 16px; 
                    font-weight: 600; 
                    line-height: 1.25; 
                }
                
                h1 { 
                    font-size: 2em; 
                    border-bottom: 1px solid #eaecef; 
                    padding-bottom: 0.3em; 
                }
                
                h2 { 
                    font-size: 1.5em; 
                    border-bottom: 1px solid #eaecef; 
                    padding-bottom: 0.3em; 
                }
                
                h3 { font-size: 1.25em; }
                h4 { font-size: 1em; }
                h5 { font-size: 0.875em; }
                h6 { font-size: 0.85em; color: #6a737d; }
                
                p { 
                    margin-top: 0;
                    margin-bottom: 16px;
                }
                
                br {
                    display: block;
                    content: '';
                    margin-top: 0;
                }
                
                a { 
                    color: #0366d6; 
                    text-decoration: none; 
                }
                
                a:hover { 
                    text-decoration: underline; 
                }
                
                strong {
                    font-weight: 600;
                }
                
                em {
                    font-style: italic;
                }
                
                code { 
                    background: rgba(175,184,193,0.2); 
                    padding: 0.2em 0.4em; 
                    border-radius: 6px; 
                    font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
                    font-size: 85%;
                }
                
                pre { 
                    background: #f6f8fa; 
                    padding: 16px; 
                    overflow: auto; 
                    border-radius: 6px;
                    line-height: 1.45;
                    border: 1px solid #e1e4e8;
                    margin-bottom: 16px;
                }
                
                pre code {
                    background: transparent;
                    padding: 0;
                    border-radius: 0;
                    font-size: 100%;
                }
                
                blockquote {
                    padding: 0 1em;
                    color: #6a737d;
                    border-left: 0.25em solid #dfe2e5;
                    margin: 0 0 16px 0;
                }
                
                blockquote > :first-child { 
                    margin-top: 0; 
                }
                
                blockquote > :last-child { 
                    margin-bottom: 0; 
                }
                
                ul, ol {
                    padding-left: 2em;
                    margin-top: 0;
                    margin-bottom: 16px;
                }
                
                li { 
                    margin-bottom: 0.25em;
                }
                
                li > p {
                    margin-bottom: 0.25em;
                }
                
                img { 
                    max-width: 100%; 
                    height: auto; 
                    border-radius: 6px;
                    box-sizing: border-box;
                }
                
                img[align='right'] { 
                    float: right; 
                    margin-left: 20px;
                    margin-bottom: 10px;
                }
                
                img[align='left'] { 
                    float: left; 
                    margin-right: 20px;
                    margin-bottom: 10px;
                }
                
                mark {
                    background-color: #ffff00;
                    padding: 0.2em 0.4em;
                    border-radius: 3px;
                }
                
                span[style*='background-color'] {
                    padding: 0.2em 0.4em;
                    border-radius: 3px;
                }
                
                table {
                    border-collapse: collapse;
                    border-spacing: 0;
                    margin-bottom: 16px;
                    width: 100%;
                    display: block;
                    overflow: auto;
                }
                
                table th, table td {
                    padding: 6px 13px;
                    border: 1px solid #dfe2e5;
                }
                
                table th {
                    font-weight: 600;
                    background: #f6f8fa;
                }
                
                table tr {
                    background-color: #fff;
                    border-top: 1px solid #c6cbd1;
                }
                
                table tr:nth-child(2n) {
                    background-color: #f6f8fa;
                }
                
                hr {
                    height: 0.25em;
                    padding: 0;
                    margin: 24px 0;
                    background-color: #e1e4e8;
                    border: 0;
                }
                
                .token.deleted {
                    background-color: #ffeef0;
                }
                
                .token.inserted {
                    background-color: #e6ffed;
                }
            ";
        }
    }
}



