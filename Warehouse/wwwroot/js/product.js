$(() => {
    //Disable and Enable product
    $("[href='#']").on("click", function () {
        let $link = $(this);
        let $row = $(this).parent().parent().addClass(".select");
        let productId = $row.children().children().val();
        if ($link.text() === 'Enable') {
            $.ajax({
                url: '/Products/Enable',
                type: "POST",
                data: JSON.stringify(productId),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data) {
                        $link.text("Disable");
                        $row.removeClass();
                    }
                }
            })
        }
        else if ($link.text() === 'Disable') {
            $.ajax({
                url: '/Products/Disable',
                type: "POST",
                data: JSON.stringify(productId),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data) {
                        $link.text("Enable");
                        $row.removeClass();
                    }
                }
            })
        }
    });
});