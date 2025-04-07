var selectedOptions = [];
$(function () {
    fillContractors();
    fillProjects();
    fillSingleProject();
    resetForm();

    $('#ddl-project-name').multipleSelect({
        placeholder: '---Please Select Project Name----',
    });

    $("#ddl-project-name").change(function (e) {
        let select = document.getElementById("ddl-project-name");
        selectedOptions = [];
        for (let i = 0; i < select.options.length; i++) {
            if (select.options[i].selected) {
                selectedOptions.push(select.options[i].value);
            }
        }
    });

    $("#frm-salary").validate({
        errorClass: "is-invalid",
        validClass: "is-valid",
        errorElement: "span",
        rules: {
            "ddl-contractor-name": "required",
            "ddl-single-project-name": "required",
            "txt-amount": "required",
        },
        messages: {
            "ddl-contractor-name": "Please Select contractor Name",
            "ddl-single-project-name": "Please Select project Name",
            "txt-amount": "Please enter Basic Salary",
        },
        errorPlacement: function (error, element) {
            if (element.attr("name") == "ddl-single-project-name") {
                error.appendTo("#ddl-single-project-name-error-div");
            } if (element.attr("name") == "ddl-contractor-name") {
                error.appendTo("#ddl-contractor-name-error-div");
            } if (element.attr("name") == "txt-amount") {
                error.appendTo("#txt-amount-error-div");
            }
        }
    });

    objSalaryTable = $('#salary-table').DataTable({
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
        "lengthMenu": [20, 40, 60, 80, 100],
        ajax: {
            url: "../api/ManageSalary/",
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
        columnDefs: [{ orderable: false, targets: [2, 4], }, { orderable: true, targets: 0 }],
          
        columns: [
            {
                class: "text-center",
                width: "10%",
                data: "AccountName",
            },
            {
                class: "text-center",
                width: "10%",
                data: "SingleProject"
            },
            {
                class: "text-center",
                width: "5%",
                data: "EffectivePercentProjectMappings",
                render: function (data, type, row, meta) {
                    if (type === 'display') {
                        let projectNames = data.map(item => item.ProjectName).join(", ");
                        return projectNames;
                    }
                    return '';
                },
                createdCell: function (cell, cellData, rowData, rowIndex, colIndex) {
                    if (colIndex === 2) {
                        if (rowData.EffectivePercentProjectMappings && rowData.EffectivePercentProjectMappings.length > 0) {
                            $(cell).html('<button class="btn-sm btn btn-outline-info toggle-row" data-row-id="' + rowData.Id + '"><i class="fa-solid fa-minus"></i></button>');
                        } else {
                            $(cell).html('<span></span>');
                        }
                    }
                }
            },
            {
                class: "text-center",
                width: "10%",
                data: "BasicAmount",
                render: function (t, d, r) {
                    return '<h5 class="cursor-pointer amount-tooltip" data-amount="' + parseFloat(r.BasicAmount) + '" data-toggle="tooltip" data-placement="top" data-m="' + Fun_toWords(Number(r.BasicAmount)) + '" ><span class="badge badge-info"> ' + r.BasicAmount.toLocaleString("en") + '</span></h5>'
                }
            },
            {
                class: "text-center",
                width: "10%",
                render: function (t, d, r) {
                    return '<a  href ="javascript:void(0);" class="cmd-edit-salary"><i class="text-info fas fa-edit pl-2"></i></a>';
                }
            }
        ],
        "order": [[0, "asc"]],
    });

    $('#salary-table tbody').on('click', 'button.toggle-row', function () {
        var rowId = $(this).data('row-id');
        var tr = $(this).closest('tr');
        var row = objSalaryTable.row(tr);

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
        } else {
            row.child(formatAdditionalData(rowId)).show();
            tr.addClass('shown');
        }
    });

    function formatAdditionalData(rowId) {
        var data = findRowById(rowId);

        if (!data) {
            return '';
        }

        var additionalData = "<div class='expanded-row-content w-25 m-auto'>";
        additionalData += "<p class='font-weight-bolder'> Project Names:";
        additionalData += "<ul>";

        if (data.EffectivePercentProjectMappings.length === 0) {
            additionalData += "<li>No Multiple Project</li>";
        } else {
            for (var i = 0; i < data.EffectivePercentProjectMappings.length; i++) {
                additionalData += "<li>" + data.EffectivePercentProjectMappings[i].ProjectName + "</li>";
            }
        }
        additionalData += "</ul>";
        additionalData += "</p>";
        additionalData += "</div>";
        return additionalData;
    }

    function findRowById(rowId) {
        var data = objSalaryTable.rows().data().toArray();
        for (var i = 0; i < data.length; i++) {
            if (data[i].Id === rowId) {
                return data[i];
            }
        }
        return null;
    }
    $(document).on('click', '.cmd-edit-salary', function () {
        var id = Number(objSalaryTable.row($(this).closest("tr")).data().Id);
        $('#hidn-form-salary-id').val(id);
        editSalary(Number(objSalaryTable.row($(this).closest("tr")).data().Id));
    });


});
function resetForm() {
    $('#salary-popup-title').text('Add Salary');
    $('#hidn-form-salary-id').val('0');
}

$('#btn-add-new-salary').click(function () {
    $('#salary-popup').modal({ backdrop: 'static', keyboard: false });
});

function fillContractors() {
    $.ajax({
        url: "../api/Getddl/Contractor",
        type: 'GET',
        success: function (result) {
            Fun_Clean_And_Fill_DDL_complete('ddl-contractor-name', result)
            $('#ddl-contractor-name').multipleSelect('refreshOptions', { filter: true });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
};

function fillSingleProject() {
    $.ajax({
        url: "../api/Getddl/SingleProject",
        type: 'GET',
        success: function (result) {
            Fun_Clean_And_Fill_DDL_complete('ddl-single-project-name', result)
            $('#ddl-single-project-name').multipleSelect('refreshOptions', { filter: true });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

$("#frm-salary").submit(function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    if (FNIsFormValid($(this))) {
        let Id = Number($('#hidn-form-sallary-id').val());
        let methodType = Id > 0 ? "PUT" : "POST";
        var data =
        {
            "Id": Id,
            "AccountId": Number($('#ddl-contractor-name option:selected').val()),
            "MultipleProjectId": selectedOptions.join(','),
            "SingleProjectId": Number($('#ddl-single-project-name option:selected').val()),
            "BasicAmount": Number($('#txt-amount').val()),
        }
        $.ajax({
            url: '../api/ManageSalary/',
            contentType: "application/json",
            type: methodType,
            data: JSON.stringify(data),
            success: function (result) {
                if (result.message == "Already Exists") {
                    toastErrorMessage("This Contractor Name Already Exists")
                }
                else {
                    ClosePleaseWait();
                    $('#salary-popup').modal('hide');
                    resetForm();
                    toastInfoMessage("SuccessFully Proccssed");
                    objSalaryTable.ajax.reload();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
});

$("#salary-popup").on("hidden.bs.modal", function () {
    FUNClearAllModalFields($(this));
    resetForm()
    $('#ddl-contractor-name').multipleSelect('uncheckAll');
    $('#ddl-contractor-name').multipleSelect('check', "");

    $('#ddl-single-project-name').multipleSelect('uncheckAll');
    $('#ddl-single-project-name').multipleSelect('check', "");

    $('#ddl-project-name').multipleSelect('uncheckAll');
    $('#ddl-project-name').multipleSelect('check', "");
});

function fillProjects() {

    $.ajax({
        url: "../api/Getddl/Project",
        type: 'GET',
        success: function (result) {
            Fun_Clean_DDL("ddl-project-name");
            $.each(result, function (index, Row) {
                $('#ddl-project-name').append('<option value=' + Row.Value + '>' + Row.Text + '</option>');
            });
            $('#ddl-project-name').multipleSelect('refreshOptions', { filter: true });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function editSalary(id) {
    $('#salary-popup-title').text('Edit Salary');
    $.ajax({
        url: "../api/ManageSalary/" + id,
        type: 'GET',
        success: function (result) {

            document.getElementById('hidn-form-sallary-id').value = result.SallaryMapping.Id;
            Fun_Select_DDL('ddl-single-project-name', result.SallaryMapping.ProjectId);
            Fun_Select_DDL('ddl-contractor-name', result.SallaryMapping.AccountId);
            $('#ddl-contractor-name').multipleSelect('refreshOptions', { filter: true });
            $('#ddl-single-project-name').multipleSelect('refreshOptions', { filter: true });

            $('#txt-amount').val(result.SallaryMapping.BasicAmount);

            let projectIds = [];
            result.EffectivePercentProjectMappings.forEach(function (mapping) {
                projectIds.push(mapping.ProjectId);
            });
            $('#ddl-project-name').multipleSelect('setSelects', projectIds);

            $('#salary-popup').modal({ backdrop: 'static', keyboard: false });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function deleteSalary(id) {
    AreYouSure('Delete Confirmation !', 'Are You Sure You Want To Delete ?', 'warning').then((value) => {
        if (value) {
            ShowPleaseWait();
            $.ajax({
                url: "../api/ManageSalary/" + id,
                type: 'DELETE',
                success: function (result) {
                    ClosePleaseWait();
                    objSalaryTable.ajax.reload();
                    toastInfoMessage("SuccessFully Proccssed");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    if (xhr.status == 500) {
                        showMessage('Error..! Internal Server Error.', xhr.responseText, 'error');
                    } else {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                }
            });
        }
    });
}