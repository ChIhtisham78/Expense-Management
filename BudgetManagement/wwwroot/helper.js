function ShowPleaseWait() {
    Swal.fire({
        title: 'Please Wait....!',
        imageUrl: '../Images/Loading1.gif',
        imageWidth: 100,
        imageHeight: 100,
        showConfirmButton: false,
        width: "24rem"
    });
}
function ClosePleaseWait() {
    Swal.close();
}

function FUN_GetDateAndTimeGerious(dtVal) {
    //return formate is mm/dd/yy hh:mm
    var ObjDateTime = new Date(dtVal);
    var datetime = (ObjDateTime.getMonth() + 1) + "/"
        + ObjDateTime.getDate() + "/"
        + ObjDateTime.getFullYear() + " "
        + FUN_Get_Time_AM_PM(ObjDateTime.getHours(), ObjDateTime.getMinutes());
    return datetime;
}

function FUN_Get_Time_AM_PM(H, M) {
    if (Number(H) < 13) {
        let Min = Number(M) < 10 ? "0" + M : M;
        if (Number(H) < 10) {
            return "0" + Number(H) + ":" + Min + " AM";
        } else {
            return H + ":" + Min + " AM";
        }

    } else if (Number(H) == 0) {
        let Min = Number(M) < 10 ? "0" + M : M;
        return "12 :" + Min + " AM";
    } else {
        let Hour = Number(H) - 12;
        let Min = Number(M) < 10 ? "0" + M : M;
        if (Number(Hour) < 10) {
            return "0" + Hour + ":" + Min + " PM";
        } else {
            return Hour + ":" + Min + " PM";
        }

    }
}

function FNIsFormValid(formForValidation) {
    if (formForValidation.valid()) {
        return true;
    } else {
        return false;
    }
}

function FUNClearAllModalFields(obj) {
    obj.find("option:selected").prop("selected", false);
    obj.find("span.is-invalid ").text("");
    obj.find(".is-invalid").removeClass("is-invalid");

    obj.find(".form-control").val("");

    obj.find("span.is-valid").text("");
    obj.find(".is-valid").val("");
    obj.find(".is-valid").removeClass("is-valid");
}

async function _AreYouSure(title, text, mode) {
    return await swal({
        title: title,
        text: text,
        icon: 'warning',
        buttons: true,
        dangerMode: true,
        closeOnEsc: false,
        closeOnClickOutside: false
    })
        .then((willDelete) => {
            return willDelete;
        });
}
async function AreYouSure(title, text, mode) {
    let Getresult = await _AreYouSure(title, text, mode);
    if (Getresult === true) {
        return true;
    } else {
        return false;
    }
}

function toastInfoMessage(message) {
    $("#toastInfo .toast-body").html(message);
    $("#toastInfo").toast({
        autohide: true,
        delay: 3000
    });
    $("#toastInfo").toast("show");
}

function toastErrorMessage(message) {
    $("#toastError .toast-body").html(message);
    $("#toastError").toast({
        autohide: true,
        delay: 3000
    });
    $("#toastError").toast("show");
}


function Fun_Clean_DDL(Dropdown_Id) {
    $('#' + Dropdown_Id).find('option').remove();
}

function Fun_Clean_And_Fill_DDL(Dropdown_Id, data) {
    $.each(data, function (index, Row) {
        $('#' + Dropdown_Id).append('<option value=' + Row.Value + '>' + Row.Text + '</option>');
    });
}
function Fun_Chose_option_to_DDL(Dropdown_Id) {
    $('#' + Dropdown_Id).append('<option value="" selected>--- Please Select---</option>');
}

function Fun_Clean_And_Fill_DDL_complete(Dropdown_Id, data) {
    Fun_Clean_DDL(Dropdown_Id);
    Fun_Chose_option_to_DDL(Dropdown_Id);
    Fun_Clean_And_Fill_DDL(Dropdown_Id, data);
}

function Fun_Clean_And_Fill_DDL_complete_Without_selection(Dropdown_Id, data) {
    Fun_Clean_DDL(Dropdown_Id);
    Fun_Clean_And_Fill_DDL(Dropdown_Id, data);
}

function Fun_Select_DDL(OptionId, Value) {
    $('#' + OptionId + ' > option').each(function () { if ($(this).val() == Value) { $(this).prop("selected", true); } });
}



function Fun_Select_MultipleSelectDDL(OptionId, Value) {
    $('#' + OptionId).multipleSelect('uncheckAll');
    $('#' + OptionId).multipleSelect('check', Value);
}

function FUN_GetJqueryDate(datePickerId) {
    var date = $('#' + datePickerId).datepicker('getDate');
    var utcDate = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes()));
    return utcDate;
}
function getDateWithFormate1(dt) {
    var currentdate = new Date(dt);
    let month = (currentdate.getMonth() + 1) < 10 ? "0" + (currentdate.getMonth() + 1) : (currentdate.getMonth() + 1);
    let day = currentdate.getDate() < 10 ? "0" + currentdate.getDate() : currentdate.getDate();
    var datetime = day + "/"
        + month + "/"
        + currentdate.getFullYear();
    return datetime;
}


var th = ['', 'Thousand', 'million', 'billion', 'trillion'];
var dg = ['zero', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine'];
var tn = ['ten', 'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen', 'seventeen', 'eighteen', 'nineteen'];
var tw = ['twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];

function Fun_toWords(s) {

    s = s.toString();
    s = s.replace(/[\, ]/g, '');
    if (s != parseFloat(s)) return 'not a number';
    var x = s.indexOf('.');
    var fulllength = s.length;

    if (x == -1) x = s.length;
    if (x > 15) return 'too big';
    var startpos = fulllength - (fulllength - x - 1);
    var n = s.split('');

    var str = '';
    var str1 = '';
    var sk = 0;
    for (var i = 0; i < x; i++) {
        if ((x - i) % 3 == 2) {
            if (n[i] == '1') {
                str += tn[Number(n[i + 1])] + ' ';
                i++;
                sk = 1;
            } else if (n[i] != 0) {
                str += tw[n[i] - 2] + ' ';

                sk = 1;
            }
        } else if (n[i] != 0) {
            str += dg[n[i]] + ' ';
            if ((x - i) % 3 == 0) str += 'hundred ';
            sk = 1;
        }
        if ((x - i) % 3 == 1) {
            if (sk) str += th[(x - i - 1) / 3] + ' ';
            sk = 0;
        }
    }
    if (x != s.length) {

        str += 'and ';
        str1 += 'cents ';
        var j = startpos;

        for (var i = j; i < fulllength; i++) {

            if ((fulllength - i) % 3 == 2) {
                if (n[i] == '1') {
                    str += tn[Number(n[i + 1])] + ' ';
                    i++;
                    sk = 1;
                } else if (n[i] != 0) {
                    str += tw[n[i] - 2] + ' ';

                    sk = 1;
                }
            } else if (n[i] != 0) {

                str += dg[n[i]] + ' ';
                if ((fulllength - i) % 3 == 0) str += 'hundred ';
                sk = 1;
            }
            if ((fulllength - i) % 3 == 1) {

                if (sk) str += th[(fulllength - i - 1) / 3] + ' ';
                sk = 0;
            }
        }
    }
    var result = str.replace(/\s+/g, ' ') + str1;
    return result;
}

function numberToWords(number) {
    var words = ['', 'One', 'Two', 'Three', 'Four', 'Five', 'Six', 'Seven', 'Eight', 'Nine', 'Ten',
        'Eleven', 'Twelve', 'Thirteen', 'Fourteen', 'Fifteen', 'Sixteen', 'Seventeen', 'Eighteen', 'Nineteen'];
    var tens = ['', '', 'Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninety'];
    function convertTwoDigits(num) {
        if (num < 20) {
            return words[num];
        } else {
            return tens[Math.floor(num / 10)] + ' ' + words[num % 10];
        }
    }
    function convertThreeDigits(num) {
        var Hundred = Math.floor(num / 100);
        var remainder = num % 100;
        var wordsArr = [];
        if (Hundred > 0) {
            wordsArr.push(words[Hundred] + ' Hundred');
        }
        if (remainder > 0) {
            wordsArr.push(convertTwoDigits(remainder));
        }
        return wordsArr.join(' ');
    }
    if (number === 0) {
        return 'zero';
    } else {
        var result = '';
        var Billion = Math.floor(number / 1000000000);
        var Million = Math.floor((number % 1000000000) / 1000000);
        var Thousand = Math.floor((number % 1000000) / 1000);
        var remainder = number % 1000;
        if (Billion > 0) {
            result += convertThreeDigits( Billion) + ' Billion ';
        }
        if (Million > 0) {
            result += convertThreeDigits(Million) + ' Million ';
        }
        if (Thousand > 0) {
            result += convertThreeDigits(Thousand) + ' Thousand ';
        }
        if (remainder > 0) {
            result += convertThreeDigits(remainder);
        }
        return result.trim();
    }
}
$('body').on('mouseover', 'td .amount-tooltip', function () {
    var amount = parseFloat($(this).data('amount'));
    $(this).attr('title', numberToWords(amount));
});