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
                    App.hidePreloader();
                }
                ,function () {
                    App.hidePreloader();
                }
            );

        },
    }
}();



$(document).ready(function () {

    Dashboard.initializeDashboard();

    $("#input_search_bar").keyup(function () {
        let num = $(this).length;
        if (num > 2) {
            Dashboard.executeSearch();
        }
    });
});