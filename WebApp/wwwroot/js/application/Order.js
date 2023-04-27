
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
                    "<'row'<'col-sm-4'l><'col-sm-8'p>>",
                buttons: [
                    {
                        className: 'btn-xs btn-success mt-1',
                        text: '<i class="fa-solid fa-plus"></i> Add',
                        action: function (e, dt, node, config) {
                            $('#addProductModal').modal('show');

                            $("#btn_product_add").click(function () {
                                App.addButtonSpinner($("#btn_product_add"));

                                let productNameAdd = $("#input_add_productname").val();
                                let productPriceAdd = $("#input_add_productprice").val();

                                App.requiredTextValidator($('#input_add_productname').val(), $('#input_add_productname'));
                                App.requiredTextValidator($('#input_add_productprice').val(), $('#input_add_productprice'));

                                if (productNameAdd == "" || productPriceAdd == "" || productPriceAdd == 0) {
                                    App.alert("error", "Name and Price is required", "Error", undefined);
                                    App.removeButtonSpinner($("#btn_product_add"));
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
                                            App.removeButtonSpinner($("#btn_product_add"));
                                            $("#addProductModal").modal("hide");
                                            App.alert("success", json.message, "Success", window.location.origin + "/Home/Products");
                                        }
                                        else {
                                            App.alert("error", json.message, "Error", undefined);

                                            setTimeout(function () {
                                                App.removeButtonSpinner($("#btn_product_add"));
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
                            return '<button type="button" class="btn btn-success btn-xs btn-product-edit" data-id="' + data + '"'
                                + 'data-name="' + row.Name + '"'
                                + 'data-price="' + row.Price + '"'
                                + '><i class="fa-solid fa-pencil"></i></button> | <button type="button" class="btn btn-danger btn-xs btn-product-remove" data-id=' + data + '><i class="fa-solid fa-trash"></i></button>'
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

                $(".btn-product-edit").click(function () {
                    let productId = $(this).data("id");
                    let productName = $(this).data("name");
                    let productPrice = $(this).data("price");

                    $("#input_edit_productname").val(productName);
                    $("#input_edit_productprice").val(productPrice);
                    $("#input_edit_productid").val(productId);

                    $("#editProductModal").modal("show");
                });



                $("#btn_product_update").click(function () {
                    App.addButtonSpinner($("#btn_product_update"));

                    let productId = $("#input_edit_productid").val();
                    let productNameUpdate = $("#input_edit_productname").val();
                    let productPriceUpdate = $("#input_edit_productprice").val();

                    App.requiredTextValidator($('#input_edit_productname').val(), $('#input_edit_productname'));
                    App.requiredTextValidator($('#input_edit_productprice').val(), $('#input_edit_productprice'));

                    if (productNameUpdate == "" || productPriceUpdate == "" || productPriceUpdate == 0) {
                        App.alert("error", "Name and Price is required", "Error", undefined);
                        App.removeButtonSpinner($("#btn_product_update"));
                        return;
                    }

                    let model = {
                        Id: productId,
                        Name: productNameUpdate,
                        Price: productPriceUpdate
                    }

                    App.ajaxPut(updateProductEndPoint,
                        JSON.stringify(model),
                        'text',
                        function (data) {
                            var json = JSON.parse(data);

                            if (json.isSuccessful) {
                                App.removeButtonSpinner($("#btn_product_update"));
                                $("#editProductModal").modal("hide");
                                App.alert("success", json.message, "Success", window.location.origin + "/Home/Products");
                            }
                            else {
                                App.alert("error", json.message, "Error", undefined);

                                setTimeout(function () {
                                    App.removeButtonSpinner($("#btn_product_update"));
                                }, 500);
                            }

                        },
                        function (response) {
                            console.log(response);
                        }
                    );


                });



                $(".btn-product-remove").click(function () {
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
        }
    }
}();



$(document).ready(function () {
    Product.initializeProductsTable();
});