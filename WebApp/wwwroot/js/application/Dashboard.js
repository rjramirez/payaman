var dashboardSearchPageIndex = 1;

let Dashboard = function () {

    let searchEndPoint = "/Home/Search";
    let storesEndPoint = "/Home/MenuRightBarStores";

    return {
        initializeDashboard: function () {
            Dashboard.menuSetup();
            Dashboard.executeSearch();
            //Dashboard.checkCart();
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


                    $(".btn_order_items").click(function () {
                        let productId = $(this).data("product-id");
                        let productName = $(this).data("product-name");
                        let productPrice = $(this).data("product-price");

                        Dashboard.addItemToCart(productId, productName, productPrice);

                        Dashboard.checkCart();

                    });

                    Dashboard.checkCart();

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
                url = storesEndPoint + `?StoreId=${storeId}`;
            else
                url = storesEndPoint + `?StoreId=${0}`;

            //Menu Right Navbar
            let storesUrl = window.location.origin + url;
            App.ajaxGet(storesUrl
                , "html"
                , function (data) {
                    $("#ul_right_navbar").html(data);

                    $(".store_names").click(function () {
                        let storeId = $(this).data("store-id");
                        let storeName = $(this).data("store-name");
                        let storeAddress = $(this).data("store-address");

                        let storeVM = {
                            Id: storeId,
                            Name: storeName,
                            Address: storeAddress
                        };

                        App.menuSetup(storeVM);

                        //Search dashboard with the storeId selected
                        var pathname = window.location.pathname;
                        var dashboardPage = "Home/Index";
                        if (pathname.indexOf(dashboardPage) != -1) {
                            App.executeSearch(storeId);
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

                //Get existing cartItems if available
                let cartItems = JSON.parse(sessionStorage.getItem('cartItems'));

                let orderItemListArr = [];

                //build orderItemsList
                for (var index = 0; index < cartItems.length; ++index) {
                    var cart = cartItems[index];

                    let orderItem = {
                        ProductId: cart.ProductId,
                        ProductName: cart.ProductName,
                        ProductPrice: cart.ProductPrice,
                        Quantity: cart.Quantity,
                        TotalAmount: (parseFloat(cart.TotalAmount).toFixed(2))
                    };

                    orderItemListArr.push(orderItem);
                }

                let cartTotalAmount = 0.00;

                for (var index = 0; index < cartItems.length; ++index) {
                    cartTotalAmount += parseFloat(cartItems[index].TotalAmount);
                }


                let orderDetails = {
                    TotalAmount: cartTotalAmount,
                    OrderItemList: orderItemListArr
                };

                let viewCartEndPoint = window.location.origin + "/Home/ViewCart";

                App.ajaxPost(viewCartEndPoint,
                    JSON.stringify(orderDetails),
                    'text',
                    function (data) {
                        var json = JSON.parse(data);

                        if (json.isSuccessful && json.data != null) {
                            window.location.replace(window.location.origin + "/Home/Cart");
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

    Dashboard.initializeDashboard();

    $("#input_search_bar").keyup(function () {
        let num = $(this).val().length;
        if (num >= 2) {
            Dashboard.executeSearch();
        } else if (num == 0) {
            Dashboard.executeSearch();
        }
    });

    $('#input_search_bar[type=search]').on('search', function () {
        Dashboard.executeSearch();
    });


    
});