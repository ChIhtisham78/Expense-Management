$(function () {
    fillBusinessAccounts();
    $("#frm-transaction").validate({
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "span",
        rules: {
            "ddl-beneficiary-name": { required: true },
            "dt-transaction-date": { required: true },
            "txt-amount": { required: true, digits: true },
        },
        messages: {
            "ddl-beneficiary-name": "Required *",
            "dt-transaction-date": "Required *",
            "txt-amount": "Required *",
        },
    });

    objCapitalTransactionTable = $('#transaction-table').DataTable({
        dom: '<"col-sm-12 float-right"B>lfrtip',
        buttons: [
            {
                extend: 'csv',
                exportOptions: { columns: [0, 1, 2, 3, 4, 5] }
            },
            {
                extend: 'pdf',
                exportOptions: { columns: [0, 1, 2, 3, 4, 5] },
                customize: function (doc) {
                    // Set the alignment of the content to center
                    doc.content[1].table.body.forEach(function (row) {
                        row.forEach(function (cell) {
                            cell.alignment = 'center';
                        });
                    });
                    // Set the width of the table to cover the full page width
                    doc.content[1].table.widths = Array(doc.content[1].table.body[0].length + 1).join('*').split('');
                }
            },
            {
                extend: 'print',
                exportOptions: { columns: [0, 1, 2, 3, 4, 5] }
            }
        ],
        "lengthMenu": [20, 40, 60, 80, 100],
        ajax: {
            url: "../api/ManageInvoice/BusinessCapitalTransaction",
            type: 'GET',
            dataSrc: "",
        },
        initComplete: function () {
            ClosePleaseWait();
            $('.buttons-csv').html('<i class="fa fa-file-excel text-info fa-2x" />');
            $('.buttons-pdf').html('<i class="fa fa-file-pdf text-info fa-2x" />');
            $('.buttons-print').html('<i class="fa fa-print text-info fa-2x" />');
        },
        order: [0, "desc"],
        columns: [
            {
                class: "text-center",
                width: "10%",
                data: "TransactionId"
            },
            {
                class: "text-center",
                width: "10%",
                data: "TransactionDate",
                render: function (d, t, r) {
                    return getDateWithFormate1(r.TransactionDate);
                }
            },
            {
                class: "text-center",
                width: "10%",
                data: "Amount",
                render: function (t, d, r) {
                    return '<h5 class="cursor-pointer amount-tooltip" data-amount="' + parseFloat(r.Amount) + '" data-toggle="tooltip" data-placement="top" data-m="' + Fun_toWords(Number(r.Amount)) + '" ><span class="badge badge-info"> ' + r.Amount.toLocaleString("en") + '</span></h5>'
                }
            },  
            {
                class: "text-center",
                width: "25%",
                data: "Payee"
            },
            {
                class: "text-center",
                width: "25%",
                data: "Recipient"
            },
            {
                class: "text-center desc-wrap",
                width: "20%",
                data: "Desc"
            }
        ],
        autoWidth: false,
        wordWrap: true
    });
    // End Ready Function.
});

$('#btn-add-new-capital-transaction').click(function () {
    $('#transaction-popup').modal({ backdrop: 'static', keyboard: false });
});

function fillBusinessAccounts() {
    $.ajax({
        url: "../api/Getddl/BusinessAcc",
        type: 'GET',
        success: function (result) {

            Fun_Clean_And_Fill_DDL_complete('ddl-beneficiary-name', result)
            $('#ddl-beneficiary-name').attr('disabled', false);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

$("#frm-transaction").submit(function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    if (FNIsFormValid($(this))) {
        ShowPleaseWait();
        var data =
        {
            "Amount": Number($('#txt-amount').val()),
            "BeneficiaryAccountId": Number($('#ddl-beneficiary-name option:selected').val()),
            "Desc": $('#txt-desc').val(),
            "TransactionDate": FUN_GetJqueryDate('dt-transaction-date'),
        }
        $.ajax({
            url: '../api/ManageInvoice/BusinessCapitalTransaction',
            contentType: "application/json",
            type: 'POST',
            data: JSON.stringify(data),
            success: function (result) {
                ClosePleaseWait();
                $('#transaction-popup').modal('hide');
                toastInfoMessage("SuccessFully Proccssed")
                //showMessage('Success..!', '', 'success');
                objCapitalTransactionTable.ajax.reload();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
});

$("#transaction-popup").on("hidden.bs.modal", function () {
    FUNClearAllModalFields($(this));
});