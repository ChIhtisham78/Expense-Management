$(document).ready(function () {
    populateDropdown();
    initializeTables();
    function populateDropdown() {
        $.ajax({
            url: '../api/Getddl/ExpenseAccounts',
            type: 'GET',
            dataType: 'json',

            success: function (data) {
                $('#filter-dropdown').append('<option value="0">Select All</option>');

                data.forEach(function (expenseAccount) {
                    $('#filter-dropdown').append('<option value="' + expenseAccount.AccountId + '">' + expenseAccount.AccountName + '</option>');
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error:', textStatus, errorThrown);
            }
        });
    }

    $('#btn-reports').click(function () {
        printReports();
    });


    function printReports() {
        var inwardData = ObjInwardTable.rows().data().toArray();
        var outwardData = ObjOutWardTable.rows().data().toArray();
        var remainingBalance = 0;

        var formattedData = [];
        var combinedData = [...inwardData, ...outwardData];
        combinedData.sort((a, b) => new Date(a.Date) - new Date(b.Date));

        combinedData.forEach(row => {
            if (row.hasOwnProperty('TransactionDate')) {
                remainingBalance += parseFloat(row.Amount); 
                formattedData.push({
                    Date: row.TransactionDate,
                    Description: row.Desc,
                    Credited: row.Amount,
                    Debited: '',
                    Balance: remainingBalance.toFixed(2)
                });
            } else {
                remainingBalance -= parseFloat(row.Amount); 
                formattedData.push({
                    Date: row.Date,
                    Description: row.Description,
                    Credited: '',
                    Debited: row.Amount,
                    Balance: remainingBalance.toFixed(2)
                });
            }
        });
        var printContent = `
<div style="font-family: Arial, sans-serif; padding: 20px;">
    <div style="text-align: center;">
        <img src="/Images/buhhtech_logo.jpeg" alt="BUHTECH" style="width: 75px; margin-bottom: 3px;">
        <h1 style="margin-bottom: 5px;">Reports E Statement</h1>        
        <p style="margin: 0;">Statement Period: Monthly</p>
    </div>
    
    <div style="margin-top: 20px;">
        <div style="display: flex; justify-content: space-between;">
            <div>
                <p style="margin-bottom: 5px;"><strong>Remaining Balance:</strong> PKR:${remainingBalance.toFixed(2)}</p>
            </div>
        </div>
    </div>
    
    <div style="margin-top: 20px;">
        <h2 style="margin-bottom: 10px; text-align: center;">Transactions</h2>
<hr style="border: none; border-top: 2px solid black; width: 100%; margin: auto;"/>

        <table style="width:100%;">
            <tr>
                <th style="border: 1px solid #ddd; padding: 8px;">Date</th>
                <th style="border: 1px solid #ddd; padding: 8px;">Description</th>
                <th style="border: 1px solid #ddd; padding: 8px;">Credited</th>
                <th style="border: 1px solid #ddd; padding: 8px;">Debited</th>
                <th style="border: 1px solid #ddd; padding: 8px;">Balance</th>
            </tr>
            ${formattedData.map(row => `
                <tr>
                    <td style="border: 1px solid #ddd; padding: 8px;">${row.Date}</td>
                    <td style="border: 1px solid #ddd; padding: 8px;">${row.Description}</td>
                    <td style="border: 1px solid #ddd; padding: 8px;">${row.Credited}</td>
                    <td style="border: 1px solid #ddd; padding: 8px;">${row.Debited}</td>
                    <td style="border: 1px solid #ddd; padding: 8px;">${row.Balance}</td>
                </tr>
            `).join('')}
        </table>
    </div>
</div>
`;
        var printWindow = window.open('', '_blank');
        printWindow.document.open();
        printWindow.document.write('<html><head><title>Print Reports</title></head><body style="font-size: 14px;">' + printContent + '</body></html>');
        printWindow.document.close();
        printWindow.print();
    }


    $('#filter-dropdown').change(function () {
        var accountId = $(this).val();
        if (accountId) {
            $('#Selected').removeClass('d-none');
            $('#inward-table').removeClass('d-none');
            $('#outward-table').removeClass('d-none');
            reloadTableData(accountId, true);
            reloadTableData(accountId, false);
            calculateBalance(accountId);
        } else {
            $('#Selected').addClass('d-none');
            $('#inward-table').addClass('d-none');
            $('#outward-table').addClass('d-none');
        }
    });

    function reloadTableData(accountId, isInward) {
        var table;
        if (isInward) {
            table = ObjInwardTable;
        } else {
            table = ObjOutWardTable;
        }
        table.ajax.url(isInward ? "../api/ManageReports/GetReport?accountId=" + accountId : "../api/ManageReports/ReportGenerator?accountId=" + accountId).load();
        calculateBalance(accountId);
    }


    function calculateBalance(accountId) {
        $.ajax({
            url: '../api/ManageReports/GetInitialBalance?accountId=' + accountId,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                $('#balance').text(data.toFixed(2));
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error:', textStatus, errorThrown);
            }
        });
    }


    function initializeTables() {
        $(function () {
            ObjInwardTable = $('#inward-table').DataTable({
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
                        exportOptions: { columns: [0, 1, 2, 3, 4] }
                    }
                ],
                "lengthMenu": [10, 25, 50, 100],
                ajax: {
                    url: "../api/ManageReports/GetReport",
                    type: 'GET',
                    dataSrc: "",
                },
                columns: [
                    { data: "TransactionId", ordering: false },
                    {
                        data: "TransactionDate",
                        render: function (data, type, row) {
                            var date = new Date(data);
                            return date.toLocaleDateString();
                        }
                    },
                    {
                        data: "Amount",
                        render: function (data, type, row) {
                            return '<span class="badge amount-tooltip" data-amount="' + parseFloat(row.Amount) + '" style="font-size: 12px; background-color: green; color: white;">' + parseFloat(data).toLocaleString() + '</span>';
                        }
                    },
                    { data: "Payee" },
                    { data: "Recipient" },
                    { data: "Desc" }
                ],

                initComplete: function () {
                    ClosePleaseWait();
                    $('.buttons-csv').html('<i class="fa fa-file-excel text-info fa-2x" />');
                    $('.buttons-pdf').html('<i class="fa fa-file-pdf text-info fa-2x" />');
                    $('.buttons-print').html('<i class="fa fa-print text-info fa-2x" />');
                },
                autoWidth: false,
                wordWrap: true


            });
        });


        $(function () {
            ObjOutWardTable = $('#outward-table').DataTable({
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
                        exportOptions: { columns: [0, 1, 2, 3, 4, 5] }
                    }
                ],
                "lengthMenu": [10, 25, 50, 100],
                ajax: {
                    url: "../api/ManageReports/ReportGenerator",
                    type: 'GET',
                    dataSrc: "",
                },
                columns: [
                    { data: "Id" },

                    { data: "AccountName" },
                    { data: "Recipient" },
                    {
                        data: "Amount",
                        render: function (data, type, row) {
                            return '<span class="badge badge-primary amount-tooltip" data-amount="' + parseFloat(row.Amount) + '" style="font-size: 12px;">' + parseFloat(data).toLocaleString() + '</span>';
                        }
                    },
                    {
                        data: "Date",
                        render: function (data, type, row) {
                            var date = new Date(data);
                            return date.toLocaleDateString();
                        }
                    },
                    { data: "Description" },

                ],
                initComplete: function () {
                    ClosePleaseWait();
                    $('.buttons-csv').html('<i class="fa fa-file-excel text-info fa-2x" />');
                    $('.buttons-pdf').html('<i class="fa fa-file-pdf text-info fa-2x" />');
                    $('.buttons-print').html('<i class="fa fa-print text-info fa-2x" />');
                },
                autoWidth: false,
                wordWrap: true
            });
        });
    }
});