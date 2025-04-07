(function () {
    objPayableInvoice = $('#PayableInvoice-table').DataTable({
        dom: '<"col-sm-12 float-right"B>lfrtip',
        buttons: [
            {
                extend: 'csv',
                exportOptions: { columns: [0, 1, 2, 3,4] }
            },
            {
                extend: 'pdf',
                exportOptions: { columns: [0, 1, 2, 3,4] },
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
                exportOptions: { columns: [0, 1, 2, 3,4] }
            }
        ],
        "lengthMenu": [10, 25, 50, 100],
        ajax: {
            url: "../api/ManagePayableInvoice/GetPayableInvoices",
            type: 'GET',
            dataSrc: "",
        },
        columns: [
            {
                class: "text-center",
                width: "30%",
                data: "AccountName"
            },
            {
                class: "text-center",
                width: "20%",
                data: "InvoiceAmount",
                render: function (t, d, r) {
                    return '<h5 class="cursor-pointer amount-tooltip" data-amount="' + parseFloat(r.InvoiceAmount) + '" data-toggle="tooltip" data-placement="top" data-m="' + Fun_toWords(Number(r.InvoiceAmount)) + '" ><span class="badge badge-info"> ' + r.InvoiceAmount.toLocaleString("en") + '</span></h5>'
                }
            },
            {
                class: "text-center",
                width: "20%",
                data: "InvoiceDate",
                render: function (data, type, row) {
                    var date = new Date(data);
                    return date.toLocaleDateString();
                },
            },
            {
                class: "text-center",
                width: "20%",
                data: "TransactionAmount",
                render: function (t, d, r) {
                    return '<h5 class="cursor-pointer amount-tooltip" data-amount="' + parseFloat(r.TransactionAmount) + '" data-toggle="tooltip" data-placement="top" data-m="' + Fun_toWords(Number(r.TransactionAmount)) + '" ><span class="badge badge-info"> ' + r.TransactionAmount.toLocaleString("en") + '</span></h5>'
                }
            },
            {
                class: "text-center",
                width: "20%",
                data: "Balance",
                render: function (t, d, r) {
                    return '<h5 class="cursor-pointer amount-tooltip" data-amount="' + parseFloat(r.Balance) + '" data-toggle="tooltip" data-placement="top" data-m="' + Fun_toWords(Number(r.Balance)) + '" ><span class="badge badge-info"> ' + r.Balance.toLocaleString("en") + '</span></h5>'
                }
            }
        ],
        "order": [[0,1,2,3,"desc"]],

        autoWidth: false,
        wordWrap: true,
        initComplete: function () {
            ClosePleaseWait();
            $('.buttons-csv').html('<i class="fa fa-file-excel text-info fa-2x" />');
            $('.buttons-pdf').html('<i class="fa fa-file-pdf text-info fa-2x" />');
            $('.buttons-print').html('<i class="fa fa-print text-info fa-2x" />');
        }
    });
})();
