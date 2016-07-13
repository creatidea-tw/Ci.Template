$(function(){
	$("#TreeView li.trvw_gorup .trvw_item > span").click(function(){
	    if (!$(this).parent().parent().hasClass("trvw_showed")) {
	        $(this).parent().parent().addClass("trvw_showed").siblings().removeClass("trvw_showed").find("li.trvw_gorup").removeClass("trvw_showed");
	        // 	$(this).parent().parent().addClass("trvw_showed");
		}else{
			$(this).parent().parent().removeClass("trvw_showed").find("li.trvw_gorup").removeClass("trvw_showed");
		}
	});

    // stop toggle by child
	$("#TreeView li.trvw_gorup .trvw_item > span a").click(function (e) {
	    e.stopPropagation();
	});

    // open all parents
	$("#TreeView span a.btn-info").parents("li.trvw_gorup").addClass("trvw_showed");

    // open all
	$("#openAll").click(function () {
	    $("#TreeView li.trvw_gorup").addClass("trvw_showed");
	});

    // close all
	$("#closeAll").click(function () {
	    $("#TreeView li.trvw_gorup").removeClass("trvw_showed");
	});   
	

    // check all children
	$(".checkAll").change(function () {
	    $(this).closest("li.trvw_gorup").find("input:checkbox").prop("checked", $(this).is(":checked"));
	});
});