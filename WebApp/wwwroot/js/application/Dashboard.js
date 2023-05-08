var dashboardSearchPageIndex = 1;

let Dashboard = function () {

    let searchEndPoint = "/Home/Search";
    let viewCartEndPoint = "/Home/ViewCart";


    return {
        initializeDashboard: function () {
            Dashboard.executeSearch();
            Dashboard.checkCart();
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


            $("#btnViewCart").click(function (e) {
                e.preventDefault();
                console.log("View cart Clicked");
                $("#viewCartModal").modal("show");
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

    $("#span_count_cart_items").click(function () {

        //public int Id { get; set; }
        //public int CashierId { get; set; }
        //public decimal TotalAmount { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public DateTime ModifiedDate { get; set; }
        //public string TransactionBy { get; set; }
        //public bool Active { get; set; }
        //public IEnumerable < OrderItemDetail > OrderItemList { get; set; }

        //public int Id { get; set; }
        //public int ProductId { get; set; }
        //public string ProductName { get; set; }
        //public decimal ProductPrice { get; set; }
        //public int Quantity { get; set; }
        //public decimal TotalAmount { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public DateTime ModifiedDate { get; set; }
        //public string TransactionBy { get; set; }
        //public bool Active { get; set; }

        //Get existing cartItems if available
        let cartItems = JSON.parse(sessionStorage.getItem('cartItems'));


        let orderDetails = {
            TotalAmount: 0
        };

        let orderItemListArr = [];

        for (var index = 0; index < cartItems.length; ++index) {
            orderDetails.TotalAmount += parseInt(cartItems[index].TotalAmount);
        }


        //build orderItemsList
        for (var index = 0; index < cartItems.length; ++index) {
            var cart = cartItems[index];

            let orderItem = {
                ProductId: cart.ProductId,
                ProductName: cart.ProductName,
                ProductPrice: cart.ProductPrice,
                Quantity: cart.Quantity,
                TotalAmount: cart.TotalAmount
            };

            orderItemListArr.push(orderItem);
        }

        console.log(orderDetails);


        App.ajaxPost(viewCartEndPoint,
            JSON.stringify(orderDetails),
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
    
});