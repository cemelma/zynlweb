$(function () {
    var status = $("#ProcessMessage").val();
    var result = $("#hdsaveResult").val();
    var prId = $("#hdProductId").val();
    $("#imgloader").css("display", "none");
    if (status == "True" || status == "true")
        MessageBox("İşlem Başarıyla Tamamlandı", "info");
    else if (status == "False" || status == "false")
        MessageBox("İşlem Sırasında Bir Hata Oluştu.", "alert");

    $("#tabs").tabs();

    $.ajax({
        type: 'POST',
        url: '/Product/GetDetail',
        data: '{id:"' + prId + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: 'html',
        success: function (result) {
            $.unblockUI();
            $("#resultTable").html(result);

            //$("#ProductSubGroupId").empty().append($("<option></option>").val("").html("Ürün Alt Grubunu Seçiniz..."));
            //$("#g1").css("display", "block");
            //$.each(result, function (i, item) {
            //    $("#ProductSubGroupId").append($("<option></option>").val(item.Value).html(item.Text));
            //});
            //$("#ProductSubGroupId").removeAttr("disabled");
            //$("#imgloader2").css("display", "none");
        },
        error: function () {
            $.unblockUI();
        }
    });



    $("#btnclear").click(function () {
        $("#txtcode").val("");
        $("#txtmalzeme").val("");
        $("#txtebat").val("");
        $("#txtagirlik").val("");
        $("#txtton").val("");
        $("#txtfiyat").val("");
        $("#txtbirim").val("");
        //$("#txtrenk").css("background-color", "white");
        $("#txtrenk").val("");

    });

    $("#btnsaveProp").click(function () {
        var code = $("#txtcode").val();
        var malzeme = $("#txtmalzeme").val();
        var ebat = $("#txtebat").val();
        var agirlik = $("#txtagirlik").val();
        var ton = $("#txtton").val();
        var fiyat = $("#txtfiyat").val();
        var birim = $("#txtbirim").val();
        var renk = $("#txtrenk").css("background-color");
        //var renkadi = $("#txtrenkadi").val();

        if (code == "" || malzeme == "" || fiyat == "" || ebat == "" || agirlik == "" || ton == "" || birim == "" || renk == "") {
            alert("Kayıt eklemek için tüm alanları doldurmalısınız");
            return false;
        }
        $.blockUI({
            css: { backgroundColor: 'transparent', border: 'none' },
            message: "<div id='circleG'><div id='circleG_1' class='circleG'></div><div id='circleG_2' class='circleG'></div><div id='circleG_3' class='circleG'></div></div> "
        });
        $.ajax({
            type: 'POST',
            url: '/Product/SaveDetail',
            data: '{code:"' + code + '",malzeme:"' + malzeme + '",birim:"' + birim + '",ebat:"' + ebat + '",agirlik:"' + agirlik + '",ton:"' + ton + '",fiyat:"' + fiyat + '",renk:"' + renk + '",prId:"' + prId + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: 'html',
            success: function (result) {
                $.unblockUI();
                $("#resultTable").html(result);

                //$("#ProductSubGroupId").empty().append($("<option></option>").val("").html("Ürün Alt Grubunu Seçiniz..."));
                //$("#g1").css("display", "block");
                //$.each(result, function (i, item) {
                //    $("#ProductSubGroupId").append($("<option></option>").val(item.Value).html(item.Text));
                //});
                //$("#ProductSubGroupId").removeAttr("disabled");
                //$("#imgloader2").css("display", "none");
            },
            error: function () {
                $.unblockUI();
            }
        });

    });




});
