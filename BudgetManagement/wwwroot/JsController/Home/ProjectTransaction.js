$(function () {
    fillClients();
    $("#frm-project-transaction").validate({
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "span",
        rules: {
            "ddl-client-name": { required: true },
            "ddl-project-name": { required: true },
            "dt-project-transaction-date": { required: true },
            "txt-amount": { required: true, digits: true },
        },
        messages: {
            "ddl-client-name": "Required *",
            "ddl-project-name": "Required *",
            "dt-project-transaction-date": "Required *",
            "txt-amount": "Required *",
        },
    });

    objProjectTransactionTable = $('#project-transaction-table').DataTable({
        dom: '<"col-sm-12 float-right"B>lfrtip',
        buttons: [
            {
                extend: 'csv',
                exportOptions: { columns: [0, 1, 2, 3, 4] }
            },
            {
                extend: 'pdf',
                exportOptions: { columns: [0, 1, 2, 3, 4] },
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
                exportOptions: { columns: [0, 1, 2, 3, 4] }
            }
        ],
        "lengthMenu": [20, 40, 60, 80, 100],
        ajax: {
            url: "../api/ManageInvoice/ProjectTransaction",
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
        columnDefs: [{ orderable: false, targets: [4, 5], }],
        columns: [
            {
                class: "text-center",
                width: "10%",
                data: "AccName"
            },
            {
                class: "text-center",
                width: "10%",
                data: "ProjectName"
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
                    return '<h5 class="cursor-pointer  amount-tooltip" data-amount="' + parseFloat(r.Amount) + '" data-amount="' + parseFloat(r.Amount) + '" data-toggle="tooltip" data-placement="top" data-m="' + Fun_toWords(Number(r.Amount)) + '" ><span class="badge badge-info"> ' + r.Amount.toLocaleString("en") + '</span></h5>'
                }
            },
            {
                class: "text-center desc-wrap",
                width: "20%",
                data: "Desc"
            },
            {
                class: "text-center",
                width: "10%",
                render: function (data, type, full, meta) {
                    return `<button class="btn-sm btn btn-outline-info toggle-row" onclick="salaryConfirm(${meta.row})"><i class="fa-solid fa-plus"></i></button>`;
                }
            }
        ],
        autoWidth: false,
        wordWrap: true
    });
});

$('#btn-add-new-project-transaction').click(function () {
    $('#project-transaction-popup').modal({ backdrop: 'static', keyboard: false });
});

function fillClients() {
    $.ajax({
        url: "../api/Getddl/Clients",
        type: 'GET',
        success: function (result) {
            Fun_Clean_And_Fill_DDL_complete('ddl-client-name', result)
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

$("#frm-project-transaction").submit(function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    if (FNIsFormValid($(this))) {
        ShowPleaseWait();
        var data =
        {
            "Amount": Number($('#txt-amount').val()),
            "ClientId": Number($('#ddl-client-name option:selected').val()),
            "ProjectId": Number($('#ddl-project-name option:selected').val()),
            "Desc": $('#txt-desc').val(),
            "TransactionDate": FUN_GetJqueryDate('dt-project-transaction-date'),
        }
        $.ajax({
            url: '../api/ManageInvoice/ProjectTransaction',
            contentType: "application/json",
            type: 'POST',
            data: JSON.stringify(data),
            success: function (result) {
                ClosePleaseWait();
                $('#project-transaction-popup').modal('hide');
                toastInfoMessage("SuccessFully Proccssed")
                objProjectTransactionTable.ajax.reload();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
});

$("#project-transaction-popup").on("hidden.bs.modal", function () {
    FUNClearAllModalFields($(this));
});


$('#ddl-client-name').change(function () {
    let selectedVal = $('#ddl-client-name option:selected').val();
    if (Number(selectedVal) > 0) {
        populateProjectTransactionWRTType(selectedVal);
        $('#ddl-project-name').prop('disabled', false);
    } else {
        Fun_Clean_DDL("ddl-project-name");
        Fun_Chose_option_to_DDL("ddl-project-name");
        $('#ddl-project-name').prop('disabled', true);
    }
});


function populateProjectTransactionWRTType(id) {
    $.ajax({
        url: "../api/Getddl/GetProjectWRTType/" + id,
        type: 'GET',
        success: function (result) {
            Fun_Clean_And_Fill_DDL_complete('ddl-project-name', result)
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function salaryConfirm(transactionId) {
    var itemdata = objProjectTransactionTable.row(transactionId).data();
    console.log(itemdata)
    var data = {
        "Amount": itemdata.Amount,
        "ProjectId": itemdata.ProjectId,
        "TransactionId": itemdata.Id,
    }

    $.ajax({
        url: '../api/GetProjectList',
        contentType: "application/json",
        type: 'POST',
        data: JSON.stringify(data),
        success: function (data, textStatus, xhr) {
            if (data === "Not Add salaries") {
                toastErrorMessage("You Are Not Added Salaries of this Month");
            } else if (data === "Salaries already In db") {
                toastErrorMessage("Salaries Already Generated");
            } else if (data === "No Contractor") {
                toastErrorMessage("This Project Is Not Assigned To Any Contractor");
            } else if (data === "Successfully Generated") {
                toastInfoMessage("Successfully Generated");
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            alert('Error: ' + errorThrown);
        }
    });
}