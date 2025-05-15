$(document).ready(function () {
    // process checkbox
    $("#mytable #checkall").click(function () {
        if ($("#mytable #checkall").is(':checked')) {
            $("#mytable input[type=checkbox]").each(function () {
                $(this).prop("checked", true);
            });

        } else {
            $("#mytable input[type=checkbox]").each(function () {
                $(this).prop("checked", false);
            });
        }
    });
    $("[data-toggle=tooltip]").tooltip();

    // load html
    const loadHTML = `<p class="text-center text-success" style="font-weight: bold;">
                                                    <span class="glyphicon glyphicon-refresh spinning"></span> Đang tải dữ liệu...
                                                  </p>`;
    function filter(page = 1) {
        const url = $('#formFilter').attr("action");
        console.log("url: ", url);

        // add load
        $('#filterResult').html(loadHTML);

        // check status of button previous pagination
        const currentPage = +($('#currentPage').val());
        const disableStyle = "disabled";
        if (currentPage > 1) {
            $('#btnPrev').removeClass(disableStyle);
        } else {
            $('#btnPrev').addClass(disableStyle);
        }

        const data = {
            Gender: $('#gender').val(),
            Field: $('#field').val(),
            SearchValue: $("#searchValue").val().trim(),
            Page: page,
            PageSize: $('#currentPageSize').val()
        };
        console.log("data: ", data);

        $.ajax({
            contentType: 'application/json',
            url: url,
            method: "POST",
            data: JSON.stringify(data),
            success: function (res) {
                setTimeout(function () {
                    $('#filterResult').html(res);

                    // check status of button previous pagination
                    const currentPage = +($('#currentPage').val());
                    const disableStyle = "disabled";
                    if (currentPage > 1) {
                        $('#btnPrev').removeClass(disableStyle);
                    } else {
                        $('#btnPrev').addClass(disableStyle);
                    }

                    // process lock and unlock account
                    $('.btnUpdateAccountStatus').click(function () {
                        const url = $(this).attr('data-url');
                        const data = {
                            employeeID: +($(this).attr('data-id')),
                        };
                        console.log("update account id: ", data, " with url: ", url);
                        $(`#status-${data.employeeID}`).removeClass("in");

                        $.ajax({
                            contentType: 'application/json',
                            url: url,
                            method: "POST",
                            data: JSON.stringify(data),
                            success: function (res) {
                                filter();
                                //$('#filterResult').html(res);
                                //$(`#status-${data.employeeID}`).removeClass("modal-backdrop");
                                console.log("===> res: ", res);
                            },
                            error: function (err) {
                                console.log("===> err: ", err);
                            }
                        });
                    });

                    // action pagination
                    $('#currentPage').change(function () {
                        const currentPage = $(this).val();
                        if (!currentPage) return;
                        filter($('#currentPage').val());
                    });

                    $('#btnPrev').click(function () {
                        const currentPage = +($('#currentPage').val());
                        if (currentPage == 1) return;
                        $('#currentPage').val(currentPage - 1);
                        filter($('#currentPage').val());
                    });

                    $('#btnNext').click(function () {
                        const currentPage = +($('#currentPage').val());
                        $('#currentPage').val(currentPage + 1);
                        filter($('#currentPage').val());
                    });

                }, 500);
                // console.log("===> res: ", res);


            },
            error: function (err) {
                console.log("===> err: ", err);
            }
        });

        // clean modal-backdrop
        $('.modal-backdrop').remove();
    }

    filter(1);

    // filter submit
    $('#formFilter').submit(function (e) {
        e.preventDefault();
        filter($('#currentPage').val());
    });

    $('#searchValue').on('input', function (e) {
        const value = e.target.value;
        if (!value) filter($('#currentPage').val());
    });

    $('#btnFilter').click(function () {
        filter($('#currentPage').val());
    });
});