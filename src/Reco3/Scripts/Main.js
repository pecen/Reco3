function loadPartialView(url, targetId, optionalData, showProgress) {
    optionalData = (typeof optionalData === "undefined") ? "" : optionalData;
    $("body").addClass('ajaxLoading');
    if (showProgress===true)
        startLoading();
    $.ajax({
            type: "GET",
            url: url,
            data: optionalData,
            datatype: "html",
            cache: false,
            success: function (data) {
                $('#' + targetId).html(data);
                hideErrorMessage();
            },
            error: function (xhr) { onFailedAjaxResponse(xhr) },
            complete: function () {
            $("body").removeClass('ajaxLoading');
            if (showProgress === true)
                stopLoading();
            }
        }
    );
} 



function startLoading() {
    $('#divLoad').show();
}

function stopLoading() {
    $('#divLoad').hide();
}
function oneStepForward() {
    var id = $('#PDFlowSteps .active').attr('id');
    $('#' + id).toggleClass('active');
    $('#' + id).next().toggleClass('active');
}
function getTablesData(id, actionName, requestPageInput, lengthMenuArray, columnDefs) {

    $.fn.dataTable.ext.errMode = 'none';
    startLoading();
    var table = $('#' + id).on('error.dt', function (e, settings, techNote, message) {
        stopLoading();
        $('#DataTableError').text(message);
        $('#DataTableError').show();
    }).DataTable({
        "searching": false,
        "ordering": false,
        "lengthMenu": lengthMenuArray,
        "language": {
            "lengthMenu": "<b>Number of rows to show:</b> _MENU_ ",
            "info": "<b>Showing _START_ to _END_ of _TOTAL_</b>",
            "paginate": {
                "first": "<<",
                "previous": "<",
                "next": ">",
                "last": ">>"
            }
        },
        "sDom": 'lpit',
        "sPaginationType": "full_numbers",
        "bServerSide": true,
        "sAjaxSource": "BackOffice/" + actionName,
        "fnServerParams": function (aoData) {
            aoData.push(
                { "name": "RequestedDate", "value": requestPageInput.RequestedDate },
                { "name": "ShowAllRequests", "value": requestPageInput.ShowAllRequests },
                { "name": "VehicleGroup", "value": requestPageInput.Search.VehicleGroup },
                { "name": "Status", "value": requestPageInput.Search.Status },
                { "name": "State", "value": requestPageInput.Search.State },
                { "name": "ProductionDateFrom", "value": requestPageInput.Search.ProductionDateFrom },
                { "name": "ProductionDateTo", "value": requestPageInput.Search.ProductionDateTo },
                { "name": "CalculationDateFrom", "value": requestPageInput.Search.CalculationDateFrom },
                { "name": "CalculationDateTo", "value": requestPageInput.Search.CalculationDateTo },
                { "name": "ChassisNo", "value": requestPageInput.Search.ChassisNo },
                { "name": "RequestCode", "value": requestPageInput.Search.RequestCode },
                { "name": "ErrorSource", "value": requestPageInput.Search.ErrorSource },
                { "name": "DistributorId", "value": requestPageInput.Search.DistributorId }
            );
        },
        "columnDefs": columnDefs,
        "scrollY": "400px",
        "scrollCollapse": true,
    });

    $('#' + id).on('length.dt', function () {
        startLoading();
    });
    $('#' + id).on('page.dt', function () {
        startLoading();
    });

    return table;
}

function copyToClipboard(elementId) {
    var aux = document.createElement("input");
    aux.setAttribute("value", document.getElementById(elementId).innerHTML);
    document.body.appendChild(aux);
    aux.select();
    document.execCommand("copy");

    document.body.removeChild(aux);
}
function onFailedAjaxResponse(n) { n.status === 555 ? $("body").html(n.responseText) : showErrorMessage("ERROR", "Could not connect to the server.") }
function onFailedAjaxResponse(xhr) {
    if (xhr.status === 555) {
        $("body").html(xhr.responseText);
    } else {
        showErrorMessage("ERROR", "Could not connect to the server.");
    }
}
function initActiveButtons(targetClass) {
    $('.' + targetClass).click(function () {
        $('.' + targetClass).removeClass('active');
        $(this).addClass('active');
    });
}
function showErrorMessage(n, t) { $("#errorMessage-title").html(n); $("#errorMessage-body").html(t); $("#errorMessage-div").fadeIn(300) }
function hideErrorMessage() { $("#errorMessage-div").fadeOut(300) }