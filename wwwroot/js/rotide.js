let gRotide = {};
let gLoadedFile = "";
let gRotideElement = null;
let gSelectedProject = "";

$(document).ready(function() {
    //BuildFileTree();
    gRotideElement = document.getElementById("textInput");
    gRotide = CodeMirror.fromTextArea(gRotideElement, {
        lineNumbers: true,
        mode: "javascript",
        theme: "monokai",
    });
    const rotideHeight = $(window).height() - 150; // TODO: find a better way to set the height of the editor window
    gRotide.setSize(-1, rotideHeight);
    //BindContextMenu();
    BindDropDownMenuAction();
});

// Drop down menu handler
function BindDropDownMenuAction() {
    $("a.dropdown-item").on("click", function(){
        gSelectedProject = this.innerHTML;
        BuildFileTree(gSelectedProject);
    });
}

// TODO: create right-click menu
(function ($, window) {

    $.fn.contextMenu = function (settings) {

        return this.each(function () {

            // Open context menu
            $(this).on("contextmenu", function (e) {
                // return native menu if pressing control
                if (e.ctrlKey) return;
                
                //open menu
                var $menu = $(settings.menuSelector)
                    .data("invokedOn", $(e.target))
                    .show()
                    .css({
                        position: "absolute",
                        left: getMenuPosition(e.clientX, 'width', 'scrollLeft'),
                        top: getMenuPosition(e.clientY, 'height', 'scrollTop')
                    })
                    .off('click')
                    .on('click', 'a', function (e) {
                        $menu.hide();
                
                        var $invokedOn = $menu.data("invokedOn");
                        var $selectedMenu = $(e.target);
                        
                        settings.menuSelected.call(this, $invokedOn, $selectedMenu);
                    });
                
                return false;
            });

            //make sure menu closes on any click
            $('body').click(function () {
                $(settings.menuSelector).hide();
            });
        });
        
        function getMenuPosition(mouse, direction, scrollDir) {
            var win = $(window)[direction](),
                scroll = $(window)[scrollDir](),
                menu = $(settings.menuSelector)[direction](),
                position = mouse + scroll;
                        
            // opening menu would pass the side of the page
            if(mouse + menu > win && menu < mouse)
                position -= menu;
            
            return position;
        }

    };
})(jQuery, window);

$("div .fileTree").contextMenu({
    menuSelector: "#contextMenu",
    menuSelected: function (invokedOn, selectedMenu) {
        var msg = "You selected the menu item '" + selectedMenu.text() +
            "' on the value '" + invokedOn.text() + "'";
        alert(msg);
    }
});


// Right-click context menu handler
//function BindContextMenu(){
//    //console.log($("li.directory > a"));
//    $("li.directory > a").bind("contextmenu",function(e){
//        e.preventDefault();
//        //console.log(e.pageX + "," + e.pageY);
//        const menuDirObj = $("#ctxMenuDir");
//        menuDirObj.css("left",e.pageX);
//        menuDirObj.css("top",e.pageY);
//        // $("#ctxMenuDir").hide(100);
//        menuDirObj.fadeIn(200,startFocusOut());
//    });
//}

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
    const data = JSON.stringify({
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
        { script: "/Rotide/GetFiles", root: projectName },
        function(file) {
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

    const data = JSON.stringify({
        ProjectName: gSelectedProject,
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
    const ext = fileName.split('.')[1];
    let retValue = "";

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
    const ext = fileName.split('.').length === 2 ? fileName.split('.')[1] : "";
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