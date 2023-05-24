
/* Formatting function for row details - modify as you need */
function format(d) {

    let templateDropdown =
        '<table class="table-xs text-xs" cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
        '<tr colspan="2">' +
        '<td>Order Id: ' + d.Id + '</td>' +
        '</tr>';

    for (var x = 0; x < d.OrderItemList.length; x++) {
        templateDropdown += '<tr><td>ID: ' + d.OrderItemList[x].ProductId + '</td><td>Name: ' + d.OrderItemList[x].ProductName + '</td></tr><tr><td>Quantity x Price: ' + d.OrderItemList[x].Quantity + " x ₱" + d.OrderItemList[x].ProductPrice + '</td>' + '<td>Sum: ₱' + d.OrderItemList[x].TotalAmount + '</td></tr>';
    }

    templateDropdown += '</table>';

    return templateDropdown;
}

let Order = function () {
    let ordersEndPoint = "/Order/GetAllOrders";
    let updateOrderEndPoint = "/Order/Update";
    let removeOrderEndPoint = "/Order/Remove";
    let addOrderEndPoint = "/Order/Add";

    return {
        initializeOrdersTable: function () {

            var tableOrders = $('#orders_table').DataTable({
                ajax:
                {
                    url: window.location.origin + ordersEndPoint,
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
                columns: [
                    {
                        className: 'dt-control',
                        orderable: false,
                        data: null,
                        defaultContent: '',
                    },
                    {
                        data: 'CreatedDate',
                        render: function (data) {
                            return moment(data).format('MMMM Do YYYY, h:mm:ss a');
                        } 

                    },
                    { data: 'CashierName' },
                    {
                        data: 'TotalAmount',
                        render: function (data) {
                            return '₱' + data;
                        }                    },
                    {
                        data: 'Id',
                        render: function (data, type, row) {
                            return '<button type="button" class="btn btn-danger btn-xs btnOrderRemove" data-id=' + data + '><i class="fa-solid fa-trash"></i></button>'
                        }
                    }
                ],
                order: [[1, 'desc']]
            });

            // Add event listener for opening and closing details
            $('#orders_table tbody').on('click', 'td.dt-control', function () {
                var tr = $(this).closest('tr');
                var row = tableOrders.row(tr);

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


            $('#orders_table').on('init.dt', function () {
                //$(".btnOrderEdit").prop("onclick", null).off("click");
                //$(".btnOrderEdit").on("click", function () {
                //    let orderId = $(this).data("id");
                //    let orderName = $(this).data("name");
                //    let orderPrice = $(this).data("price");

                //    $("#input_edit_ordername").val(orderName);
                //    $("#input_edit_orderprice").val(orderPrice);
                //    $("#input_edit_orderid").val(orderId);

                //    $("#editOrderModal").modal("show");
                //});


                //$("#btn_order_update").prop("onclick", null).off("click");
                //$("#btn_order_update").click(function () {
                //    App.addButtonSpinner($("#btn_order_update"));

                //    let orderId = $("#input_edit_orderid").val();
                //    let orderNameUpdate = $("#input_edit_ordername").val();
                //    let orderPriceUpdate = $("#input_edit_orderprice").val();

                //    App.requiredTextValidator($('#input_edit_ordername').val(), $('#input_edit_ordername'));
                //    App.requiredTextValidator($('#input_edit_orderprice').val(), $('#input_edit_orderprice'));

                //    if (orderNameUpdate == "" || orderPriceUpdate == "" || orderPriceUpdate == 0) {
                //        App.alert("error", "Name and Price is required", "Error", undefined);
                //        App.removeButtonSpinner($("#btn_order_update"));
                //        return;
                //    }

                //    let model = {
                //        Id: orderId,
                //        Name: orderNameUpdate,
                //        Price: orderPriceUpdate
                //    }

                //    App.ajaxPut(updateOrderEndPoint,
                //        JSON.stringify(model),
                //        'text',
                //        function (data) {
                //            var json = JSON.parse(data);

                //            if (json.isSuccessful) {
                //                App.removeButtonSpinner($("#btn_order_update"));
                //                $("#editOrderModal").modal("hide");
                //                App.alert("success", json.message, "Success", window.location.origin + "/Home/Orders");
                //            }
                //            else {
                //                App.alert("error", json.message, "Error", undefined);

                //                setTimeout(function () {
                //                    App.removeButtonSpinner($("#btn_order_update"));
                //                }, 500);
                //            }

                //        },
                //        function (response) {
                //            console.log(response);
                //        }
                //    );


                //});

                $(".btnOrderRemove").prop("onclick", null).off("click");
                $(".btnOrderRemove").on("click", function () {
                    let orderId = $(this).data("id");

                    App.confirmDialogueModal("Message",
                        "Are you sure you want to delete this order?",
                        "bg-warning",
                        Order.deleteOrder(orderId),
                        undefined,
                        undefined
                    );

                   
                });
            });
        },
        deleteOrder: function (orderId) {
            let param = {
                Id: orderId
            }

            App.ajaxPost(removeOrderEndPoint,
                JSON.stringify(param),
                'text',
                function (data) {
                    var json = JSON.parse(data);

                    if (json.isSuccessful) {
                        App.alert("success", json.message, "Success", window.location.origin + "/Home/Orders");

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
    Order.initializeOrdersTable();
});