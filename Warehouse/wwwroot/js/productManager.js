$("#ProductId").prop("disabled", true);
        $("#type").chosen();
        $("#ProductId").chosen();
        $("#WareHouseId").chosen();
        $(() => {
            $("#type").on("change", function () {
                TypeSpanDelete();
                NameSpanDelete();
                var selected = $(this).children("option:selected").val();
                $("li:contains('Select')").remove();
                $.ajax({
                    url: '/Products/Get',
                    type: "POST",
                    data: JSON.stringify(selected),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        $("#ProductId").empty();
                        $("#ProductId").attr("disabled", false);
                        $("#ProductId").append("<option selected disabled value='0'>Select name...</select>");
                        data.forEach(function (element) {
                            $("#ProductId").append("<option value=" + element.id + ">" + element.name + "</option>");
                        });
                        $("#ProductId").trigger("chosen:updated");
                    }
                })
            });
            $("#ProductId").on("change", () => {
                NameSpanDelete();
            })
            $("#WareHouseId").on("change", () => {
                HouseSpanDelete();
            });
        });
        function Stop() {
            if (NameNotValid() | TypeNotValid() | HouseNotValid()) {
                return false;
            }
        };

        //delete span tag
        function TypeSpanDelete() {
            $("#TypeDiv span.SelectError").remove();
        }
        function NameSpanDelete() {
            $("#NameDiv span.SelectError").remove();
        }
        function HouseSpanDelete() {
            $("#HouseDiv span.SelectError").remove();
        }

        //validation
        function TypeNotValid() {
            let type = $("#type").children("option:selected");
            if (type.val() == "") {
                TypeSpanDelete();
                $("#TypeDiv").append("<span class='SelectError'>*Product type not selected</span>");
                return true;
            }
            return false;
        }
        function NameNotValid() {
            let name = $("#ProductId").children("option:selected");
            if (name.val() == "") {
                NameSpanDelete();
                $("#NameDiv").append("<span class='SelectError'>*Product name not selected</span>")
                return true;
            }
            return false;
        }
        function HouseNotValid() {
            let house = $("#WareHouseId").children("option:selected");
            if (house.val() == "") {
                HouseSpanDelete();
                $("#HouseDiv").append("<span class='SelectError'>*WareHouse not selected</span>")
                return true;
            }
            return false;
        }