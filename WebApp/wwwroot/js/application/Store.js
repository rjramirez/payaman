
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

let Store = function () {
    let storesEndPoint = "/Store/GetAllStores";
    let updateStoreEndPoint = "/Store/Update";
    let removeStoreEndPoint = "/Store/Remove";
    let addStoreEndPoint = "/Store/Add";

    return {
        initializeStoresTable: function () {

            var tableStores = $('#stores_table').DataTable({
                ajax:
                {
                    url: window.location.origin + storesEndPoint,
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
                            $('#addStoreModal').modal('show');

                            $("#btnStoreAdd").prop("onclick", null).off("click");
                            $("#btnStoreAdd").click(function () {
                                App.addButtonSpinner($("#btnStoreAdd"));

                                let storeNameAdd = $("#input_add_storename").val();
                                let storeAddressAdd = $("#input_add_storeaddress").val();

                                App.requiredTextValidator($('#input_add_storename').val(), $('#input_add_storename'));
                                App.requiredTextValidator($('#input_add_storeaddress').val(), $('#input_add_storeaddress'));

                                if (storeNameAdd == "" || storeAddressAdd == "") {
                                    App.alert("error", "Name and Address is required", "Error", undefined);
                                    App.removeButtonSpinner($("#btnStoreAdd"));
                                    return;
                                }

                                let model = {
                                    Name: storeNameAdd,
                                    Address: storeAddressAdd
                                }

                                App.ajaxPost(addStoreEndPoint,
                                    JSON.stringify(model),
                                    'text',
                                    function (data) {
                                        var json = JSON.parse(data);

                                        if (json.isSuccessful) {
                                            App.removeButtonSpinner($("#btnStoreAdd"));
                                            $("#addStoreModal").modal("hide");
                                            App.alert("success", json.message, "Success", window.location.origin + "/Home/Stores");
                                        }
                                        else {
                                            App.alert("error", json.message, "Error", undefined);

                                            setTimeout(function () {
                                                App.removeButtonSpinner($("#btnStoreAdd"));
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
                            return '<button type="button" class="btn btn-success btn-xs btnStoreEdit" data-id="' + data + '"'
                                + 'data-name="' + row.name + '"'
                                + 'data-address="' + row.address + '"'
                                + '><i class="fa-solid fa-pencil"></i></button> | <button type="button" class="btn btn-danger btn-xs btnStoreRemove" data-id=' + data + '><i class="fa-solid fa-trash"></i></button>'
                        }
                    }
                ],
                order: [[1, 'asc']]
            });

            // Add event listener for opening and closing details
            $('#stores_table tbody').on('click', 'td.dt-control', function () {
                var tr = $(this).closest('tr');
                var row = tableStores.row(tr);

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


            $('#stores_table').on('init.dt', function () {

                $(".btnStoreEdit").prop("onclick", null).off("click");
                $(".btnStoreEdit").click(function () {
                    let storeId = $(this).data("id");
                    let storeName = $(this).data("name");
                    let storeAddress = $(this).data("address");

                    $("#input_edit_storename").val(storeName);
                    $("#input_edit_storeid").val(storeId);
                    $("#input_edit_storeaddress").val(storeAddress);

                    $("#editStoreModal").modal("show");
                });


                $("#btnStoreUpdate").prop("onclick", null).off("click");
                $("#btnStoreUpdate").click(function () {
                    App.addButtonSpinner($("#btnStoreUpdate"));

                    let storeId = $("#input_edit_storeid").val();
                    let storeNameUpdate = $("#input_edit_storename").val();
                    let storeAddressUpdate = $("#input_edit_storeaddress").val();

                    App.requiredTextValidator($('#input_edit_storename').val(), $('#input_edit_storename'));
                    App.requiredTextValidator($('#input_edit_storeaddress').val(), $('#input_edit_storeaddress'));

                    if (storeNameUpdate == "") {
                        App.alert("error", "Name and Price is required", "Error", undefined);
                        App.removeButtonSpinner($("#btnStoreUpdate"));
                        return;
                    }

                    let model = {
                        Id: storeId,
                        Name: storeNameUpdate,
                        Address: storeAddressUpdate
                    }

                    App.ajaxPut(updateStoreEndPoint,
                        JSON.stringify(model),
                        'text',
                        function (data) {
                            var json = JSON.parse(data);

                            if (json.isSuccessful) {
                                App.removeButtonSpinner($("#btnStoreUpdate"));
                                $("#editStoreModal").modal("hide");
                                App.alert("success", json.message, "Success", window.location.origin + "/Home/Stores");
                            }
                            else {
                                App.alert("error", json.message, "Error", undefined);

                                setTimeout(function () {
                                    App.removeButtonSpinner($("#btnStoreUpdate"));
                                }, 500);
                            }

                        },
                        function (response) {
                            console.log(response);
                        }
                    );


                });


                $(".btnStoreRemove").prop("onclick", null).off("click");
                $(".btnStoreRemove").click(function () {
                    let storeId = $(this).data("id");

                    let param = {
                        Id: storeId
                    }

                    App.ajaxPost(removeStoreEndPoint,
                        JSON.stringify(param),
                        'text',
                        function (data) {
                            var json = JSON.parse(data);

                            if (json.isSuccessful) {
                                App.alert("success", json.message, "Success", window.location.origin + "/Home/Stores");

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
    Store.initializeStoresTable();
});