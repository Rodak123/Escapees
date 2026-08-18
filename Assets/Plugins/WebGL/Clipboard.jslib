mergeInto(LibraryManager.library, {
    CopyToClipboard: function (textPtr, alertTextPtr) {
        const str = textPtr ? UTF8ToString(textPtr) : "";
        const alertMsg = alertTextPtr ? UTF8ToString(alertTextPtr) : "";

        function showAlert() {
            if (alertMsg && alertMsg.length > 0) {
                alert(alertMsg);
            }
        }

        function fallbackCopy(textToCopy) {
            const textArea = document.createElement("textarea");
            textArea.value = textToCopy;
            textArea.style.position = "fixed";
            textArea.style.left = "-999999px";
            textArea.style.top = "-999999px";
            document.body.appendChild(textArea);
            textArea.focus();
            textArea.select();

            try {
                const successful = document.execCommand("copy");
                if (successful) {
                    showAlert();
                }
            } catch (err) {
                console.error("Fallback copy failed: ", err);
            }

            document.body.removeChild(textArea);
        }

        if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(str).then(function () {
                showAlert();
            }).catch(function () {
                fallbackCopy(str);
            });
        } else {
            fallbackCopy(str);
        }
    }
});