

$(document).ready(function() {
    

    var toolbarOptions = [
        ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
        ['blockquote', 'code-block'],
        [{ 'list': 'ordered' }, { 'list': 'bullet' }],
        [{ 'script': 'sub' }, { 'script': 'super' }],      // superscript/subscript
        [{ 'indent': '-1' }, { 'indent': '+1' }],
        [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
        [{ 'align': [] }], ['link', 'image']
    ];
    var $listItems = $("ul#listOfBlocks li");
    

    for (var i = 0; i < $listItems.length; i++) {
        var $li = $listItems[i];
        var mainEditorIdentifier = $($li).data("iden");
        var quill = new Quill('#' + mainEditorIdentifier, {
            modules: {
                toolbar: toolbarOptions
            },
            theme: 'snow',
            placeholder: 'Compose an epic...'
        });
    }

    var editors = $(".editorArea");

    for (var i = 0; i < editors.length; i++) {
        $(editors[i]).hide();
    }

    var outputs = $(".output");

    for (var i = 0; i < outputs.length; i++) {
        $(outputs[i]).classList = "color-standard output";
        $(outputs[i]).show();
    }

    if (window.location.href.includes("Item/Create")) {
        $(".dropdown").hide();
    }
});

/**
 * Display the introduction editor.
 */
function showIntroductionEditor(el) {
    $("#introductionEditor").show();
    $("#introductionOutputContainer").hide();
    $(el).hide();
}
function removeIntroduction() {
    $("#introductionEditor").hide();
    $("#showIntroductionEditorButton").show();
    $(".ql-editor").html("");
    $("#introductionOutput").html("");
}
function saveBlock(blockId, blockInputIdentifier, outputContainer, editorIdentifier, outputTextIdentifier, editButtonIdentifier) {

    $(outputContainer).show();
    $(editButtonIdentifier).show();
    $(editorIdentifier).hide();
    var html = $(blockInputIdentifier + " .ql-editor").html();
    $(outputTextIdentifier).html(html);

    $.post({
        url: "/Item/UpdateTextBlock",
        data: {
            blockId: blockId,
            html: html
        },
        dataType: "json"
    });                            

    playSound('/sounds/ding.mp3', 0.1);
        
}

function removeBlock(blockId, editorIdentifier) {
    $(editorIdentifier).hide();

    $.post({
            url: "/Item/DeleteTextBlock",
            data: {
                blockId: blockId
            },
            dataType: "json"
        });
    playSound('/sounds/drop.wav', 0.1);
}


function addTextBlock(postId) {

    var ul = $("#listOfBlocks");
    $.post({
            url: "/Item/AddTextBlock",
            data: {
                postId: postId
            },
            dataType: "json"
        })
        .done(function (result, status) {

            $.post({
                url: "/Item/GetTextBlockHtml",
                    data: {
                        block: result
                    },
                    dataType: "html"
                })
                .done(function(html, status) {   
                    new Audio('/sounds/clickSound.wav').play(); 
                    ul.append(html);

                    var toolbarOptions = [
                        ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
                        ['blockquote', 'code-block'],
                        [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                        [{ 'script': 'sub' }, { 'script': 'super' }],      // superscript/subscript
                        [{ 'indent': '-1' }, { 'indent': '+1' }],
                        [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
                        [{ 'align': [] }],
                        ['link', 'image']
                    ];

                    var quill = new Quill('#' + result.mainEditorIdentifier, {
                        modules: {
                            toolbar: toolbarOptions
                        },
                        theme: 'snow',
                        placeholder: 'Compose an epic...'
                    });
                });


           
        });
}

function showBlockEditor(el, blockEditor, editorIdentifier, blockOutputContainer, editorOutputIdentifier) {
    $(blockEditor).hide();    
    $(editorIdentifier).show();
    $(el).hide();
    new Audio('/sounds/clickSound.wav').play();
};