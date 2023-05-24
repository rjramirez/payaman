
/* Formatting function for row details - modify as you need */
function format(d) {
    // `d` is the original data object for the row
    return (
        '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
        '<tr>' +
        '<td>ID:</td>' +
        '<td>' +
        d.AppUserId +
        '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Date Created:</td>' +
        '<td>' +
        moment(d.createdDate).format('MMMM Do YYYY, h:mm:ss a') +
        '</td>' +
        '</tr>' +
        '</table>'
    );
}

let User = function () {
    let usersEndPoint = "/AppUser/GetAllAppUsers";
    let updateUserEndPoint = "/AppUser/Update";
    let removeUserEndPoint = "/AppUser/Remove";
    let addUserEndPoint = "/AppUser/Add";

    return {
        initializeUsersTable: function () {

            App.singleSelect2("#ddlAddUserRole");
            App.singleSelect2("#ddlEditUserRole");


            var tableUsers = $('#users_table').DataTable({
                ajax:
                {
                    url: window.location.origin + usersEndPoint,
                    type: 'GET',
                    contentType: 'application/json; charset=utf-8',
                    dataSrc: function (receivedData) {
                        //console.log(receivedData);
                        return receivedData;
                    },
                },
                dom: "<'top'<'row'<'col-sm-2'B><'col-sm-4'l><'col-sm-4'f>>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-9'p>>",
                buttons: [
                    {
                        className: 'btn-xs btn-success mt-1',
                        text: '<i class="fa-solid fa-plus"></i> Add',
                        action: function (e, dt, node, config) {
                            $('#addUserModal').modal('show');

                            $("#btnUserAdd").prop("onclick", null).off("click");
                            $("#btnUserAdd").click(function () {
                                App.addButtonSpinner($("#btnUserAdd"));


                                let userFirstnameAdd = $("#inputAddUserFirstname").val();
                                let userLastnameAdd = $("#inputAddUserLastname").val();
                                let userUsernameAdd = $("#inputAddUserUsername").val();
                                let userPasswordAdd = $("#inputAddUserPassword").val();
                                let userRoleAdd = $("#ddlAddUserRole").val();

                                
                                App.requiredTextValidator(userFirstnameAdd, $('#inputAddUserFirstname'));
                                App.requiredTextValidator(userLastnameAdd, $('#inputAddUserLastname'));
                                App.requiredTextValidator(userUsernameAdd, $('#inputAddUserUsername'));
                                App.requiredTextValidator(userPasswordAdd, $('#inputAddUserPassword'));
                                App.requiredSingleSelectValidator(userRoleAdd, $("#ddlAddUserRole"));


                                if (userFirstnameAdd == "" || userLastnameAdd == "" || userUsernameAdd == "" ||
                                    userPasswordAdd == "" || userRoleAdd == "") {
                                    App.alert("error", "All fields are required", "Error", undefined);
                                    App.removeButtonSpinner($("#btnUserAdd"));
                                    return;
                                }

                                let model = {
                                    FirstName: userFirstnameAdd,
                                    LastName: userLastnameAdd,
                                    Username: userUsernameAdd,
                                    Password: userPasswordAdd,
                                    AppUserRoleId: userRoleAdd
                                }

                                App.ajaxPost(addUserEndPoint,
                                    JSON.stringify(model),
                                    'text',
                                    function (data) {
                                        var json = JSON.parse(data);

                                        if (json.isSuccessful) {
                                            App.removeButtonSpinner($("#btnUserAdd"));
                                            $("#addUserModal").modal("hide");
                                            App.alert("success", json.message, "Success", window.location.origin + "/Home/Users");
                                        }
                                        else {
                                            App.alert("error", json.message, "Error", undefined);

                                            setTimeout(function () {
                                                App.removeButtonSpinner($("#btnUserAdd"));
                                            }, 500);
                                        }

                                    },
                                    function (response) {
                                        console.log(response);
                                    }
                                );


                            });
                        }
                    }
                ],
                columns: [
                    {
                        className: 'dt-control',
                        orderable: false,
                        data: null,
                        defaultContent: '',
                    },
                    { data: 'FirstName' },
                    { data: 'LastName' },
                    { data: 'Username' },
                    {
                        data: 'AppUserRole',
                        render: function (data) {
                            return data.Name
                        }
                    },
                    {
                        data: 'AppUserId',
                        render: function (data, type, row) {
                            return '<button type="button" class="btn btn-success btn-xs btnUserEdit" data-appuserid="' + data + '"'
                                + 'data-appuserrolename="' + row.AppUserRole.Name + '"'
                                + 'data-appuserroleid="' + row.AppUserRole.Value + '"'
                                + 'data-firstname="' + row.FirstName + '"'
                                + 'data-lastname="' + row.LastName + '"'
                                + 'data-username="' + row.Username + '"'
                                + '><i class="fa-solid fa-pencil"></i></button> | <button type="button" class="btn btn-danger btn-xs btnUserRemove" data-appuserid=' + data + '><i class="fa-solid fa-trash"></i></button>'
                        }
                    }
                ],
                order: [[1, 'desc']]
            });

            // Add event listener for opening and closing details
            $('#users_table tbody').on('click', 'td.dt-control', function () {
                var tr = $(this).closest('tr');
                var row = tableUsers.row(tr);

                if (row.child.isShown()) {
                    // This row is already open - close it
                    row.child.hide();
                    tr.removeClass('shown');
                } else {
                    // Open this row
                    row.child(format(row.data())).show();
                    tr.addClass('shown');
                }
            });


            $('#users_table').on('init.dt', function () {

                $(".btnUserEdit").prop("onclick", null).off("click");
                $(".btnUserEdit").click(function () {
                    let userId = $(this).data("appuserid");
                    let firstName = $(this).data("firstname");
                    let lastName = $(this).data("lastname");
                    let userName = $(this).data("username");
                    let roleId = $(this).data("appuserroleid");


                    $("#inputEditUserId").val(userId);
                    $("#inputEditRoleId").val(roleId);
                    $("#inputEditUserFirstname").val(firstName);
                    $("#inputEditUserLastname").val(lastName);
                    $("#inputEditUsername").val(userName);

                    $("#ddlEditUserRole").val(roleId).trigger("change");

                    $("#editUserModal").modal("show");
                });


                $("#btnUserUpdate").prop("onclick", null).off("click");
                $("#btnUserUpdate").click(function () {
                    App.addButtonSpinner($("#btnUserUpdate"));

                    let userId = $("#inputEditUserId").val();
                    let userFirstnameUpdate = $("#inputEditUserFirstname").val();
                    let userLastnameUpdate = $("#inputEditUserLastname").val();
                    let userUsernameUpdate = $("#inputEditUsername").val();
                    let userPasswordUpdate = $("#inputEditUserPassword").val();
                    let userRoleUpdate = $("#ddlEditUserRole").val();


                    App.requiredTextValidator(userFirstnameUpdate, $('#inputEditUserFirstname'));
                    App.requiredTextValidator(userLastnameUpdate, $('#inputEditUserLastname'));
                    App.requiredTextValidator(userUsernameUpdate, $('#inputEditUsername'));
                    App.requiredTextValidator(userPasswordUpdate, $('#inputEditUserPassword'));
                    App.requiredSingleSelectValidator(userRoleUpdate, $("#ddlEditUserRole"));

                    if (userFirstnameUpdate == "" || userLastnameUpdate == "" || userUsernameUpdate == "" ||
                        userPasswordUpdate == "" || userRoleUpdate == "") {
                        App.alert("error", "All fields are required", "Error", undefined);
                        App.removeButtonSpinner($("#btnUserUpdate"));
                        return;
                    }


                    let model = {
                        AppUserId: userId,
                        FirstName: userFirstnameUpdate,
                        LastName: userLastnameUpdate,
                        Username: userUsernameUpdate,
                        Password: userPasswordUpdate,
                        AppUserRoleId: userRoleUpdate
                    }

                    App.ajaxPut(updateUserEndPoint,
                        JSON.stringify(model),
                        'text',
                        function (data) {
                            var json = JSON.parse(data);

                            if (json.isSuccessful) {
                                App.removeButtonSpinner($("#btnUserUpdate"));
                                $("#editUserModal").modal("hide");
                                App.alert("success", json.message, "Success", window.location.origin + "/Home/Users");
                            }
                            else {
                                App.alert("error", json.message, "Error", undefined);

                                setTimeout(function () {
                                    App.removeButtonSpinner($("#btnUserUpdate"));
                                }, 500);
                            }

                        },
                        function (response) {
                            console.log(response);
                        }
                    );


                });


                $(".btnUserRemove").prop("onclick", null).off("click");
                $(".btnUserRemove").click(function () {

                    let appUserId = $(this).data("appuserid");

                    App.confirmDialogueModal("Message",
                        "Are you sure you want to delete this user?",
                        "bg-warning",
                        User.deleteUser(appUserId),
                        undefined,
                        undefined
                    );


                });
            });
        },
        deletUser: function (appUserId) {

            let param = {
                AppUserId: appUserId
            }

            App.ajaxPost(removeUserEndPoint,
                JSON.stringify(param),
                'text',
                function (data) {
                    var json = JSON.parse(data);

                    if (json.isSuccessful) {
                        App.alert("success", json.message, "Success", window.location.origin + "/Home/Users");

                    }
                    else {
                        App.alert("error", json.message, "Error", undefined);

                        setTimeout(function () {
                        }, 500);
                    }

                },
                function (response) {
                    console.log(response);
                }
            );
        }
    }
}();



$(document).ready(function () {
    User.initializeUsersTable();
});