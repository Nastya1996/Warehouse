$("#ProductId").prop("disabled", true);
        $("#type").chosen();
        $("#ProductId").chosen();
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
        });
        function Stop() {
            if (NameNotValid() | TypeNotValid()) {
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