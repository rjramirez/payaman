
let Cart = function () {
    return {
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
                else {
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
            else {
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
        },
        removeItemToCart: function (productId, productName, productPrice) {

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
                else {
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
            else {
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
        },
        checkOutCart: function () {
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
        },
        changeQuantityItem: function (productId, quantity) {

            //Get existing cartItems if available
            let cartItems = JSON.parse(sessionStorage.getItem('cartItems'));

            if (sessionStorage.cartItems) {
                let cartTotalAmount = 0.00;
                for (var index = 0; index < cartItems.length; ++index) {
                    var product = cartItems[index];
                    if (product.ProductId == productId) {
                        product.Quantity = parseInt(quantity);
                        product.TotalAmount = parseFloat(parseFloat(product.ProductPrice) * parseFloat(product.Quantity)).toFixed(2);
                        sessionStorage.setItem("cartItems", JSON.stringify(cartItems));
                        $(".spanProductSum_" + productId).html("₱" + product.TotalAmount);
                    }
                    cartTotalAmount += parseFloat(product.TotalAmount);
                    console.log(product.TotalAmount)
                }

                $("#spanCartTotalAmount").html("₱" + parseFloat(cartTotalAmount).toFixed(2));
            }


        }
    }
}();



$(document).ready(function () {
    $(".item_quantity").change(function () {
        let productId = $(this).data("product-id");
        let oldVal = $(this).data("old-value");
        let newVal = $(this).val();

        Cart.changeQuantityItem(productId, newVal);
         
    });

    $("#btnCartCheckout").click(function () {

    });
});