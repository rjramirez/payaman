
/* Formatting function for row details - modify as you need */
function format(d) {
    // `d` is the original data object for the row
    return (
        '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
        '<tr>' +
        '<td>ID:</td>' +
        '<td>' +
        d.Id +
        '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Date Created:</td>' +
        '<td>' +
        moment(d.CreatedDate).format('MMMM Do YYYY, h:mm:ss a') +
        '</td>' +
        '</tr>' +
        '</table>'
    );
}

let Product = function () {
    let productsEndPoint = "/Product/GetAllProducts";
    let updateProductEndPoint = "/Product/Update";
    let removeProductEndPoint = "/Product/Remove";
    let addProductEndPoint = "/Product/Add";

    return {
        resetTable: function () {
            // Quickly and simply clear a table
            $('#products_table').dataTable().fnClearTable();

            // Restore the table to it's original state in the DOM by removing all of DataTables enhancements, alterations to the DOM structure of the table and event listeners
            $('#products_table').dataTable().fnDestroy();
        },
        initializeStoreSelect: function () {

            let storesUrl = window.location.origin + "/Store/GetAllStores";
            App.ajaxGet(storesUrl
                , "text"
                , function (data) {
                    var jsonStore = JSON.parse(data);

                    let storeOptionsHTML = "";
                    for (var x = 0; x < jsonStore.length; x++) {
                        if ($("#spanStoreNameSelected").html() == jsonStore[x].name) {
                            storeOptionsHTML += '<option value="' + jsonStore[x].id + '" selected="selected">' + jsonStore[x].name + '</option>';
                        }
                        storeOptionsHTML += '<option value="' + jsonStore[x].id + '">' + jsonStore[x].name + '</option>';
                    }
                    $("#selectStoreName").html(storeOptionsHTML);

                    App.hidePreloader();
                }
                , function () {
                    App.hidePreloader();
                }
            );
        },
        initializeProductsTable: function () {

            var tableProducts = $('#products_table').DataTable({
                ajax:
                {
                    url: window.location.origin + productsEndPoint,
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
                            $('#addProductModal').modal('show');

                            $("#btnProductAdd").prop("onclick", null).off("click");
                            $("#btnProductAdd").click(function () {
                                App.addButtonSpinner($("#btnProductAdd"));

                                let productNameAdd = $("#input_add_productname").val();
                                let productPriceAdd = $("#input_add_productprice").val();

                                App.requiredTextValidator($('#input_add_productname').val(), $('#input_add_productname'));
                                App.requiredTextValidator($('#input_add_productprice').val(), $('#input_add_productprice'));

                                if (productNameAdd == "" || productPriceAdd == "" || productPriceAdd == 0) {
                                    App.alert("error", "Name and Price is required", "Error", undefined);
                                    App.removeButtonSpinner($("#btnProductAdd"));
                                    return;
                                }

                                let model = {
                                    Name: productNameAdd,
                                    Price: productPriceAdd
                                }

                                App.ajaxPost(addProductEndPoint,
                                    JSON.stringify(model),
                                    'text',
                                    function (data) {
                                        var json = JSON.parse(data);

                                        if (json.isSuccessful) {
                                            App.removeButtonSpinner($("#btnProductAdd"));
                                            $("#addProductModal").modal("hide");
                                            App.alert("success", json.message, "Success", window.location.origin + "/Home/Products");
                                        }
                                        else {
                                            App.alert("error", json.message, "Error", undefined);

                                            setTimeout(function () {
                                                App.removeButtonSpinner($("#btnProductAdd"));
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
                    { data: 'Name' },
                    {
                        data: 'Price',
                        render: function (data) {
                            return '₱' + data;
                        }
                    },
                    {
                        data: 'Id',
                        render: function (data, type, row) {
                            return '<button type="button" class="btn btn-success btn-xs btnProductEdit" data-id="' + data + '"'
                                + 'data-name="' + row.Name + '"'
                                + 'data-price="' + row.Price + '"'
                                + '><i class="fa-solid fa-pencil"></i></button> | <button type="button" class="btn btn-danger btn-xs btnProductRemove" data-id=' + data + '><i class="fa-solid fa-trash"></i></button>'
                        }
                    }
                ],
                order: [[1, 'asc']]
            });

            // Add event listener for opening and closing details
            $('#products_table tbody').on('click', 'td.dt-control', function () {
                var tr = $(this).closest('tr');
                var row = tableProducts.row(tr);

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


            $('#products_table').on('init.dt', function () {

                $(".btnProductEdit").prop("onclick", null).off("click");
                $(".btnProductEdit").click(function () {
                    let productId = $(this).data("id");
                    let productName = $(this).data("name");
                    let productPrice = $(this).data("price");

                    $("#input_edit_productname").val(productName);
                    $("#input_edit_productprice").val(productPrice);
                    $("#input_edit_productid").val(productId);

                    $("#editProductModal").modal("show");
                });


                $("#btnProductUpdate").prop("onclick", null).off("click");
                $("#btnProductUpdate").click(function () {
                    App.addButtonSpinner($("#btnProductUpdate"));

                    let productId = $("#input_edit_productid").val();
                    let productNameUpdate = $("#input_edit_productname").val();
                    let productPriceUpdate = $("#input_edit_productprice").val();
                    let productStoreId = $("#selectStoreName").val();

                    App.requiredTextValidator($('#input_edit_productname').val(), $('#input_edit_productname'));
                    App.requiredTextValidator($('#input_edit_productprice').val(), $('#input_edit_productprice'));
                    App.requiredTextValidator($('#productStoreSelected').val(), $('#productStoreSelected'));
                    

                    if (productNameUpdate == "" || productPriceUpdate == "" || productPriceUpdate == 0 || productStoreId == "") {
                        App.alert("error", "Name, Price, and Store is required", "Error", undefined);
                        App.removeButtonSpinner($("#btnProductUpdate"));
                        return;
                    }

                    let model = {
                        Id: productId,
                        Name: productNameUpdate,
                        Price: productPriceUpdate,
                        StoreId: pro
                    }

                    App.ajaxPut(updateProductEndPoint,
                        JSON.stringify(model),
                        'text',
                        function (data) {
                            var json = JSON.parse(data);

                            if (json.isSuccessful) {
                                App.removeButtonSpinner($("#btnProductUpdate"));
                                $("#editProductModal").modal("hide");
                                App.alert("success", json.message, "Success", window.location.origin + "/Home/Products");
                            }
                            else {
                                App.alert("error", json.message, "Error", undefined);

                                setTimeout(function () {
                                    App.removeButtonSpinner($("#btnProductUpdate"));
                                }, 500);
                            }

                        },
                        function (response) {
                            console.log(response);
                        }
                    );


                });


                $(".btnProductRemove").prop("onclick", null).off("click");
                $(".btnProductRemove").click(function () {
                    let productId = $(this).data("id");

                    let param = {
                        Id: productId
                    }

                    App.ajaxPost(removeProductEndPoint,
                        JSON.stringify(param),
                        'text',
                        function (data) {
                            var json = JSON.parse(data);

                            if (json.isSuccessful) {
                                App.alert("success", json.message, "Success", window.location.origin + "/Home/Products");

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



            //Populate Store names in Modal
            $("#selectStoreName").html();
        }
    }
}();



$(document).ready(function () {
    Product.initializeProductsTable();
    Product.initializeStoreSelect();
});