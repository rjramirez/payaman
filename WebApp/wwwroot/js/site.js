$(document).ready(function () {
    App.initialize();
    if (window.localStorage.getItem("toastrMsg") !== null) {
        let toastrMsg = JSON.parse(window.localStorage.getItem("toastrMsg"));
        App.alert(toastrMsg.type, toastrMsg.message, toastrMsg.title);
        window.localStorage.clear();
    }

    $('[data-widget="pushmenu"]').PushMenu('collapse');
    $(document).on('shown.lte.pushmenu', function () {
        $('[data-widget="pushmenu"]').css("margin-left", 250 + "px");
    });

    $(document).on('collapsed.lte.pushmenu', function () {
        $('[data-widget="pushmenu"]').css("margin-left", 0 + "px");
    });


    $('.nav-sidebar .nav-item').click(function () {
        $('.nav-sidebar .nav-item').removeClass('active');
        $(this).addClass('active');
    });


});

const AppConstant = {
    dateFormat: "MM/DD/YYYY",
    queryStringDateTimeFormat: "D MMMM YYYY hh:mm:ss A"
};

const monthNames = ["January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"]

var App = function () {
    let storesEndPoint = "/Home/MenuRightBarStores";
    let searchEndPoint = "/Home/Search";

    return {

        initialize: function () {
            toastr.options = {
                "closeButton": true,
                "debug": false,
                "newestOnTop": false,
                "progressBar": true,
                "positionClass": "toast-top-right",
                "preventDuplicates": false,
                "onclick": null,
                "showDuration": "500",
                "hideDuration": "1000",
                "timeOut": "10000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }

            App.menuSetup();
            
        },
        executeSearch: function (storeId) {
            let searchKeyword = $("#input_search_bar").val();

            let url = searchEndPoint + `?PageNumber=${dashboardSearchPageIndex}`;
            url += `&Keyword=${searchKeyword}`;
            url += `&StoreId=${storeId}`;

            let orderSearchUrl = window.location.origin + url;
            //window.location.replace(searchurl);

            App.ajaxGet(orderSearchUrl
                , "html"
                , function (data) {
                    $("#orders_row").html(data);
                    App.hidePreloader();
                }
                , function () {
                    App.hidePreloader();
                }
            );

        },
        menuSetup: function (storeId) {

            let url = storesEndPoint + `?StoreId=${storeId}`;

            //Menu Right Navbar
            let storesUrl = window.location.origin + url;
            App.ajaxGet(storesUrl
                , "html"
                , function (data) {
                    $("#ul_right_navbar").html(data);

                    $(".store_names").click(function () {
                        let storeId = $(this).data("store-id");

                        App.menuSetup(storeId);


                        //Search dashboard with the storeId selected
                        var pathname = window.location.pathname; 
                        var dashboardPage = "Home/Index";
                        if (pathname.indexOf(dashboardPage) != -1) {
                            App.executeSearch(storeId);
                        }
                    });

                    App.hidePreloader();
                }
                , function () {
                    App.hidePreloader();
                }
            );
        },
        ajaxPost: function (_url, _data, _dataType, _successFn, _errorFn) {
            App.ajaxCall('POST', _url, _data, _dataType, _successFn, _errorFn);
        },
        ajaxPut: function (_url, _data, _dataType, _successFn, _errorFn) {
            App.ajaxCall('PUT', _url, _data, _dataType, _successFn, _errorFn);
        },
        ajaxGet: function (_url, _dataType, _successFn, _errorFn) {
            $.ajax({
                url: _url,
                type: 'GET',
                dataType: _dataType,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    App.ajaxSuccess(_successFn, data);
                },
                error: function (jqXHR, textStatus) {
                    App.ajaxError(jqXHR, textStatus, _errorFn);
                }
            });
        },
        ajaxCall: function (_methodType, _url, _data, _dataType, _successFn, _errorFn) {
            $.ajax({
                url: _url,
                type: _methodType,
                data: _data,
                dataType: _dataType,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    App.ajaxSuccess(_successFn, data);
                },
                error: function (jqXHR, textStatus) {
                    App.ajaxError(jqXHR, textStatus, _errorFn);
                }
            });
        },
        ajaxSuccess: function (_successFn, _data,) {
            if (_successFn != undefined) {
                _successFn(_data);
            }
        },
        ajaxError: function (_jqXHR, _textStatus, _errorFn) {
            App.hidePreloader();
            if (_errorFn != undefined) {
                _errorFn(_jqXHR);
            }
            else if (_textStatus != 'parsererror' && _jqXHR.status != 500) {
                App.alert("error", 'Error', _jqXHR.responseText);
            }
            else {
                App.alert("error", 'Unexpected error occur', 'Ooops!');
            }
        },
        alert: function (_type, _message, _title, _urlRedirect) {
            /*_type: value should be any of the following: success, warning, info, error
             _title: optional if you dont want to show title for the alert box
             _urlRedirect: used to delay the message after redirect to URL*/
            if (_urlRedirect != undefined) {
                localStorage.setItem("toastrMsg",
                    JSON.stringify({
                        type: _type,
                        title: _title,
                        message: _message
                    }));

                window.location.replace(_urlRedirect);
            }
            else {
                if (_title != undefined) {
                    window.toastr[_type](_message, _title);
                }
                else {
                    window.toastr[_type](_message);
                }
            }
        },
        addButtonSpinner: function (buttonId) {
            $(buttonId).attr("data-original-text", $(buttonId).html());
            $(buttonId).prop("disabled", true);
            $(buttonId).html('<i class="spinner-border spinner-border-sm"></i> Loading...');
        },
        removeButtonSpinner: function (buttonId) {
            $(buttonId).prop("disabled", false);
            $(buttonId).html($(buttonId).attr("data-original-text"));
        },
        addBoxSpinner: function (boxId) {
            let boxSpinnerElement = '<div class="overlay"><span class="fa fa-sync-alt fa-spin"></span></div>';
            $(boxId).prepend(boxSpinnerElement);
        },
        removeBoxSpinner: function (boxid) {
            $(boxid).find('.overlay:first-child').remove();
        },
        showValidationMessage: function (validationMessage) {
            if (validationMessage.length > 0) {
                let message = "";

                for (let value of validationMessage) {
                    message += value + "<br/>";
                }

                App.alert("error", message, "Validation Summary");
            }
        },
        showPreloader: function () {
            let preloader = $("#divPreloader");
            preloader.removeAttr("style");
            preloader.children().removeAttr("style");
        },
        hidePreloader: function () {
            setTimeout(function () {
                let preloader = $("#divPreloader");

                if (preloader) {
                    preloader.css('height', 0);
                    setTimeout(function () {
                        preloader.children().hide();
                    }, 200);
                }
            }, 200);
        },

        dateSinglePicker: function (controlSelector) {
            $(controlSelector).daterangepicker({
                singleDatePicker: true,
                showDropdowns: true,
                minYear: 1901
            });
        },

        checkboxvalue: function (controlSelector) {
            $(controlSelector).on('change', function () {
                if ($(this).is(':checked')) {
                    $(this).val('true');
                } else {
                    $(this).val('false');
                }
            });
        },

        dateRangePicker: function (controlSelector) {
            $(controlSelector).daterangepicker({
                timePicker: false,
                autoUpdateInput: false,
                locale: {
                    cancelLabel: 'Clear',
                    format: AppConstant.dateFormat
                },
                ranges: {
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
                    'This Year': [moment().startOf('year'), moment().endOf('year')]
                }
            });
            $(controlSelector).on('apply.daterangepicker', function (ev, picker) {
                $(this).val(picker.startDate.format(AppConstant.dateFormat) + ' - ' + picker.endDate.format(AppConstant.dateFormat));
            });
        },
        multiSelect2: function (controlSelector) {
            $(controlSelector).select2({
                theme: 'bootstrap4',
                width: '100%',
            });

            $(controlSelector).on("select2:unselect", function (evt) {
                if (!evt.params.originalEvent) {
                    return;
                }

                evt.params.originalEvent.stopPropagation();
            });
        },
        singleSelect2: function (controlSelector, hideSearchBox) {
            if (hideSearchBox) {
                $(controlSelector).select2({
                    theme: 'bootstrap4',
                    width: '100%',
                    minimumResultsForSearch: 'Infinity'
                });
            }
            else {
                $(controlSelector).select2({
                    theme: 'bootstrap4',
                    width: '100%'
                });
            }
        },
        requiredTextValidator: function (value, control) {
            if (value == "") {
                //validationMessages.push(message);
                control.addClass("is-invalid");
            }
            else {
                control.removeClass("is-invalid");
            }
            //return validationMessages;
        },
        requiredSingleSelectValidator: function (value, control) {
            if (value == "" || value == 0) {
                //validationMessages.push(message);
                control.addClass("is-invalid");
            }
            else {
                control.removeClass("is-invalid");
            }
            //return validationMessages;
        }
    }
}();