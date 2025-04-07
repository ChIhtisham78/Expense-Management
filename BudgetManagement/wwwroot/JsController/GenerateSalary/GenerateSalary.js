$(function () {
    checkSalariesStatus();
    objGenerateSalaryTable = $('#GenerateSalary-table').DataTable({
        dom: '<"col-sm-12 float-right"B>lfrtip',
        buttons: [
            {
                extend: 'csv',
                exportOptions: { columns: [0, 1, 2, 3, 4, 5, 6] }
            },
            {
                extend: 'pdf',
                exportOptions: { columns: [0, 1, 2, 3, 4, 5, 6] },
                customize: function (doc) {
                    doc.content[1].table.body.forEach(function (row) {
                        row.forEach(function (cell, i) {
                            cell.alignment = 'center';
                        });
                    });
                }
            },
            {
                extend: 'print',
                exportOptions: { columns: [0, 1, 2, 3, 4, 5, 6] }
            }
        ],
        "lengthMenu": [10, 25, 50, 100],
        ajax: {
            url: "../api/ManageGenerateSalary/",
            type: 'GET',
            dataSrc: "",
        },
        initComplete: function () {
            ClosePleaseWait();
            $('.buttons-csv').html('<i class="fa fa-file-excel text-info fa-2x" />');
            $('.buttons-pdf').html('<i class="fa fa-file-pdf text-info fa-2x" />');
            $('.buttons-print').html('<i class="fa fa-print text-info fa-2x" />');
        },

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
                data: "BasicAmount",
                render: function (t, d, r) {
                    return '<h5 class="cursor-pointer amount-tooltip" data-amount="' + parseFloat(r.BasicAmount) + '" data-toggle="tooltip" data-placement="top" data-m="' + Fun_toWords(Number(r.BasicAmount)) + '" ><span class="badge badge-info"> ' + r.BasicAmount.toLocaleString("en") + '</span></h5>'
                }
            },
            {
                class: "text-center",
                width: "10%",
                data: "BonusAmount",
                render: function (t, d, r) {
                    return '<h5 class="cursor-pointer amount-tooltip" data-amount="' + parseFloat(r.BonusAmount) + '"  data-toggle="tooltip" data-placement="top" data-m="' + Fun_toWords(Number(r.BonusAmount)) + '" ><span class="badge badge-info"> ' + r.BonusAmount.toLocaleString("en") + '</span></h5>'
                }
            },
            {
                class: "text-center",
                width: "10%",
                data: "GrossPercentAmount",
                render: function (t, d, r) {
                    return '<h5 class="cursor-pointer amount-tooltip" data-amount="' + parseFloat(r.GrossPercentAmount) + '" data-toggle="tooltip" data-placement="top" data-m="' + Fun_toWords(Number(r.GrossPercentAmount)) + '" ><span class="badge badge-info"> ' + r.GrossPercentAmount.toLocaleString("en") + '</span></h5>'
                }
            },
            {
                class: "text-center",
                width: "10%",
                data: "GrossTotal",
                render: function (t, d, r) {
                    return '<h5 class="cursor-pointer amount-tooltip" data-amount="' + parseFloat(r.GrossTotal) + '" data-toggle="tooltip" data-placement="top" data-m="' + Fun_toWords(Number(r.GrossTotal)) + '" ><span class="badge badge-info"> ' + r.GrossTotal.toLocaleString("en") + '</span></h5>'
                }
            },
            {
                class: "text-center",
                width: "10%",
                data: "GeneratedSalaryMonth",
                render: function (d, t, r) {
                    return d; 
                }
            },
            {
                class: "text-center",
                width: "10%",
                render: function (data, type, full, meta) {
                    return `<button class="btn btn-sm btn-outline-info" onclick="generateInvoice(${ meta.row })"><i class="fa-solid fa-plus"></i></button>`; 
                }
            },
        ],
    });
});



function generateInvoice(id) {    
    var itemdata = objGenerateSalaryTable.row(id).data();
    console.log(itemdata);
    var data = {
        "Id": itemdata.Id,
        "AccountId": itemdata.AccountId
    }
    $.ajax({
        url: '../api/ManageGenerateSalary/GenerateInvoice',
        type: 'POST',
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function (data, textStatus, xhr) {
            if (data == "Invoice already!!") {
                toastErrorMessage("Invoice Already Generated")
            } else {
                toastInfoMessage("SuccessFully Proccssed");
                objGenerateSalaryTable.ajax.reload();    
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            alert('Error: ' + errorThrown);
        }
    });
}

function generateSalaries() {
    $.ajax({
        url: '../api/ManageGenerateSalary/GenerateSalaries',
        type: 'POST',
        contentType: 'application/json',    
        success: function (data, textStatus, xhr) {
            if (data === "Salaries already") {
                toastErrorMessage("Salaries Already Generated");
            } else if (data === "NoData") {
                toastErrorMessage("Please add basic Salary first");
            } else if (data === "Salaries successfully") {
                toastInfoMessage("Successfully Generated");
                objGenerateSalaryTable.ajax.reload();
                $('#btn-add-generate-salaries').prop('disabled', true);
                localStorage.setItem('salariesGenerated', true);
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            alert('Error: ' + errorThrown);
        }
    });
}
function checkSalariesStatus() {
    $.ajax({
        url: '../api/ManageGenerateSalary/CheckSalariesStatus',
        type: 'GET',
        success: function (data, textStatus, xhr) {
            if (data === "SalariesGenerated") {
                $('#btn-add-generate-salaries').prop('disabled', true);
                localStorage.setItem('salariesGenerated', true);
            } else if (data === "SalariesNotGenerated") {
                $('#btn-add-generate-salaries').prop('disabled', false);
                localStorage.setItem('salariesGenerated', false);
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            alert('Error: ' + errorThrown);
        }
    });
}


$(function () {
    ObjCompressTable = $('#compress-table').DataTable({
        searching: false,
        paging: false,
        info: false,
        ajax: {
            url: "../api/ManageCompress/GetCompressTable",
            type: 'GET',
            dataSrc: "",
        },
        columns: [
            { data: "AccountName" },
            {
                data: "Date",
                render: function (data, type, row) {
                    var date = new Date(data);
                    return date.toLocaleDateString();
                }
            },
            { data: "ExpenceType" },
            {
                data: "Amount",
                render: function (data, type, row) {
                    return '<span class="badge badge-primary amount-tooltip" data-amount="' + parseFloat(row.Amount) + '" style="font-size: 9px;">' + parseFloat(data).toLocaleString() + '</span>';
                }
            },

        ],
    });
});