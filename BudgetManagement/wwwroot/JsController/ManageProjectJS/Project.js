$(function () {
    ShowPleaseWait();
    resetForm();
    fillClients();

    $("#frm-project").validate({
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "span",
        rules: {
            "txt-project-name": { required: true },
            "ddl-client-name": { required: true },
            "ContractorEffectivePercentage": {
                required: true,
                range: [0, 100]
            }
        },
        messages: {
            "txt-project-name": "Project Name is Required *",
            "ddl-client-name": "Client Name is Required *",
            "ContractorEffectivePercentage": {
                required: "Contractor Effective Percentage is Required *",
                range: "Contractor Effective Percentage must be between 0 - 100 *"
            }
        },
    });

    objProjectTable = $('#project-table').DataTable({
        dom: '<"col-sm-12 float-right"B>lfrtip',
        buttons: [
            {
                extend: 'csv',
                exportOptions: { columns: [0, 1, 2] }
            },
            {
                extend: 'pdf',
                exportOptions: { columns: [0, 1, 2] },
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
                exportOptions: { columns: [0, 1, 2] }
            }
        ],
        "lengthMenu": [10, 25, 50, 100],
        ajax: {
            url: "../api/ManageProject/Project",
            type: 'GET',
            dataSrc: "",
        },
        initComplete: function () {
            ClosePleaseWait();
            $('.buttons-csv').html('<i class="fa fa-file-excel text-info fa-2x" />');
            $('.buttons-pdf').html('<i class="fa fa-file-pdf text-info fa-2x" />');
            $('.buttons-print').html('<i class="fa fa-print text-info fa-2x" />');
        },
        columnDefs: [{ orderable: false, targets: [3], }],
        columns: [
            {
                class: "text-center",
                width: "20%",
                data: "Client.AccName"
            },
            {
                class: "text-center",
                width: "25%",
                data: "ProjectName"
            },
            {
                class: "text-center",
                width: "10%",
                data: "ContractorEffectivePercent",
                render: function (data) {
                    return parseFloat(data).toFixed(2);
                }
            },
            {
                width: "10%",
                class: "text-center",
                render: function (t, d, r) {

                    return '<a onclick="editProject(' + r.Id + ')" href ="javascript:void(0);" class="cmd-edit-project"><i class="text-info fas fa-edit pl-2"></i></a>' +
                        '<a onclick="deleteProject(' + r.Id + ')" href ="javascript:void(0);" class="cmd-delete-project"><i class="text-primary fas fa-trash pl-2"></i></a>';
                }
            },
        ]
    });

    // End Ready...
});

$('#btn-add-new-project').click(function () {
    $('#project-popup-title').text('Add New Project');
    $('#project-popup').modal({ backdrop: 'static', keyboard: false });
});

$("#frm-project").submit(function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    if (FNIsFormValid($(this))) {
        ShowPleaseWait();
        let Id = Number($('#hidn-txt-project-id').val());
        let methodType = Id > 0 ? "PUT" : "POST";
        var data =
        {
            "Id": Id,
            "ClientId": Number($('#ddl-client-name option:selected').val()),
            "ProjectName": $('#txt-project-name').val(),
            "ContractorEffectivePercent": Number($('#ContractorEffectivePercent').val())
        }
        console.log(data);
        $.ajax({
            url: "../api/ManageProject/Project",
            contentType: "application/json",
            type: methodType,
            data: JSON.stringify(data),
            success: function (result) {
                resetForm();
                ClosePleaseWait();
                $('#project-popup').modal('hide');
                toastInfoMessage("SuccessFully Proccssed")
                //showMessage('Success..!', '', 'success');
                objProjectTable.ajax.reload();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
});

function editProject(id) {
    ShowPleaseWait();

    $('#project-popup-title').text('Edit Project');
    $.ajax({
        url: "../api/ManageProject/Project/" + id,
        type: 'GET',
        success: function (result) {

            $('#hidn-txt-project-id').val(result.Id);
            Fun_Select_DDL('ddl-client-name', result.ClientId);
            $('#txt-project-name').val(result.ProjectName);
            $('#ContractorEffectivePercent').val(result.ContractorEffectivePercent);
            ClosePleaseWait();
            $('#project-popup').modal({ backdrop: 'static', keyboard: false });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function deleteProject(id) {
    AreYouSure('Delete Confirmation !', 'Are You Sure You Want To Delete ?', 'warning').then((value) => {
        if (value) {
            ShowPleaseWait();
            $.ajax({
                url: "../api/ManageProject/DeleteProject/" + id,
                type: 'DELETE',
                success: function (result) {
                    ClosePleaseWait();
                    toastInfoMessage("SuccessFully Proccssed")
                    //showMessage('Success..!', '', 'success');
                    objProjectTable.ajax.reload();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    });
}

$("#project-popup").on("hidden.bs.modal", function () {
    FUNClearAllModalFields($(this));
    resetForm();
});

function resetForm() {
    $('#hidn-txt-project-id').val('0');
    $('#project-popup-title').text('Add New Project');
}

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