

$(document).ready(function() {

    if (window.location.href.includes("/Item/Create")) {
        $("#postMetaDetails").hide();
        $("#imageuploaderbutton").hide();
    }

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
    $(outputTextIdentifier).show();
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
                        [{ 'align': [] }],
                        ['link']
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

function addHeaderBlock(postId) {

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
                .done(function (html, status) {
                    new Audio('/sounds/clickSound.wav').play();
                    ul.append(html);

                    var toolbarOptions = [
                        ['bold', 'underline'],
                        [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
                        [{ 'align': [] }]
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

function addImageBlock(postId) {

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
                .done(function (html, status) {
                    new Audio('/sounds/clickSound.wav').play();
                    ul.append(html);

                    var toolbarOptions = [
                        ['image']
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
    $("#" + el.id).hide();
    $(el).hide();
    new Audio('/sounds/clickSound.wav').play();
};


var element = document.querySelector('#choice');
element.addEventListener(
            'addItem',
            function (event) {
                // do something creative here...
                console.log(event.detail.id);
                console.log(event.detail.value);
                console.log(event.detail.label);
            },
            false
);

var availableChoices = [];

$.post({
        url: "/Topic/GetAvailableTopics",
        dataType: "json"
    })
    .done(function(result, status) {
        availableChoices = result;

        
        var choices = new Choices(element, {
            items: [],
            choices: availableChoices.map(x => { return { value: x.id, label: x.name } }),
            maxItemCount: 3,
            searchFloor: 3,
            searchResultLimit: 5,
            position: 'top',
        });

        


        choices.setChoices(
            availableChoices,
            'id',
            'name',
            true);

        var selectedFlags = [];

        if (window.location.href.includes("Item/Edit/")) {
            var id = parseInt(window.location.href.split("Edit/")[1]);

            if (id != null && id > 0) {
                $.post({
                        url: "/Topic/GetTopicsForItem",
                        data: {
                            id: id
                        },
                        dataType: "json"
                    })
                    .done(function(result, status) {
                        selectedFlags = result;
                        for (var i = 0; i < selectedFlags.length; i++) {
                            choices.setChoiceByValue(selectedFlags[i].id);
                        }
                    });
            } 
        }
    });

$("#imageFileUpload").on("change", function () {

    var output = $('#backgroundOutput');
    output.attr("src", URL.createObjectURL(event.target.files[0]));
    var el = $('#imageuploader');
    el.attr("class", el.attr("class") + " wow flipOutX");
    output.on("load",
        function () {
            URL.revokeObjectURL(output.src); // free memory
        });

    $("#uploadThumbnailForm").submit();
});