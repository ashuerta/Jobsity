$(document).ready(function () {
    $("#frmCreateOne").submit(function (e) {
        e.preventDefault();
        debugger;
        // Get some values from elements on the page:
        var $form = $(this),
            data = {
                UserName: $form.find("input[name='UserName']").val(),
                Email: $form.find("input[name='Email']").val(),
                Password: $form.find("input[name='Password']").val()
            };
        if (data.Password != $form.find("input[name='passwordConfirm']").val()) {
            $('.error_msj > p').text('Password missmatch');
            return false;
        }
        $.ajax({
            type: "POST",
            url: api + 'Security/Register',
            data: JSON.stringify(data),
            success: createSuccess,
            error: createError,
            contentType: 'application/json'
        });
    });
});

function createSuccess(e) {
    if (e.success) {
        window.location.href = baseUrl + '/Home/Created'
        return;
    }
    $('.error_msj  > p').text('Error: ' + e.message);
}

function createError(e) {
    $('.error_msj  > p').text('Error: ' + e);
}
