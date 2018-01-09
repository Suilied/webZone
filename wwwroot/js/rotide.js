var gRotide = {};
var gLoadedFile = "";
var gRotideElement = null;
var gSelectedProject = null;

function OpenFile() {
    var filePath = $("#filePath").val();
    $.get("/Rotide/GetFileContents?filePath=" + filePath,
        function(data) {
            $("#textInput").val(data);
        });
}

$(document).ready(function() {
    //BuildFileTree();

    gRotideElement = document.getElementById("textInput");
    gRotide = CodeMirror.fromTextArea(gRotideElement, {
        lineNumbers: true,
        mode: "javascript",
        theme: "monokai",
    });
    var rotideHeight = $(window).height() - 150; // TODO: find a better way to set the height of the editor window
    gRotide.setSize(-1, rotideHeight);
});

$("select.project-selector").onchange = function(){
    // call GetProject with the correct params
    console.log("Great value:", this.value);
});

function GetProject(projectName) {
    $.ajax({
        type: "POST",
        url: "/Rotide/GetProject",
        data: { name: projectName },
        contentType: "application/json",
        success: function (data) {
            console.log(data);
        }
    });
}

function BuildFileTree() {
    $(".fileTree").fileTree(
        { script: "/Rotide/GetFiles" },
        function(file) {
            LoadFile(file);
        });
}

function LoadFile(file) {
    if (!HasValidExtension(file))
        return;

    $.get("/Rotide/GetFileContents",
        { filepath: file },
        function (data) {
            gLoadedFile = file;
            gRotide.setValue(data.content);
            gRotide.setOption("mode", GetModeFromExtension(file));
        });
}

function SaveFile() {
    gRotide.save();

    var data = JSON.stringify({
        FilePath: gLoadedFile,
        FileContents: gRotide.getTextArea().value
    });

    $.ajax({
        type: "POST",
        url: "/Rotide/SaveFileContents",
        data: data,
        contentType: "application/json",
        success: function (data) {
            console.log(data);
        }
    });
}

function GetModeFromExtension(fileName) {
    var ext = fileName.split('.')[1];
    var retValue = "";

    switch(ext) {
        case "txt":
        case "json":
        case "js":
            retValue = "javascript";
            break;
        case "cshtml":
            retValue = "application/x-ejs";
            break;
        case "html":
        case "htm":
            retValue = "htmlmixed";
            break;
        case "c":
        case "cpp":
        case "h":
        case "cs":
            retValue = "text/x-c++src";
            break;
        case "md":
            retValue = "markdown";
            break;
        case "css":
            retValue = "css";
            break;
        case "php":
            retValue = "php";
            break;
        case "py":
            retValue = "python";
            break;
        case "sql":
            retValue = "sql";
            break;
        case "xml":
            retValue = "xml";
            break;
        case "lua":
            retValue = "lua";
            break;
        default:
            retValue = "javascript";
    }

    return retValue;
}

function HasValidExtension(fileName) {
    var ext = fileName.split('.').length === 2 ? fileName.split('.')[1] : "";
    if (ext === "")
        return false;

    switch(ext) {
        case "txt":
        case "js":
        case "html":
        case "htm":
        case "c":
        case "cpp":
        case "h":
        case "cs":
        case "cshtml":
        case "json":
        case "md":
        case "css":
        case "php":
        case "py":
        case "sql":
        case "xml":
        case "lua":
            return true;
            break;
        default:
            return false;
    }
}