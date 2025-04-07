$(function () {
    ShowPleaseWait();
    resetForm();

    $("#frm-business").validate({
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "span",
        rules: {
            "txt-business-name": { required: true },
        },
        messages: {
            "txt-business-name": "Required *",
        },
    });

    objBusinessTable = $('#business-table').DataTable({
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
            url: "../api/ManageAccount/Business",
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
                        '<a onclick="editBusiness(' + r.Id + ')" href ="javascript:void(0);" class="cmd-edit-business"><i class="text-info fas fa-edit pl-2"></i></a>' +
                        '<a onclick="deleteBusiness(' + r.Id + ')" href ="javascript:void(0);" class="cmd-delete-business"><i class="text-primary fas fa-trash pl-2"></i></a>';
                }
            },
        ],
        autoWidth: false,
        wordWrap: true
    });

    // End Ready...
});

$('#btn-add-new-business').click(function () {
    $('#business-popup-title').text('Add New Business');
    $('#business-popup').modal({ backdrop: 'static', keyboard: false });
});

$("#frm-business").submit(function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    if (FNIsFormValid($(this))) {
        ShowPleaseWait();
        let Id = Number($('#hidn-txt-business-id').val());
        let methodType = Id > 0 ? "PUT" : "POST";
        let url = Id > 0 ? "../api/ManageAccount/EditAccount" : "../api/ManageAccount/Business"
        var data =
        {
            "Id": Id,
            "Name": $('#txt-business-name').val(),
            "Email": $('#txt-business-email').val(),
            "Cell": $('#txt-business-cell').val(),
            "WebSiteLink": $('#txt-business-webLink').val(),
            "Desc": $('#txt-business-desc').val(),
        }
        $.ajax({
            url: url,
            contentType: "application/json",
            type: methodType,
            data: JSON.stringify(data),
            success: function (result) {
                resetForm();
                ClosePleaseWait();
                $('#business-popup').modal('hide');
                toastInfoMessage("SuccessFully Proccssed")
                //showMessage('Success..!', '', 'success');
                objBusinessTable.ajax.reload();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
});

function editBusiness(id) {
    ShowPleaseWait();

    $('#business-popup-title').text('Edit Business');
    $.ajax({
        url: "../api/ManageAccount/Account/" + id,
        type: 'GET',
        success: function (result) {

            $('#hidn-txt-business-id').val(result.Id);

            $('#txt-business-name').val(result.AccName);
            $('#txt-business-email').val(result.Email);
            $('#txt-business-cell').val(result.Cell);
            $('#txt-business-webLink').val(result.WebSiteLink);
            $('#txt-business-desc').val(result.Desc);

            ClosePleaseWait();
            $('#business-popup').modal({ backdrop: 'static', keyboard: false });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function deleteBusiness(id) {
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
                    objBusinessTable.ajax.reload();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    });
}

$("#business-popup").on("hidden.bs.modal", function () {
    FUNClearAllModalFields($(this));
    resetForm();
});

function resetForm() {
    $('#hidn-txt-business-id').val('0');
    $('#business-popup-title').text('Add New Business');
}

