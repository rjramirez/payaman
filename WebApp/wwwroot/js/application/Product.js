var employeeSearchPageIndex = 1;


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

let Product = function () {
    return {
        initialize: function () {


        },

    }
}();

$(document).ready(function () {
    let productsEndPoint = "/Product/GetAllProducts";



    var table = $('#products_table').DataTable({
        ajax:
        {
            url: window.location.origin + productsEndPoint,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataSrc: function (receivedData) {
                console.log(receivedData);
                return receivedData;
            },
        },
        columns: [
            {
                className: 'dt-control',
                orderable: false,
                data: null,
                defaultContent: '',
            },
            { data: 'Name'},
            {
                data: 'Price',
                render: function (data) {
                    return '₱' + data;
                }
            },
            {
                data: 'Id', render: function (data, type, row) {
                    return '<button class="btn btn-success btn-xs"><i class="fa-solid fa-plus"></i></button> | <button class="btn btn-danger btn-xs"><i class="fa-solid fa-trash"></i></button>'
                }
            }
        ],
        order: [[1, 'asc']],
    });

    // Add event listener for opening and closing details
    $('#products_table tbody').on('click', 'td.dt-control', function () {
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





