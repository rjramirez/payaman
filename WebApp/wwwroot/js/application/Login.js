$(document).ready(function () {
    Login.initialize();
});

let Login = function () {
    let userEndPoint = "/Home/Login";

    return {
        initialize: function () {
            
            $("#btn_login").click(function () {
                Login.authenticate();
            });

            $("#btn_signup").click(function () {
                Login.register();
            });
        },
        register: function () {
            App.addButtonSpinner($("#btn_signup"));



            App.requiredTextValidator($('#input_username').val(), $('#input_username'));
            App.requiredTextValidator($('#input_password').val(), $('#input_password'));

            let un = $("#input_username").val();
            let pw = $("#input_password").val();

            if (un == "" || pw == "") {
                App.alert("error", "Username and Password is required", "Error", undefined);
                App.removeButtonSpinner($("#btn_signup"));
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
                        App.alert("success", "Sign in successful", "Success", undefined);
                        App.removeButtonSpinner($("#btn_signup"));

                        let dashboard = window.location.origin + "/Home/Index";
                        window.location.replace(dashboard);
                    }
                    else {
                        App.alert("error", json.message, "Error", undefined);

                        setTimeout(function () {
                            App.removeButtonSpinner($("#btn_signup"));
                        }, 500);
                    }

                },
                function (response) {

                    console.log(response);
                    App.removeButtonSpinner($("#btn_signup"));
                }
            );

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
                        App.alert("success", "Sign in successful", "Success", undefined);
                        App.removeButtonSpinner($("#btn_login"));

                        let dashboard = window.location.origin + "/Home/Index";
                        window.location.replace(dashboard);
                    }
                    else {
                        App.alert("error", json.message, "Error", undefined);

                        setTimeout(function () {
                            App.removeButtonSpinner($("#btn_login"));
                        }, 500);
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