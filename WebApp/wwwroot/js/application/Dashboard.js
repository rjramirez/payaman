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
        let num = $(this).val().length;
        if (num >= 2) {
            Dashboard.executeSearch();
        }
    });

    $('#input_search_bar[type=search]').on('search', function () {
        Dashboard.executeSearch();
    });
});