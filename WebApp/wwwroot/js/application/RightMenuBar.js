var dashboardSearchPageIndex = 1;

let RightMenuBar = function () {

    let searchEndPoint = "/Home/Search";
    let storesEndPoint = "/Home/MenuRightBarStores";

    return {
        initializeRightMenuBar: function () {
            RightMenuBar.menuSetup();
            RightMenuBar.executeSearch();
        },
        executeSearch: function (storeId) {
            let searchKeyword = $("#input_search_bar").val() ? $("#input_search_bar").val() : "";

            let url = searchEndPoint + `?PageNumber=${dashboardSearchPageIndex}`;
            url += `&Keyword=${searchKeyword}`;
            if (storeId > 0)
                url += `&StoreId=${storeId}`;

            let orderSearchUrl = window.location.origin + url;
            //window.location.replace(searchurl);

            App.ajaxGet(orderSearchUrl
                , "html"
                , function (data) {
                    $("#orders_row").html(data);

                    $(".btn_order_items").prop("onclick", null).off("click");
                    $(".btn_order_items").click(function () {
                        let productId = $(this).data("product-id");
                        let productName = $(this).data("product-name");
                        let productPrice = $(this).data("product-price");

                        RightMenuBar.addItemToCart(productId, productName, productPrice);

                        RightMenuBar.checkCart();

                    });

                    RightMenuBar.checkCart();

                    App.hidePreloader();
                }
                ,function () {
                    App.hidePreloader();
                }
            );

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

                    let storeNameSelected = "";
                    jsonStore.forEach(function (i, d) {
                        if (i.isSelected == true)
                        {
                            storeNameSelected = i.name;
                            return;
                        }
                    });

                    $("#spanStoreNameSelected").html(storeNameSelected);

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

                        RightMenuBar.executeSearch(storeId);
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
    RightMenuBar.initializeRightMenuBar();
});