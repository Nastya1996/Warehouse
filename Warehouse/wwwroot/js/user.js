$(() => {
    $("[href='#']").on("click", function () {
        let $link = $(this);
        $(this).parent().parent().addClass(".select");
        $link.addClass('context-menu-one');
        $.contextMenu({
            selector: '.context-menu-one',
            trigger: 'left',
            callback: function (key, options) {
            },
            position: function (opt, x, y) {
                opt.$menu.css({ top: y + 10, left: x - 90 });
            },
            className: "contextmenu - custom contextmenu-custom__highlight",
            items: {
                "Yes": {
                    name: 'Yes', icon: 'fas fa-check-circle',
                    callback: function () {
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
                                        $.confirm({
                                            icon: 'fas fa-check-circle',
                                            title: 'Warehouse!',
                                            content: 'User has been activated!',
                                            type: 'dark',
                                            typeAnimated: true,
                                            closeAnimation: 'rotateXR',
                                            buttons: {
                                                close: {
                                                    btnClass: "btn-dark",
                                                    action: function () {
                                                        $link.text("Disable").attr("class", "Disable btn");
                                                    }
                                                }
                                            }
                                        });
                                        $row.removeClass();
                                    }
                                    else {
                                        $.confirm({
                                            icon: 'fas fa-exclamation-triangle',
                                            title: 'Warehouse!',
                                            content: 'An error occurred. Please, try again!',
                                            type: 'red',
                                            typeAnimated: true,
                                            closeAnimation: 'rotateXR',
                                            buttons: {
                                                close: {
                                                    btnClass: "btn-red",
                                                    action: function () {
                                                        $link.text("Enable").attr("class", "Enable btn");
                                                    }
                                                }
                                            }
                                        });
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
                                        $.confirm({
                                            icon: 'fas fa-check-circle',
                                            title: 'Warehouse!',
                                            content: 'User has been deactivated!',
                                            type: 'dark',
                                            typeAnimated: true,
                                            closeAnimation: 'rotateXR',
                                            buttons: {
                                                close: {
                                                    btnClass: "btn-dark",
                                                    action: function () {
                                                        $link.text("Enable").attr("class", "Enable btn");
                                                    }
                                                }
                                            }
                                        });
                                        $row.removeClass();
                                    }
                                    else {
                                        $.confirm({
                                            icon: 'fas fa-exclamation-triangle',
                                            title: 'Warehouse!',
                                            content: 'An error occurred. Please, try again!',
                                            type: 'red',
                                            typeAnimated: true,
                                            closeAnimation: 'rotateXR',
                                            buttons: {
                                                close: {
                                                    btnClass: "btn-red",
                                                    action: function () {
                                                        $link.text("Disable").attr("class", "Disable btn");
                                                    }
                                                }
                                            }
                                        });
                                    }
                                }
                            })
                        }
                        $row.removeClass();
                    }
                },
                "sep1": "---------",
                "Cansel": { name: "Cansel", icon: 'far fa-times-circle' }
            }
        });
        
    });
});