$(function () {
    ShowPleaseWait();
    resetForm();
    objUserManagement = $('#user-table').DataTable({
        dom: '<"col-sm-12 float-right"B>lfrtip',
        buttons: [
            {
                extend: 'csv',
                exportOptions: { columns: [0, 1, 2, 3, 4,5,6] }
            },
            {
                extend: 'pdf',
                exportOptions: { columns: [0, 1, 2, 3, 4,5,6] },
                customize: function (doc) {
                    doc.content[1].table.body.forEach(function (row) {
                        row.forEach(function (cell) {
                            cell.alignment = 'center';
                        });
                    });
                    doc.content[1].table.widths = Array(doc.content[1].table.body[0].length + 1).join('*').split('');
                }
            },
            {
                extend: 'print',
                exportOptions: { columns: [0, 1, 2, 3, 4,5,6] }
            }
        ],
        "lengthMenu": [10, 25, 50, 100],
        ajax: {
            url: "../api/ManageUser/GetUsers",
            type: 'GET',
            dataSrc: "",
        },
        initComplete: function () {
            ClosePleaseWait();
            $('.buttons-csv').html('<i class="fa fa-file-excel text-info fa-2x" />');
            $('.buttons-pdf').html('<i class="fa fa-file-pdf text-info fa-2x" />');
            $('.buttons-print').html('<i class="fa fa-print text-info fa-2x" />');
        },
        columnDefs: [{ orderable: false, targets: [7], }],
        columns: [
            {
                class: "text-center",
                width: "20%",
                data: "Name",
            },
            {
                class: "text-center",
                width: "20%",
                data: "Address"
            },
            {
                class: "text-center",
                width: "20%",
                data: "City"
            },
            {
                class: "text-center",
                width: "20%",
                data: "Password"
            },
            {
                class: "text-center",
                width: "20%",
                data: "ZipCode"
            },
            {
                class: "text-center",
                width: "20%",
                data: "IsCompleted"
            },
            {
                class: "text-center",
                width: "20%",
                data: "Email"
            },
            {
                class: "text-center",
                width: "20%",
                render: function (t, d, r) {
                    console.log(r)
                    return `<a onclick="editUser('${r.Id}')" href="javascript:void(0);" class="cmd-edit-user"><i class="text-info fas fa-edit pl-2"></i></a><a onclick="DeleteUser('${r.Id}')" href="javascript:void(0);" class="cmd-delete-user"><i class="text-primary fas fa-trash pl-2"></i></a>`;

                }
            },
        ],

        autoWidth: false,
        wordWrap: true
    });

    // Initialize form validation
    $("#frm-user").validate({
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "span",
        rules: {
            "txt-user": { required: true },
           // "txt-address": { required: true },
            //"txt-user-city": { required: true },
            //"txt-state": { required: true },
           // "txt-zipcode": { required: true },
            "txt-email": { required: true },
            "txt-password": { required: true },
        },
        messages: {
            "txt-user": "Required *",
            //"txt-address": "Required *",
            //"txt-user-city": "Required *",
           // "txt-state": "Required *",
           // "txt-zipcode": "Required *",
            "txt-email": "Required *",
            "txt-password": "Required *",
        },
    });
});


$('#btn-add-new-user').click(function () {
    $('#user-popup-title').text('Add New User');
    $('#txt-email').prop('readonly', false);
    $('#frm-user').validate().settings.rules["txt-email"].required = true;
    $('#user-popup').modal({ backdrop: 'static', keyboard: false });
});

$("#frm-user").submit(function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    if (FNIsFormValid($(this))) {
        ShowPleaseWait();
        let Id = $('#hidn-txt-user-id').val();
        let methodType = Id != 0 ? "PUT" : "POST";
        let url = methodType === "PUT" ? "/api/ManageUser/EditUser" : "/api/ManageUser/SaveUserData";

        var data = {
            "Id": Id,
            "Name": $('#txt-user').val(),
            "Address": $('#txt-address').val(),
            "City": $('#txt-user-city').val(),
            "State": $('#txt-state').val(),
            "ZipCode": $('#txt-zipcode').val(),
            "Email": $('#txt-email').val(),
            "Password": $('#txt-password').val()
        };
        $.ajax({
            url: url,
            contentType: "application/json",
            type: methodType,
            data: JSON.stringify(data),
            success: function (result) {
                resetForm();
                ClosePleaseWait();
                $('#user-popup').modal('hide');
                toastInfoMessage("Successfully Processed");
                objUserManagement.ajax.reload();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                ClosePleaseWait();
                $('#user-popup').modal('hide');
                toastErrorMessage(xhr.responseText);
                alert(xhr.status);
                alert(thrownError);
            }
        });

    }
});



function editUser(id) {
    ShowPleaseWait();
    $('#user-popup-title').text('Edit User');
    $.ajax({
        url: "/api/ManageUser/EditUserData/" + id,
        type: 'GET',
        success: function (result) {
            if (result) {
                $('#hidn-txt-user-id').val(id);
                $('#txt-user').val(result.Name);
                $('#txt-address').val(result.Address);
                $('#txt-user-city').val(result.City); 
                $('#txt-state').val(result.State);    
                $('#txt-zipcode').val(result.ZipCode);
                $('#txt-password').val(result.Password);
                $('#txt-email').val(result.Email).prop('readonly', true);
                $('#frm-user').validate().settings.rules["txt-email"].required = false;
                ClosePleaseWait();
                $('#user-popup').modal({ backdrop: 'static', keyboard: false });
            } else {
                ClosePleaseWait();
                alert("User data not found.");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            ClosePleaseWait();
            alert(xhr.status + ': ' + thrownError);
        }
    });
}




function DeleteUser(id) {
    AreYouSure('Delete Confirmation !', 'Are You Sure You Want To Delete ?', 'warning').then((value) => {
        if (value) {
            ShowPleaseWait();
            $.ajax({
                url: "../api/ManageUser/DeleteUser/" + id,
                type: 'DELETE',
                success: function (result) {
                    ClosePleaseWait();
                    toastInfoMessage("SuccessFully Proccssed")
                    objUserManagement.ajax.reload();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
    });
}

$("#user-popup").on("hidden.bs.modal", function () {
    FUNClearAllModalFields($(this));
    resetForm();
});

function resetForm() {
    $('#hidn-txt-user-id').val('0');
    $('#user-popup-title').text('Add New User');
}



