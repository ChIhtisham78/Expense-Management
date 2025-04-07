$(function () {
    resetForm();
    ShowPleaseWait();
    fillBusinessAccount();

    $("#frm-expence").validate({
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "span",
        rules: {
            "ddl-acc-name": { required: true },
            "txt-expence-type": { required: true },
            "dt-expence-date": { required: true },
            "txt-amount": { required: true },
        },
        messages: {
            "ddl-acc-name": "Required *",
            "txt-expence-type": "Required *",
            "dt-expence-date": "Required *",
            "txt-amount": "Required *",
        },
    });

    objExpenceTable = $('#expence-table').DataTable({
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
        "lengthMenu": [10, 25, 50, 100],
        ajax: {
            url: "../api/ManageExpence/Expences",
            type: 'GET',
            dataSrc: "",
        },
        initComplete: function () {
            ClosePleaseWait();
            $('.buttons-csv').html('<i class="fa fa-file-excel text-info fa-2x" />');
            $('.buttons-pdf').html('<i class="fa fa-file-pdf text-info fa-2x" />');
            $('.buttons-print').html('<i class="fa fa-print text-info fa-2x" />');
        },
        columnDefs: [{ orderable: false, targets: [6], }],
        columns: [
            {
                class: "text-center",
                width: "5%",
                data: "Id"
            },
            {
                class: "text-center",
                width: "15%",
                data: "ExpenceType"
            },
            {
                class: "text-center",
                width: "20%",
                data: "Account.AccName"
            },
            {
                class: "text-center",
                width: "10%",
                data: "ExpenceDate",
                render: function (d, t, r) {
                    return getDateWithFormate1(r.ExpenceDate);
                }
            },
            {
                class: "text-center",
                width: "15%",
                data: "ExpenceAmount",
                render: function (d, t, r) {
                    return '<h5 class="cursor-pointer amount-tooltip" data-amount="' + parseFloat(r.ExpenceAmount) + '" data-toggle="tooltip" data-placement="top" data-m="' + Fun_toWords(Number(r.ExpenceAmount)) + '" ><span class="badge badge-info"> ' + r.ExpenceAmount.toLocaleString("en") + '</span></h5>';
                }
            },
            {
                class: "text-center desc-wrap",
                width: "25%",
                data: "ExpenceDesc"
            },
            {
                class: "text-center",
                width: "10%",
                render: function (t, d, r) {
                    let tr = '';
                    tr += '<a onclick="eidtExpence(' + r.Id + ')" href ="javascript:void(0);" class="cmd-edit-client"><i class="text-info fas fa-edit pl-3"></i></a> ';
                    //if (IsSuperAdmin === 'True' || IsExpenceEditRole === 'True') {
                    //    tr += '<a onclick="eidtExpence(' + r.Id + ')" href ="javascript:void(0);" class="cmd-edit-client"><i class="text-info fas fa-edit pl-3"></i></a> ';
                    //}
                    //if (IsSuperAdmin === 'True' || IsExpenceDeleteRole === 'True' ) {
                    tr += '<a onclick = "deleteExpence(' + r.Id + ')" href = "javascript:void(0);" > <i class="text-primary fas fa-trash pl-3"></i></a >';
                    /*  }*/
                    return tr;
                }
            },
        ],
        autoWidth: false,
        wordWrap: true
    });

    // End Ready...
});

$('#btn-add-new-expence').click(function () {
    $('#expence-popup-title').text('Add New Expence');
    $('#expence-popup').modal({ backdrop: 'static', keyboard: false });
});

$("#frm-expence").submit(function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    if (FNIsFormValid($(this))) {
        ShowPleaseWait();
        let Id = Number($('#hidn-txt-expence-id').val());

        let methodType = Id > 0 ? "PUT" : "POST";
        var data =
        {
            "Id": Id,
            "AccountId": $('#ddl-acc-name option:selected').val(),
            "ExpenceType": $('#txt-expence-type').val(),
            "ExpenceDate": FUN_GetJqueryDate('dt-expence-date'),
            "ExpenceDesc": $('#txt-expence-desc').val(),
            "ExpenceAmount": Number($('#txt-amount').val()),
        }
        console.log(data);
        $.ajax({
            url: '../api/ManageExpence/Expence',
            contentType: "application/json",
            type: methodType,
            data: JSON.stringify(data),
            success: function (result) {
                resetForm();
                ClosePleaseWait();
                $('#expence-popup').modal('hide');
                toastInfoMessage("SuccessFully Proccssed")
                //showMessage('Success..!', '', 'success');
                objExpenceTable.ajax.reload();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
});

function eidtExpence(id) {
    ShowPleaseWait();

    $('#expence-popup-title').text('Edit Expence');
    $.ajax({
        url: "../api/ManageExpence/Expence/" + id,
        type: 'GET',
        success: function (result) {

            $('#hidn-txt-expence-id').val(result.Id);
            Fun_Select_DDL('ddl-acc-name', result.AccountId)
            $('#txt-expence-type').val(result.ExpenceType);
            $('#txt-expence-desc').val(result.ExpenceDesc);
            $('#dt-expence-date').val(getDateWithFormate1(result.ExpenceDate));
            $('#txt-amount').val(result.ExpenceAmount);

            ClosePleaseWait();
            $('#expence-popup').modal({ backdrop: 'static', keyboard: false });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

$("#expence-popup").on("hidden.bs.modal", function () {
    FUNClearAllModalFields($(this));
    resetForm();
});

function resetForm() {
    $('#hidn-txt-expence-id').val('0');
    $('#expence-popup-title').text('Add New Expence');
}

function fillBusinessAccount() {
    $.ajax({
        url: "../api/Getddl/BusinessAcc",
        type: 'GET',
        success: function (result) {
            Fun_Clean_And_Fill_DDL_complete('ddl-acc-name', result)
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function deleteExpence(id) {
    AreYouSure('Delete Confirmation !', 'Are You Sure You Want To Delete ?', 'warning').then((value) => {
        if (value) {
            ShowPleaseWait();
            $.ajax({
                url: "../api/ManageExpence/DeleteExpence/" + id,
                type: 'DELETE',
                success: function (result) {
                    ClosePleaseWait();
                    toastInfoMessage("SuccessFully Proccssed")
                    //showMessage('Success..!', '', 'success');
                    objExpenceTable.ajax.reload();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    });
}