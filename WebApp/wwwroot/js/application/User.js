
/* Formatting function for row details - modify as you need */
function format(d) {
    // `d` is the original data object for the row
    return (
        '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
        '<tr>' +
        '<td>ID:</td>' +
        '<td>' +
        d.id +
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
    let usersEndPoint = "/User/GetAllUsers";
    let updateUserEndPoint = "/User/Update";
    let removeUserEndPoint = "/User/Remove";
    let addUserEndPoint = "/User/Add";

    return {
        initializeUsersTable: function () {

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

                                let userNameAdd = $("#input_add_username").val();
                                let userAddressAdd = $("#input_add_useraddress").val();

                                App.requiredTextValidator($('#input_add_username').val(), $('#input_add_username'));
                                App.requiredTextValidator($('#input_add_useraddress').val(), $('#input_add_useraddress'));

                                if (userNameAdd == "" || userAddressAdd == "") {
                                    App.alert("error", "Name and Address is required", "Error", undefined);
                                    App.removeButtonSpinner($("#btnUserAdd"));
                                    return;
                                }

                                let model = {
                                    Name: userNameAdd,
                                    Address: userAddressAdd
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
                    { data: 'name' },
                    {
                        data: 'id',
                        render: function (data, type, row) {
                            return '<button type="button" class="btn btn-success btn-xs btnUserEdit" data-id="' + data + '"'
                                + 'data-name="' + row.name + '"'
                                + 'data-address="' + row.address + '"'
                                + '><i class="fa-solid fa-pencil"></i></button> | <button type="button" class="btn btn-danger btn-xs btnUserRemove" data-id=' + data + '><i class="fa-solid fa-trash"></i></button>'
                        }
                    }
                ],
                order: [[1, 'asc']]
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
                    let userId = $(this).data("id");
                    let userName = $(this).data("name");
                    let userAddress = $(this).data("address");

                    $("#input_edit_username").val(userName);
                    $("#input_edit_userid").val(userId);
                    $("#input_edit_useraddress").val(userAddress);

                    $("#editUserModal").modal("show");
                });


                $("#btnUserUpdate").prop("onclick", null).off("click");
                $("#btnUserUpdate").click(function () {
                    App.addButtonSpinner($("#btnUserUpdate"));

                    let userId = $("#input_edit_userid").val();
                    let userNameUpdate = $("#input_edit_username").val();
                    let userAddressUpdate = $("#input_edit_useraddress").val();

                    App.requiredTextValidator($('#input_edit_username').val(), $('#input_edit_username'));
                    App.requiredTextValidator($('#input_edit_useraddress').val(), $('#input_edit_useraddress'));

                    if (userNameUpdate == "") {
                        App.alert("error", "Name and Price is required", "Error", undefined);
                        App.removeButtonSpinner($("#btnUserUpdate"));
                        return;
                    }

                    let model = {
                        Id: userId,
                        Name: userNameUpdate,
                        Address: userAddressUpdate
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
                    let userId = $(this).data("id");

                    let param = {
                        Id: userId
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
                });
            });
        }
    }
}();



$(document).ready(function () {
    User.initializeUsersTable();
});