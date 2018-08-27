$(document).ready(function () {
    function close_accordion_section() {
        $('.accordion .accordion-section-title').removeClass('active');
        $('.accordion .accordion-section-content').slideUp(300).removeClass('open');
    }

    $(".contactusText").on('change keyup', function () {
        if (isEmpty($(this).val())) {
            $(this).next().css('opacity', '0.5');
        }
        else {
            $(this).next().css('opacity', '1');
        }
    });

    $('.accordion-section-title').click(function (e) {
        var currentAttrValue = $(this).attr('href');

        if ($(e.target).is('.active')) {
            close_accordion_section();
        } else {
            close_accordion_section();
            $(this).addClass('active');
            $('.accordion ' + currentAttrValue).slideDown(300).addClass('open');
        }
        e.preventDefault();
    });
	
	$('textarea').on('keyup paste', function() {
		var $el = $(this),
			offset = $el.innerHeight() - $el.height();

		if ($el.innerHeight < this.scrollHeight) {
		  //Grow the field if scroll height is smaller
		  $el.height(this.scrollHeight - offset);
		} else {
		  //Shrink the field and then re-set it to the scroll height in case it needs to shrink
		  $el.height(1);
		  $el.height(this.scrollHeight - offset);
		}
	});
});

function isEmpty(value) {
    return (value == null || value === '');
}