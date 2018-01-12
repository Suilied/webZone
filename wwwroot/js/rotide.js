var gRotide = {};
var gLoadedFile = "";
var gRotideElement = null;
var gSelectedProject = "";

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
    BindContextMenu();
    BindDropDownMenuAction();
});


// Drop down menu handler
function BindDropDownMenuAction() {
    $("a.dropdown-item").on("click", function(){
        gSelectedProject = this.innerHTML;
        BuildFileTree(gSelectedProject);
    });
}

// Right-click context menu handler
function BindContextMenu(){
    //console.log($("li.directory > a"));
    $("li.directory > a").bind("contextmenu",function(e){
        e.preventDefault();
        //console.log(e.pageX + "," + e.pageY);
        const menuDirObj = $("#ctxMenuDir");
        menuDirObj.css("left",e.pageX);
        menuDirObj.css("top",e.pageY);
        // $("#ctxMenuDir").hide(100);
        menuDirObj.fadeIn(200,startFocusOut());
    });
}

function startFocusOut(){
    $(document).on("click",function(){
        $("#ctxMenuDir").hide();
        $(document).off("click");
    });
}

$(".items > li").click(function(){
    $("#op").text("You have selected "+$(this).text());
});


function GetProject(projectName) {
    $.ajax({
        type: "POST",
        url: "/Rotide/GetProject",
        data: { name: projectName },
        contentType: "application/json",
        success: function(data) {
            console.log(data);
        }
    });
}

function CreateNewProject() {
    // read the info from the modal
    var data = JSON.stringify({
        projectName: $("#inProjectName").val(),
        projectRoot: $("#inProjectRoot").val(),
    });

    // post the info to the server
    $.ajax({
        type: "POST",
        url: "/Rotide/CreateNewProject",
        data: data,
        contentType: "application/json",
        success: function (data) {
            console.log(data);

            // refresh page
            location.reload();
        }
    });

    $("#newProjectModal").modal("dispose");
}

function BuildFileTree(projectName) {
    // Build filetree with LoadFile as lmb-click event handler
    $(".fileTree").fileTree(
        { script: "/Rotide/GetFiles", root: projectName},
        function(file) {
            //console.log(file);
            LoadFile(file);
        }
    );
}

function LoadFile(file) {
    if (!HasValidExtension(file))
        return;

    $.get("/Rotide/GetFileContents",
        { project: gSelectedProject, filepath: file },
        function(data) {
            gLoadedFile = file;
            gRotide.setValue(data.content);
            gRotide.setOption("mode", GetModeFromExtension(file));
        }
    );
}

function SaveFile() {
    gRotide.save();

    var data = JSON.stringify({
        Project: gSelectedProject,
        FilePath: gLoadedFile,
        FileContents: gRotide.getTextArea().value
    });

    $.ajax({
        type: "POST",
        url: "/Rotide/SaveFileContents",
        data: data,
        contentType: "application/json",
        success: function(data) {
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