var errorSearchPagIndex = 1;
$(document).ready(function () {
    ErrorLog.initialize();
});

let ErrorLog = function () {
    let searchEndPoint = "/error/search";

    return {
        initialize: function () {
            App.dateRangePicker("#txtDateRange");

            $("#btnSearch").click(function () {
                errorSearchPagIndex = 1;
                ErrorLog.executeSearch();
            });

            $("#btnClearInput").click(function () {
                ErrorLog.clearInputs();
            });
        },

        changePage: function (pageId) {
            errorSearchPagIndex = pageId;
            ErrorLog.executeSearch();
        },
        executeSearch: function () {
            let startDate = $("#txtDateRange").val() != "" ? $("#txtDateRange").data('daterangepicker').startDate.format(AppConstant.queryStringDateTimeFormat) : "";
            let endDate = $("#txtDateRange").val() != "" ? $("#txtDateRange").data('daterangepicker').endDate.format(AppConstant.queryStringDateTimeFormat) : "";
            let searchKeyword = $("#txtSearchKeyword").val();

            let url = searchEndPoint + `?PageNumber=${errorSearchPagIndex}`;
            url += `&StartDate=${startDate}&EndDate=${endDate}`;
            url += `&SearchKeyword=${searchKeyword}`;

            let searchurl = window.location.origin + url;
            window.location.replace(searchurl);
        },
        clearInputs: function () {
            $("#txtDateRange").val('');
            $("#txtSearchKeyword").val('');
        }
    }
}();