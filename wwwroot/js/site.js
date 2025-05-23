$(document).ready(function () {
    $.validator.setDefaults({
        ignore: "[data-val-ignore-id=true] [name='Id']"
    });

    $("form").on("submit", function (e) {
        if (!$(this).valid()) {
            console.log("Validation failed. Errors: ", $(this).validate().errorList);
        } else {
            console.log("Form submitted: " + $(this).attr("action"));
            console.log("Form data: ", $(this).serialize());
        }
    });
});