$(document).ready(function () {
    Login.initialize();
});

let Login = function () {
    let userEndPoint = "/Home/AuthenticationCallback";

    return {
        initialize: function () {
            
            $("#btn_login").click(function () {
                Login.authenticate();
            });
        },
        authenticate: function () {

            App.addButtonSpinner($("#btn_login"));

            let un = $("#input_username").val();
            let pw = $("#input_password").val();

            if (un == "" || pw == "") {
                App.alert("error", "Username and Password is required", "Error", undefined);
                App.removeButtonSpinner($("#btn_login"));
                return;
            }

            let param = {
                Username: un,
                Password: pw
            };


            App.ajaxPost(userEndPoint,
                JSON.stringify(param),
                'text',
                function (data) {
                    var json = JSON.parse(data);

                    App.requiredTextValidator($('#input_username').val(), $('#input_username'));
                    App.requiredTextValidator($('#input_password').val(), $('#input_password'));

                    if (json.isCompleted) {
                        
                        App.alert("success", json.message, "Sucess", undefined);
                        App.removeButtonSpinner($("#btn_login"));

                    }
                    else {
                        App.alert("error", json.message, "Error", undefined);

                        setTimeout(function () {
                            App.removeBoxSpinner($("#btn_login"));
                        }, 500);
                        console.log(`Status Code: ${json.statusCode} - Error: ${json.error}`)
                    }

                },
                function (response) {

                    console.log(response);
                    App.removeButtonSpinner($("#btn_login"));
                }
            );


        },
        clearInputs: function () {
            $("#txtDateRange").val('');
            $("#txtSearchKeyword").val('');
        }
    }
}();