$(function () {
    objAuditLogTable = $('#AuditLog-table').DataTable({
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
            url: "../api/ManageAuditLog/GetAuditLog",
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
                data: "Timestamp",
                render: function (data, type, row) {
                    var date = new Date(data);
                    return date.toLocaleDateString();
                }

            },
            {
                class: "text-center",
                width: "10%",
                data: "UserId"
            },
            {
                class: "text-center",
                width: "10%",
                data: "UserEmail"
            },
            {
                class: "text-center",
                width: "10%",
                data: "ActionType",
            },
            {
                class: "text-center",
                width: "10%",
                data: "TableName",
            },
            {
                class: "text-center",
                width: "10%",
                data: "KeyValues",
            },
            {
                class: "text-center",
                width: "20%", 
                data: "OldValues",
            },
            {
                class: "text-center",
                width: "20%", 
                data: "NewValues",
            }
        ],
    });
});