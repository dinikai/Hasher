$('input[type="file"]').on('change', function() {
	$(this).next().html(this.files.length + " files selected");
});

function popup(text) {
	$("#popupWindow").addClass("--popup-window-visible");
	$("#popupText").html(text);
}

function closePopup() {
	$("#popupWindow").removeClass("--popup-window-visible");
}