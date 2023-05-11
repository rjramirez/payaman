
let Cart = function () {
    return {
        initializeCart: function () {
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

            let tbodyTemplate = "";

            for (var x = 0; x < cartItems.length; ++x) {
                tbodyTemplate += '<tr class="tr_' + cartItems[x].ProductId + '">' +
                    '<td colspan="2">' +
                    '<p>' + cartItems[x].ProductName + '</p>' +
                    '</td>' +
                    '<td>' +
                    '<input type="number" value="' + cartItems[x].Quantity + '" class="form-control item_quantity" data-product-id="' + cartItems[x].ProductId + '">' +
                    '</td>' +
                    '<td>₱' + cartItems[x].ProductPrice + '</td>' +
                    '<td><span class="spanProductSum_' + cartItems[x].ProductId + '">₱' + cartItems[x].TotalAmount+ '</span></td>' +
                    '<td>' +
                    '<a href="" class="btnRemoveProduct" data-product-id="' + cartItems[x].ProductId + '"><i class="fa-solid fa-trash" style="color: #9e9e9e;"></i></a>' +
                    '</td>' +
                    '</tr>';
            }

            if (tbodyTemplate == "")
            {
                tbodyTemplate = '<tr>' +
                    '<td colspan="6">' +
                    '<p>No Data Available</p>' +
                    '</td>' + 
                    '</tr>';
            }

            $("#tbodyOrderItemList").html(tbodyTemplate);
            $("#spanCartTotalAmount").html("₱" + parseFloat(cartTotalAmount).toFixed(2));

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

            let checkoutCartEndPoint = window.location.origin + "/Home/Checkout";

            App.ajaxPost(checkoutCartEndPoint,
                JSON.stringify(orderDetails),
                'text',
                function (data) {
                    var json = JSON.parse(data);

                    if (json.isSuccessful && json.data != null) {
                        window.location.replace(window.location.origin + "/Home/Receipt");
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
                        $(".spanProductSum_" + productId).html("₱" + product.TotalAmount);
                    }
                    cartTotalAmount += parseFloat(product.TotalAmount);
                }

                $("#spanCartTotalAmount").html("₱" + parseFloat(cartTotalAmount).toFixed(2));
                sessionStorage.setItem("cartItems", JSON.stringify(cartItems));
            }
        },
        removeItem: function (productId) {

            //Get existing cartItems if available
            let cartItems = JSON.parse(sessionStorage.getItem('cartItems'));

            if (sessionStorage.cartItems) {
                let cartTotalAmount = 0.00;
                for (var index = 0; index < cartItems.length; ++index) {
                    var product = cartItems[index];
                    if (product.ProductId == productId) {
                        cartItems.splice(index, 1);
                    }
                    cartTotalAmount += parseFloat(product.TotalAmount);
                }


                $("#spanCartTotalAmount").html("₱" + parseFloat(cartTotalAmount).toFixed(2));

                sessionStorage.setItem("cartItems", JSON.stringify(cartItems));

                $(".tr_" + productId).remove();

                let tbodyTemplate = $("#tbodyOrderItemList").html();
                if (tbodyTemplate == "") {
                    tbodyTemplate = '<tr>' +
                        '<td colspan="6">' +
                        '<p>No Data Available</p>' +
                        '</td>' +
                        '</tr>';
                    $("#tbodyOrderItemList").html(tbodyTemplate);
                }
            }
        }
    }
}();



$(document).ready(function () {
    Cart.initializeCart();


    $(".item_quantity").change(function () {
        let productId = $(this).data("product-id");
        let oldVal = $(this).data("old-value");
        let newVal = $(this).val();

        Cart.changeQuantityItem(productId, newVal);
         
    });


    $(".btnRemoveProduct").click(function (e) {
        e.preventDefault();
        let productId = $(this).data("product-id");
        Cart.removeItem(productId);
    });

    $("#btnCartCheckout").click(function () {
        Cart.checkOutCart();
    });
});