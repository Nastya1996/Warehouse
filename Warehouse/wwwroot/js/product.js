$("#ProductTypeId").chosen({
    search_contains: true
});
$("#UnitId").chosen({
    search_contains:true
});
$(() => {
    //Disable and Enable product
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
            items: {
                "Yes": {
                    name: "Yes", icon: "fas fa-check-circle",
                    callback: function (key, options) {
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
                                        $.confirm({
                                            icon: 'fas fa-check-circle',
                                            title: 'Warehouse!',
                                            content: 'Product has been activated!',
                                            type: 'blue',
                                            typeAnimated: true,
                                            closeAnimation: 'rotateXR',
                                            buttons: {
                                                close: {
                                                    btnClass: "btn-blue",
                                                    action: function () {
                                                    }
                                                }
                                            }
                                        });
                                        $link.text("Disable").css("color", "red");
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
                                                    }
                                                }
                                            }
                                        });
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
                                        $.confirm({
                                            icon:'fas fa-check-circle',
                                            title: 'Warehouse',
                                            content: 'Product has been deactivated!',
                                            type: 'blue',
                                            typeAnimated: true,
                                            closeAnimation: 'rotateXR',
                                            buttons: {
                                                close: {
                                                    btnClass: "btn-blue",
                                                    action: function () {
                                                    }
                                                }
                                            }
                                        });
                                        $link.text("Enable").css("color", "green");
                                        $row.removeClass();
                                    }
                                    else {
                                        $.confirm({
                                            icon:'fas fa-exclamation-triangle',
                                            title: 'Warehouse',
                                            content: 'An error occurred. Please, try again!',
                                            type: 'red',
                                            typeAnimated: true,
                                            closeAnimation: 'rotateXR',
                                            buttons: {
                                                close: {
                                                    btnClass: "btn-red",
                                                    action: function () {
                                                    }
                                                }
                                            }
                                        });
                                    }
                                }
                            })
                        }
                        $row.removeClass();
                        $link.removeClass();
                    }
                },
                "sep1": "---------",
                "Cansel": { name: "Cansel", icon: 'far fa-times-circle' }
            }
        });
        
    });
});

//Product Create selets tags validation
function PrCreateStop() {
    if (TypeNotValid() | UnitNotValid()) {
        return false;
    }
}

//validation functions
function TypeNotValid() {
    let type = $("#ProductTypeId").children("option:selected");
    if (type.val() == "") {
        TypeSpanDelete();
        $("#PrMngTypeDiv").append("<span class='SelectError'>*Product type not selected</span>");
        return true;
    }
    return false;
}
$("#ProductTypeId").on("change", () => {
    TypeSpanDelete();
});

function UnitNotValid() {
    let unit = $("#UnitId").children("option:selected");
    if (unit.val() == "") {
        UnitSpanDelete();
        $("#PrMngUnitDiv").append("<span class='SelectError'>*Unit not selected</span>");
        return true;
    }
    return false;
}
$("#UnitId").on("change", () => {
    UnitSpanDelete();
});

//delete span tag
function TypeSpanDelete() {
    $("#PrMngTypeDiv span.SelectError").remove();
}
function UnitSpanDelete() {
    $("#PrMngUnitDiv span.SelectError").remove();
}