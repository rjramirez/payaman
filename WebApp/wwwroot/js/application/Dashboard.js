var dashboardSearchPageIndex = 1;

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

let Dashboard = function () {
    let ordersEndPoint = "/Order/GetAllOrders";
    let updateOrderEndPoint = "/Order/Update";
    let removeOrderEndPoint = "/Order/Remove";
    let addOrderEndPoint = "/Order/Add";
    let searchEndPoint = "/Home/Search";


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
                dom: "Bfrtip",
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
                            return '<button type="button" class="btn btn-success btn-xs btn-order-edit" data-id="' + data + '"'
                                + 'data-name="' + row.Name + '"'
                                + 'data-price="' + row.Price + '"'
                                + '><i class="fa-solid fa-pencil"></i></button> | <button type="button" class="btn btn-danger btn-xs btn-order-remove" data-id=' + data + '><i class="fa-solid fa-trash"></i></button>'
                        }
                    }
                ],
                order: [[1, 'asc']]
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

                $(".btn-order-edit").click(function () {
                    let orderId = $(this).data("id");
                    let orderName = $(this).data("name");
                    let orderPrice = $(this).data("price");

                    $("#input_edit_ordername").val(orderName);
                    $("#input_edit_orderprice").val(orderPrice);
                    $("#input_edit_orderid").val(orderId);

                    $("#editOrderModal").modal("show");
                });



                $("#btn_order_update").click(function () {
                    App.addButtonSpinner($("#btn_order_update"));

                    let orderId = $("#input_edit_orderid").val();
                    let orderNameUpdate = $("#input_edit_ordername").val();
                    let orderPriceUpdate = $("#input_edit_orderprice").val();

                    App.requiredTextValidator($('#input_edit_ordername').val(), $('#input_edit_ordername'));
                    App.requiredTextValidator($('#input_edit_orderprice').val(), $('#input_edit_orderprice'));

                    if (orderNameUpdate == "" || orderPriceUpdate == "" || orderPriceUpdate == 0) {
                        App.alert("error", "Name and Price is required", "Error", undefined);
                        App.removeButtonSpinner($("#btn_order_update"));
                        return;
                    }

                    let model = {
                        Id: orderId,
                        Name: orderNameUpdate,
                        Price: orderPriceUpdate
                    }

                    App.ajaxPut(updateOrderEndPoint,
                        JSON.stringify(model),
                        'text',
                        function (data) {
                            var json = JSON.parse(data);

                            if (json.isSuccessful) {
                                App.removeButtonSpinner($("#btn_order_update"));
                                $("#editOrderModal").modal("hide");
                                App.alert("success", json.message, "Success", window.location.origin + "/Home/Orders");
                            }
                            else {
                                App.alert("error", json.message, "Error", undefined);

                                setTimeout(function () {
                                    App.removeButtonSpinner($("#btn_order_update"));
                                }, 500);
                            }

                        },
                        function (response) {
                            console.log(response);
                        }
                    );


                });



                $(".btn-order-remove").click(function () {
                    let orderId = $(this).data("id");

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
                });
            });
        },
        changePage: function (pageId) {
            dashboardSearchPageIndex = pageId;
            Dashboard.executeSearch();
        },
        executeSearch: function () {
            let searchKeyword = $("#input_search_bar").val();

            let url = searchEndPoint + `?PageNumber=${dashboardSearchPageIndex}`;
            url += `&Keyword=${searchKeyword}`;

            let orderSearchUrl = window.location.origin + url;
            //window.location.replace(searchurl);

            App.ajaxGet(orderSearchUrl
                , "html"
                , function (data) {
                    $("#orders_row").html(data);
                }
                ,function () {
                    App.hidePreloader();
                }
            );

        },
    }
}();



$(document).ready(function () {
    $("#input_search_bar").keyup(function () {
        let num = $(this).length;
        if (num > 2) {
            Dashboard.executeSearch();
        }
    });
});