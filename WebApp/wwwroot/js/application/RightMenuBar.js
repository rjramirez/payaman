var dashboardSearchPageIndex = 1;

let RightMenuBar = function () {

    let productsEndPoint = "/Product/GetAllProducts";
    let searchEndPoint = "/Home/Search";
    let storesEndPoint = "/Home/MenuRightBarStores";

    return {
        resetTable: function () {
            // Quickly and simply clear a table
            $('#products_table').dataTable().fnClearTable();

            // Restore the table to it's original state in the DOM by removing all of DataTables enhancements, alterations to the DOM structure of the table and event listeners
            $('#products_table').dataTable().fnDestroy();
        },
        initializeProductsTable: function (storeId) {

            //RightMenuBar.resetTable();

            var tableProducts = $('#products_table').DataTable({
                ajax:
                {
                    url: window.location.origin + productsEndPoint + "/" + storeId,
                    type: 'GET',
                    contentType: 'application/json; charset=utf-8',
                    dataSrc: function (receivedData) {
                        //console.log(receivedData);
                        return receivedData;
                    },
                },
                destroy: true,
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
                                + 'data-store-id="' + row.StoreId + '"'
                                + '><i class="fa-solid fa-pencil"></i></button> | <button type="button" class="btn btn-danger btn-xs btnProductRemove" data-id=' + data + ' data-product-name="' + row.Name + '""><i class="fa-solid fa-trash"></i></button>'
                        }
                    }
                ],
                order: [[1, 'asc']]
            });

            // Add event listener for opening and closing details
            $("#products_table tbody").prop("onclick", null).off("click");
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
                    let productStoreId = $(this).data("store-id");

                    $("#input_edit_productname").val(productName);
                    $("#input_edit_productprice").val(productPrice);
                    $("#input_edit_productid").val(productId);
                    $('#selectStoreName option[value="' + productStoreId + '"]').attr("selected", "selected");

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
                        StoreId: productStoreId
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

                $("#btnConfirmRemoveProduct").prop("onclick", null).off("click");
                $("#btnConfirmRemoveProduct").click(function () {
                    let productId = $(this).data("product-id");

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

                $(".btnProductRemove").prop("onclick", null).off("click");
                $(".btnProductRemove").click(function () {

                    let productId = $(this).data("id");
                    let productName = $(this).data("product-name");
                    $("#btnConfirmRemoveProduct").attr("data-product-id", productId);
                    $("#btnConfirmRemoveProduct").attr("data-product-name", productName);

                    $("#productNameToDelete").html(productName);
                    $("#confirmRemoveProductModal").modal("show");
                });
            });



            //Populate Store names in Modal
            $("#selectStoreName").html();
        },
        menuSetup: function (storeId) {
            let url = "";

            if (storeId > 0)
                url = storesEndPoint + `?Id=${storeId}`;
            else
                url = storesEndPoint + `?Id=${0}`;

            //Menu Right Navbar
            let storesUrl = window.location.origin + url;
            App.ajaxGet(storesUrl
                , "html"
                , function (data) {
                    var jsonStore = JSON.parse(data);
                    $("#spanStoreCount").html(jsonStore.length);

                    let btnStoreTemplate = "";

                    for (var x = 0; x < jsonStore.length; x++)
                    {
                        if (jsonStore[x].isSelected == true) {
                            btnStoreTemplate += '<button type="button" class="d-none dropdown-item store_names btn-link"' +
                                'data-store-id="' + jsonStore[x].id + '"' +
                                'data-store-name="' + jsonStore[x].name + '"' +
                                'data-store-address="' + jsonStore[x].address + '">' +
                                jsonStore[x].name +
                                '</button>';
                        }
                        else
                        {
                            btnStoreTemplate += '<button type="button" class="dropdown-item store_names btn-link"' +
                                'data-store-id="' + jsonStore[x].id + '"' +
                                'data-store-name="' + jsonStore[x].name + '"' +
                                'data-store-address="' + jsonStore[x].address + '">' +
                                jsonStore[x].name +
                                '</button>';
                        }
                    }

                    $("#divBtnStoreContainer").html(btnStoreTemplate);

                    jsonStore.forEach(function (i, d) {
                        if (i.isSelected == true)
                        {
                            $("#spanStoreNameSelected").html(i.name);
                            $("#spanStoreNameSelected").data('store-id', i.id);
                            return;
                        }
                    });

                    $(".store_names").prop("onclick", null).off("click");
                    $(".store_names").click(function () {
                        let storeId = $(this).data("store-id");
                        let storeName = $(this).data("store-name");
                        let storeAddress = $(this).data("store-address");

                        //let storeVM = {
                        //    Id: storeId,
                        //    Name: storeName,
                        //    Address: storeAddress
                        //};

                        RightMenuBar.menuSetup(storeId);

                        
                        var url = window.location.pathname.split("/");

                        if (url[1] == 'Home' && url[2] == "Index") {
                            Dashboard.executeSearch(storeId);
                        }
                        else if (url[1] == 'Home' && url[2] == "Products") {
                            Product.initializeProductsTable(storeId);
                        }
                            
                    });

                    App.hidePreloader();
                }
                , function () {
                    App.hidePreloader();
                }
            );
        },
        checkCart: function () {
            let cartItems = JSON.parse(sessionStorage.getItem('cartItems'));
            if (cartItems == null)
                return;

            if (sessionStorage.cartItems) {
                let cartItemCount = 0;
                for (var index = 0; index < cartItems.length; ++index) {
                    var product = cartItems[index];
                    if (product.Quantity > 0) {
                        $("#item_" + product.ProductId + "_quantity").html(product.Quantity);
                        cartItemCount += parseInt(product.Quantity);
                    }

                }
                $("#span_count_cart_items").html(cartItemCount);
            }

            $("#icon_cart").prop("onclick", null).off("click");
            $("#icon_cart").click(function (e) {
                e.preventDefault();

                window.location.replace(window.location.origin + "/Home/Cart");
            });
        },
        addItemToCart: function (productId, productName, productPrice) {

            //Get existing cartItems if available
            let cartItems = JSON.parse(sessionStorage.getItem('cartItems'));

            if (sessionStorage.cartItems) {
                //Check if product is existing in the cartItems
                var existInCartItems = false;

                for (var index = 0; index < cartItems.length; ++index) {
                    var product = cartItems[index];
                    if (product.ProductId == productId) {
                        existInCartItems = true;
                        break;
                    }
                }

                if (existInCartItems) {
                    //Update cartItems
                    $.each(cartItems, function (i, item) {
                        if (item.ProductId == productId) {
                            item.Quantity += 1;
                            item.TotalAmount = (item.ProductPrice * item.Quantity);
                        }
                    });

                    sessionStorage.setItem("cartItems", JSON.stringify(cartItems));
                }
                else
                {
                    //Add new product in the cart
                    let cartDetail = {
                        ProductId: productId,
                        ProductName: productName,
                        ProductPrice: productPrice,
                        TotalAmount: productPrice, 
                        Quantity: 1
                    };

                    cartItems.push(cartDetail);

                    sessionStorage.setItem("cartItems", JSON.stringify(cartItems));
                }
            }
            else
            {
                //Create cartItems for the first time
                let cartObj = [];
                let cartDetail = {
                    ProductId: productId,
                    ProductName: productName,
                    ProductPrice: productPrice,
                    TotalAmount: productPrice,
                    Quantity: 1
                };

                cartObj.push(cartDetail);

                sessionStorage.setItem("cartItems", JSON.stringify(cartObj));
            }

            let existingQuant = parseInt($("#item_" + productId + "_quantity").html()) ? parseInt($("#item_" + productId + "_quantity").html()) : 0;
            $("#item_" + productId + "_quantity").html(existingQuant + 1);
        }
    }
}();



$(document).ready(function () {
    let storeId = $(".d-none.store_names").data("store-id");

    RightMenuBar.menuSetup(storeId);
});