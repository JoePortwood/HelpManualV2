$(document).ready(function () {
    hideOrShowControls();

    $("div").removeClass("hidden");

    tinymce.init({
        selector: 'textarea',
        height: 500,
        menubar: false,
        plugins: [
            'advlist autolink lists link image charmap print preview anchor textcolor',
            'searchreplace visualblocks code fullscreen',
            'insertdatetime media table contextmenu paste code help wordcount'
        ],
        toolbar: 'insert | undo redo |  formatselect | bold italic backcolor  | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | removeformat | help',
        content_css: [
            '//fonts.googleapis.com/css?family=Lato:300,300i,400,400i',
            '//www.tinymce.com/css/codepen.min.css']
    });
});

$('#ControlTypeId').change(function () {
    hideOrShowControls();
});

function hideOrShowControls()
{
    var value = $("#ControlTypeId option:selected").text();
    var data = document.getElementById('Data');

    $('#optionsSect').hide();
    $('#startEndSect').hide();
    $('#imageSect').hide();

    //Shows appropriate sections depending on the dropdown value
    switch (value) {
        case "Image":
            $('#dataSect').hide();
            $('#imageSect').show();
            break;

        case "Drop Down":
            $('#optionsSect').show();
            //$('#startEndSect').show();
            break;

        default:
            $('#dataSect').show();
            $('#imageSect').hide();
            break;
    }
}