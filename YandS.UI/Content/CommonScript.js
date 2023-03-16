$.ajaxSetup({ cache: false });
$.blockUI.defaults.css = {
    padding: 0,
    margin: 0,
    width: '0%',
    top: '40%',
    left: '50%',
    textAlign: 'center',
    color: '#fff',
    border: '0px',
    backgroundColor: '#fff',
    opacity: 0.9,
    cursor: 'wait'
};

$.blockUI.defaults.message = '<i class="fas fa-2x fa-sync-alt fa-spin"></i>';

$(document).ready(function () {
    console.log("Common Script Page");
    $(document).ajaxStart($.blockUI).ajaxStop($.unblockUI);

    $.fn.regexMask = function (mask) {
        $(this).keypress(function (event) {
            if (!event.charCode) return true;
            var part1 = this.value.substring(0, this.selectionStart);
            var part2 = this.value.substring(this.selectionEnd, this.value.length);
            if (!mask.test(part1 + String.fromCharCode(event.charCode) + part2))
                return false;
        });
    };
    /**
     var IsDigitmask = new RegExp(/^\d*\.?(?:\d{1,4})?$/);
        var IsDigitUptoTen = new RegExp('^([1-9]|10)$');
        var IsAlphaNumeric = new RegExp('^[A-Za-z0-9 ]*$');
        var AlphaNumericSpecial = new RegExp(/^[-@@.\/#&+()\[\]\w\s]*$/)
        $(".IsAlphaNumeric").regexMask(IsAlphaNumeric);
        $(".IsDigitUptoTen").regexMask(IsDigitUptoTen);
        $(".dlsDigits").regexMask(IsDigitmask);
        $(".IsAlphaNumericSpecial").regexMask(AlphaNumericSpecial);
     
    function round2Fixed(value) {
        value = +value;

        if (isNaN(value))
            return NaN;

        // Shift
        value = value.toString().split('e');
        value = Math.round(+(value[0] + 'e' + (value[1] ? (+value[1] + 3) : 3)));

        // Shift back
        value = value.toString().split('e');
        return (+(value[0] + 'e' + (value[1] ? (+value[1] - 3) : -3))).toFixed(1);
    }

    function PadDigits(n, totalDigits) {
        n = n.toString();
        var pd = '';
        if (totalDigits > n.length) {
            for (i = 0; i < (totalDigits - n.length); i++) {
                pd += '0';
            }
        }
        return pd + n.toString();
    }
*/
});