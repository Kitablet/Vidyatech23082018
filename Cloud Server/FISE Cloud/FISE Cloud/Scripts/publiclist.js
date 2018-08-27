var array = [];
var ItemsToRemove = {
    selectedItemCount: 0,
    selectedItemIds: "",

    selectItem: function (checkbox, event) {
        var id = $(checkbox).val();
        if ($(checkbox).is(":checked")) {
            if (!($.inArray(id, array) > -1)) {
                this.selectedItemCount++;
                array.push(id);
            }
        }
        else if (!($(checkbox).is(":checked"))) {
            if ($.inArray(id, array) > -1) {
                this.selectedItemCount--;
                index = $.inArray(id, array);
                array.splice(index, 1);
            }
        }
    },

    selectAll: function (checkbox, event) {
        array.length = 0;
        this.selectedItemCount = 0;
        if ($("#chk_select_all").is(":checked")) {
            $('.chkbox').each(function () {
                $(this).attr('checked', true);
                var id = $(this).val();
                array.push(id);
                ItemsToRemove.selectedItemCount++;
            });
        }
        else if (!($("#chk_select_all").is(":checked"))) {
            $('.chkbox').each(function () {
                $(this).attr('checked', false);
                ItemsToRemove.selectedItemCount = 0;
            });
        }
    },

    removeItems: function () {
        if (this.selectedItemCount > 0) {
            ItemsToRemove.selectedItemIds = array.join(",");
            var selectedIdsToRemove = ItemsToRemove.selectedItemIds;
            $('#remove-selected-items-form #selectedIdsToRemove').val(selectedIdsToRemove);
            $('#remove-selected-items-form').submit();
            return false;
        }
    }
}


//*********to maintain the relationship b/w the selectall and other checkboxes*********//
$("#chk_select_all").change(function () {  //"select all" change
    var status = this.checked; // "select all" checked status
    $('.chkbox').each(function () { //iterate all listed checkbox items
        this.checked = status; //change ".checkbox" checked status
    });
});

$('.chkbox').change(function () { //".checkbox" change
    //uncheck "select all", if one of the listed checkbox item is unchecked
    if (this.checked == false) { //if this item is unchecked
        $("#chk_select_all")[0].checked = false; //change "select all" checked status to false
    }

    //check "select all" if all checkbox items are checked
    if ($('.chkbox:checked').length == $('.chkbox').length) {
        $("#chk_select_all")[0].checked = true; //change "select all" checked status to true
    }
});