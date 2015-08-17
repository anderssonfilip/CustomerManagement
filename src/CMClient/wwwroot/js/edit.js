$(function () {

    // call Save Action on form submit
    $("#save").click(function () {
        var form = $("#editForm");
        form.attr("action", "/Customer/Save");
        form.submit();
    });

    // call Delete Action on form submit
    $("#delete").click(function () {
        var form = $("#editForm");
        form.attr("action", '/Customer/Delete');
        form.submit();
    });

});