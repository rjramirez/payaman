var employeeSearchPageIndex = 1;



/* Formatting function for row details - modify as you need */
function format(d) {
    // `d` is the original data object for the row
    return (
        '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
        '<tr>' +
        '<td>Full name:</td>' +
        '<td>' +
        d.name +
        '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Extension number:</td>' +
        '<td>' +
        d.extn +
        '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Extra info:</td>' +
        '<td>And any further details here (images etc)...</td>' +
        '</tr>' +
        '</table>'
    );
}

$(document).ready(function () {
    var table = $('#example').DataTable({
        ajax: '../ajax/data/objects.txt',
        columns: [
            {
                className: 'dt-control',
                orderable: false,
                data: null,
                defaultContent: '',
            },
            { data: 'name' },
            { data: 'position' },
            { data: 'office' },
            { data: 'salary' },
        ],
        order: [[1, 'asc']],
    });

    // Add event listener for opening and closing details
    $('#example tbody').on('click', 'td.dt-control', function () {
        var tr = $(this).closest('tr');
        var row = table.row(tr);

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
});


let Product = function () {
    let userEndPoint = "/Home/Login";
    let registerEndPoint = "/Home/Register";

    return {
        initialize: function () {

            $("#btn_login").click(function () {
                Login.authenticate();
            });

            $("#btn_signup_modal").click(function () {
                $("#signUpModal").modal("show");
            });

            $("#btn_signup").click(function () {
                Login.register();
            });
        },
        executeSearch: function () {

            $("#search-employeebtn").attr("disabled", true);
            $("#modalheader-sicklinetracker").text("Employee Finder - Searching...");

            let Keyword = $("#inpSearchEmpNum").val();
            let ProjectId = $("#ddlProject").val();
            let EmployeeSearchEndpoint = "/ControlCenter/EmployeeSearch";
            let url = EmployeeSearchEndpoint + `?PageNumber=${employeeSearchPageIndex}`;

            if (Keyword != "")
                url += `&Keyword=${Keyword}`;

            if (ProjectId != "")
                url += `&Price=${ProjectId}`;

            console.log(employeeSearchPageIndex);
            App.ajaxGet(url
                , "html"
                , function (data) {
                    $("#employee-search-container").html("");
                    $("#employee-search-container").html(data);

                    $(".row-employee").on("click", function (e) {
                        console.log($(this).attr("data-employeeId"));
                        inpEmployeeNumber = $(this).attr("data-employeeId");
                        $("#employeeno").val($(this).attr("data-employeeId"));
                        $("#btnCloseModalSicklineTracker").trigger("click")
                        ControlCenter.getHistory();
                    });
                    setTimeout(function () {
                        $("#search-employeebtn").attr("disabled", false);
                        $("#modalheader-sicklinetracker").text("Employee Finder");
                    }, 500);
                }
                , function (response) {
                    console.log(response);
                    setTimeout(function () {
                        $("#modalheader-sicklinetracker").text("Employee Finder");
                    }, 500);

                }
            );


        },
        employeeChangePage: function (pageId) {
            employeeSearchPageIndex = pageId;
            ControlCenter.executeSearch();
        },
        clearInputs: function () {
            $("#txtDateRange").val('');
            $("#txtSearchKeyword").val('');
        }
    }
}();


