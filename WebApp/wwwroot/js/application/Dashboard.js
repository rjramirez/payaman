var dashboardSearchPageIndex = 1;

let Dashboard = function () {

    let searchEndPoint = "/Home/Search";


    return {
        initializeDashboard: function () {
            Dashboard.executeSearch();
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
                    });

                    App.hidePreloader();
                }
                ,function () {
                    App.hidePreloader();
                }
            );

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
                        console.log("ProductID: " + item.ProductId);
                        console.log("Quantity: " + item.Quantity);
                        if (item.ProductId == productId) {
                            item.Quantity += 1;
                            item.TotalAmount = (item.ProductPrice * item.Quantity);
                        }
                        console.log("ProductID: " + item.ProductId);
                        console.log("Quantity: " + item.Quantity);
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




    $("#btnViewCart").click(function () {

        
        //span_count_cart_items
        //span_count_cart_items_header
    });
});