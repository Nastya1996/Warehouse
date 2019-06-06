$(() => {
    $("[href='#']").on("click", function () {
        let $link = $(this);
        let $row = $(this).parent().parent().addClass(".select");
        let userId = $row.children().children().val();
        if ($link.text() === "Enable") {
            $.ajax({
                url: '/User/Enable',
                type: "POST",
                data: JSON.stringify(userId),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data) {
                        $("#userText").text("User has been activated!");
                        $("#userModal").modal();
                        $link.text("Disable").css("color", "red");
                        $row.removeClass();
                    }
                }
            });
        }
        else if ($link.text() === 'Disable') {
            $.ajax({
                url: '/User/Disable',
                type: "POST",
                data: JSON.stringify(userId),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data) {
                        $("#userText").text("User has been deactivated!");
                        $("#userModal").modal();
                        $link.text("Enable").css("color", "green");
                        $row.removeClass();
                    }
                }
            })
        }
    });
});