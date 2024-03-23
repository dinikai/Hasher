let fileHash;

function search() {
    let hash = document.getElementById("searchHash").value.trim();
    let metaRequest = new XMLHttpRequest();
    metaRequest.open("GET", `/meta?hash=${hash}`);
    metaRequest.send();

    metaRequest.onload = function () {
        if (metaRequest.responseText != "File not exists") {
            let meta = JSON.parse(metaRequest.responseText);
            openDecryptWindow(hash, meta.FileName);
        } else {
            popup("File with entered hash not found.");
        }
    };
}

function clearDecryptResult() {
    document.getElementById("decryptResult").innerText = "";
    document.getElementById("decryptResult").classList.remove("--decrypt-success");
    document.getElementById("decryptResult").classList.remove("--decrypt-fail");
}

function openDecryptWindow(hash, name) {
    clearDecryptResult();

    document.getElementById("decryptWindow").classList.add("--popup-window-visible");
    document.getElementById("decryptWindowFileName").innerText = name;
    document.decryptForm.action = "/" + hash;
    fileHash = hash;
    document.getElementById("hashText").innerText = hash;
    document.getElementById("key").value = "";
}

function closeDecryptWindow() {
    document.getElementById("decryptWindow").classList.remove("--popup-window-visible");
}

function setDownload(value) {
    if (value) {
        document.getElementById("download").value = "download";
        document.decryptForm.target = "";
    } else {
        document.getElementById("download").value = "";
        document.decryptForm.target = "_blank";
    }
}

function deleteFile() {
    clearDecryptResult();

    let checksumRequest = new XMLHttpRequest();
    checksumRequest.open("GET", `/check?hash=${fileHash}&key=${document.getElementById("key").value}`);
    checksumRequest.send();
    document.getElementById("decryptResult").innerText = "Checking key...";

    checksumRequest.onload = function () {
        if (checksumRequest.responseText == "true") {
            document.getElementById("decryptResult").innerText = "Deleting...";
            document.getElementById("decryptResult").classList.add("--decrypt-success");
            document.getElementById("decryptResult").classList.remove("--decrypt-fail");
            location.replace(`/delete?hash=${fileHash}&key=${document.getElementById("key").value}`);
        } else {
            document.getElementById("decryptResult").innerText = "Key is invalid";
            document.getElementById("decryptResult").classList.add("--decrypt-fail");
            document.getElementById("decryptResult").classList.remove("--decrypt-success");
        }
    };
}

function submitDecryptForm(download) {
    setDownload(download);
    clearDecryptResult();

    let checksumRequest = new XMLHttpRequest();
    checksumRequest.open("GET", `/check?hash=${fileHash}&key=${document.getElementById("key").value}`);
    checksumRequest.send();
    document.getElementById("decryptResult").innerText = "Checking key...";

    checksumRequest.onload = function () {
        if (checksumRequest.responseText == "true") {
            document.getElementById("decryptResult").innerText = "Decrypted succesfully";
            document.getElementById("decryptResult").classList.add("--decrypt-success");
            document.getElementById("decryptResult").classList.remove("--decrypt-fail");
            document.decryptForm.submit();
        } else {
            document.getElementById("decryptResult").innerText = "Key is invalid";
            document.getElementById("decryptResult").classList.add("--decrypt-fail");
            document.getElementById("decryptResult").classList.remove("--decrypt-success");
        }
    };
}