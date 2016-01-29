$(document).ready(function () {
    $("#apply").addClass("gn_center_select");
        if (document.documentElement.clientWidth > 1280) {
            $(".W_main").css("width", "1300");
        }

    });
    function trim(str) { //删除左右两端的空格
        return str.replace(/(^\s*)|(\s*$)/g, "");
    }