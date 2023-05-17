
let Receipt = function () {

    return {
        printPreview: function () {
            var divToPrint = document.getElementById('divToPrint');

            var newWin = window.open('', 'Print-Window');

            newWin.document.open();

            newWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');

            newWin.document.close();

            setTimeout(function () { newWin.close(); }, 10);
        }
    }
}();



$(document).ready(function () {
    $("#btnPrintReceipt").on("click", function () {
        Receipt.printPreview();
    });
});