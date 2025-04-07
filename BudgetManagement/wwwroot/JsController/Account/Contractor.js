$(function () {
    ShowPleaseWait();
    resetForm();

    $("#frm-contractor").validate({
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "span",
        rules: {
            "txt-contractor-name": { required: true },
        },
        messages: {
            "txt-contractor-name": "Required *",
        },
    });

    objContractorTable = $('#contractor-table').DataTable({
        dom: '<"col-sm-12 float-right"B>lfrtip',
        buttons: [
            {
                extend: 'csv',
                exportOptions: { columns: [0, 1, 2, 3] }
            },
            {
                extend: 'pdf',
                exportOptions: { columns: [0, 1, 2, 3] },
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
                exportOptions: { columns: [0, 1, 2, 3] }
            }
        ],
        "lengthMenu": [10, 25, 50, 100],
        ajax: {
            url: "../api/ManageAccount/Contractor",
            type: 'GET',
            dataSrc: "",
        },
        initComplete: function () {
            ClosePleaseWait();
            $('.buttons-csv').html('<i class="fa fa-file-excel text-info fa-2x" />');
            $('.buttons-pdf').html('<i class="fa fa-file-pdf text-info fa-2x" />');
            $('.buttons-print').html('<i class="fa fa-print text-info fa-2x" />');
        },
        columnDefs: [{ orderable: false, targets: [4], }],
        columns: [
            {
                class: "text-center",
                width: "20%",
                data: "AccName"
            },
            {
                class: "text-center",
                width: "20%",
                data: "Email"
            },
            {
                class: "text-center",
                width: "20%",
                data: "Cell"
            },
            {
                class: "text-center desc-wrap",
                width: "30%",
                data: "Desc"
            },
            {
                class: "text-center",
                width: "10%",
                render: function (t, d, r) {
                    let externalLink = '<a class=""><i class="text-dark fas fa-eye "></i></a>';
                    if (r.WebSiteLink.trim().length > 0) {
                        externalLink = '<a href="https://' + r.WebSiteLink + '" target="_blank"><i class="text-info fas fa-eye "></i></a>';
                    }
                    return externalLink +
                        '<a onclick="editContractor(' + r.Id + ')" href ="javascript:void(0);" class="cmd-edit-contractor"><i class="text-info fas fa-edit pl-2"></i></a>' +
                        '<a onclick="deleteContractor(' + r.Id + ')" href ="javascript:void(0);" class="cmd-delete-contractor"><i class="text-primary fas fa-trash pl-2"></i></a>';
                }
            },
        ],
        autoWidth: false,
        wordWrap: true

    });

    // End Ready...
});

$('#btn-add-new-contractor').click(function () {
    $('#contractor-popup-title').text('Add New Contractor');
    $('#contractor-popup').modal({ backdrop: 'static', keyboard: false });
});

$("#frm-contractor").submit(function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    if (FNIsFormValid($(this))) {
        ShowPleaseWait();
        let Id = Number($('#hidn-txt-contractor-id').val());
        let methodType = Id > 0 ? "PUT" : "POST";
        let url = Id > 0 ? "../api/ManageAccount/EditAccount" : "../api/ManageAccount/Contractor"
        var data =
        {
            "Id": Id,
            "Name": $('#txt-contractor-name').val(),
            "Email": $('#txt-contractor-email').val(),
            "Cell": $('#txt-contractor-cell').val(),
            "WebSiteLink": $('#txt-contractor-webLink').val(),
            "Desc": $('#txt-contractor-desc').val(),
        }
        $.ajax({
            url: url,
            contentType: "application/json",
            type: methodType,
            data: JSON.stringify(data),
            success: function (result) {
                resetForm();
                ClosePleaseWait();
                $('#contractor-popup').modal('hide');
                toastInfoMessage("SuccessFully Proccssed")
                //showMessage('Success..!', '', 'success');
                objContractorTable.ajax.reload();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
});

function editContractor(id) {
    ShowPleaseWait();

    $('#contractor-popup-title').text('Edit Contractor');
    $.ajax({
        url: "../api/ManageAccount/Account/" + id,
        type: 'GET',
        success: function (result) {

            $('#hidn-txt-contractor-id').val(result.Id);

            $('#txt-contractor-name').val(result.AccName);
            $('#txt-contractor-email').val(result.Email);
            $('#txt-contractor-cell').val(result.Cell);
            $('#txt-contractor-webLink').val(result.WebSiteLink);
            $('#txt-contractor-desc').val(result.Desc);

            ClosePleaseWait();
            $('#contractor-popup').modal({ backdrop: 'static', keyboard: false });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function deleteContractor(id) {
    AreYouSure('Delete Confirmation !', 'Are You Sure You Want To Delete ?', 'warning').then((value) => {
        if (value) {
            ShowPleaseWait();
            $.ajax({
                url: "../api/ManageAccount/DeleteAccount/" + id,
                type: 'DELETE',
                success: function (result) {
                    ClosePleaseWait();
                    toastInfoMessage("SuccessFully Proccssed")
                    //showMessage('Success..!', '', 'success');
                    objContractorTable.ajax.reload();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    });
}

$("#contractor-popup").on("hidden.bs.modal", function () {
    FUNClearAllModalFields($(this));
    resetForm();
});

function resetForm() {
    $('#hidn-txt-contractor-id').val('0');
    $('#contractor-popup-title').text('Add New Contractor');
}