window.markdownEditor = {
  wrapSelection: function (id, before, after) {
    const ta = document.getElementById(id);
    if (!ta) return;
    const start = ta.selectionStart ?? 0;
    const end = ta.selectionEnd ?? 0;
    const value = ta.value ?? "";
    const selected = value.substring(start, end);
    const newText = before + (selected || "") + after;
    ta.setRangeText(newText, start, end, "end");
    ta.dispatchEvent(new Event("input"));
    ta.focus();
  },
  prefixLine: function (id, prefix) {
    const ta = document.getElementById(id);
    if (!ta) return;
    const start = ta.selectionStart ?? 0;
    const end = ta.selectionEnd ?? 0;
    const value = ta.value ?? "";
    const lineStart = value.lastIndexOf("\n", start - 1) + 1;
    const lineEnd = value.indexOf("\n", end);
    const actualEnd = lineEnd === -1 ? value.length : lineEnd;
    const line = value.substring(lineStart, actualEnd);
    const newLine = line.startsWith(prefix) ? line.slice(prefix.length) : prefix + line;
    ta.setRangeText(newLine, lineStart, actualEnd, "end");
    ta.dispatchEvent(new Event("input"));
    ta.focus();
  },
  prefixLines: function (id, prefix) {
    const ta = document.getElementById(id);
    if (!ta) return;
    const start = ta.selectionStart ?? 0;
    const end = ta.selectionEnd ?? 0;
    const value = ta.value ?? "";
    const blockStart = value.lastIndexOf("\n", start - 1) + 1;
    const blockEndIndex = value.indexOf("\n", end);
    const blockEnd = blockEndIndex === -1 ? value.length : blockEndIndex;
    const block = value.substring(blockStart, blockEnd);
    const lines = block.split("\n").map(line => line.startsWith(prefix) ? line : prefix + line);
    const newBlock = lines.join("\n");
    ta.setRangeText(newBlock, blockStart, blockEnd, "end");
    ta.dispatchEvent(new Event("input"));
    ta.focus();
  },
  insertLink: function (id) {
    const ta = document.getElementById(id);
    if (!ta) return;
    const start = ta.selectionStart ?? 0;
    const end = ta.selectionEnd ?? 0;
    const value = ta.value ?? "";
    const selected = value.substring(start, end);
    const text = selected || "link text";
    const newText = `[${text}](https://)`;
    ta.setRangeText(newText, start, end, "end");
    ta.dispatchEvent(new Event("input"));
    ta.focus();
  }
};

window.richTextEditor = {
  init: function (editorId, inputId) {
    const editor = document.getElementById(editorId);
    const input = document.getElementById(inputId);
    if (!editor || !input) return;
    editor.innerHTML = input.value || "";

    const sync = () => {
      input.value = editor.innerHTML;
      input.dispatchEvent(new Event("input", { bubbles: true }));
    };

    editor.addEventListener("input", sync);
    editor.addEventListener("blur", sync);
  },
  exec: function (editorId, command, value) {
    const editor = document.getElementById(editorId);
    if (!editor) return;
    editor.focus();
    document.execCommand(command, false, value);
    // Trigger sync for Blazor binding.
    const event = new Event("input", { bubbles: true });
    editor.dispatchEvent(event);
  },
  heading: function (editorId, level) {
    const tag = "H" + (level || 3);
    this.exec(editorId, "formatBlock", tag);
  },
  list: function (editorId) {
    this.exec(editorId, "insertUnorderedList");
  },
  link: function (editorId) {
    const url = prompt("Enter link URL", "https://");
    if (!url) return;
    this.exec(editorId, "createLink", url);
  }
};
