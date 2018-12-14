

$(document).ready(function () {
    initEditor($('#textarea'), false, function (ed) {
        
    });

    $('#savebtn').click(function (e) {
        var description = tinymce.activeEditor.getContent({format : 'raw'});
        $('#contentarea').append(description);
    });
});