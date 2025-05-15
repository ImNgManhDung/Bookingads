$(document).ready(function () {
    const loadHTML = `<p class="text-center text-success" style="font-weight: bold;">
                                <span class="glyphicon glyphicon-refresh spinning"></span> Đang tải dữ liệu...
                              </p>`;
    function filter(page = 1) {
        const url = $('#formFilter').attr("action");
        $('#filterResult').html(loadHTML);

        const currentPage = +($('#currentPage').val());
        const disableStyle = "disabled";
        if (currentPage > 1) {
            $('#btnPrev').removeClass(disableStyle);
        } else {
            $('#btnPrev').addClass(disableStyle);
        }

        const data = {
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

                    $('.btnEditCatelogProduct').click(function () {
                        const url = $(this).attr('data-url');
                        const catelogProductID = +($(this).attr('data-id'))
                        const data = {
                            CatelogProductsID: catelogProductID,
                            CatelogName: $(`#catelogName-${catelogProductID}`).val(),
                        };
                        $(`#edit-${data.catelogID}`).removeClass("in");

                        $.ajax({
                            contentType: 'application/json',
                            url: url,
                            method: "POST",
                            data: JSON.stringify(data),
                            success: function (res) {
                                if (res) {
                                    $('.modal-backdrop').remove();
                                    $('#errorMsg').show();
                                    $('#errorMsg').text(res);

                                    setTimeout(() => {
                                        $('#errorMsg').text('');
                                        $('#errorMsg').hide();
                                    }, 2000);

                                    $('#catelogNameAdd').val('');
                                    return;
                                }

                                $('#errorMsg').text('');
                                $('#errorMsg').hide();

                                $('#filterResult').html(loadHTML);

                                $('.modal-backdrop').remove();

                                filter();
                            },
                            error: function (err) {
                                console.log("===> err: ", err);
                            }
                        });
                    });

                    $('.btnDeleteCatelogProduct').click(function () {
                        const url = $(this).attr('data-url');
                        const data = {
                            catelogID: +($(this).attr('data-id')),
                        };

                        $(`#delete-${data.catelogID}`).removeClass("in");

                        $.ajax({
                            contentType: 'application/json',
                            url: url,
                            method: "POST",
                            data: JSON.stringify(data),
                            success: function (res) {
                                // console.log("===> res: ", res);
                                $('#filterResult').html(loadHTML);

                                // clean modal-backdrop
                                $('.modal-backdrop').remove();

                                filter();

                            },
                            error: function (err) {
                                console.log("===> err: ", err);
                            }
                        });
                    });

                    $('.btnAddCatelogProduct').click(function () {
                        const url = $(this).attr('data-url');
                        const data = {
                            CatelogName: $('#catelogNameAdd').val(),
                        };
                        // $(`#add`).removeClass("in");

                        console.log('===> add catelog product: ', data);

                        $.ajax({
                            contentType: 'application/json',
                            url: url,
                            method: "POST",
                            data: JSON.stringify(data),
                            success: function (res) {
                                if (res) {
                                    $('.modal-backdrop').remove();
                                    $('#errorMsg').show();
                                    $('#errorMsg').text(res);

                                    setTimeout(() => {
                                        $('#errorMsg').text('');
                                        $('#errorMsg').hide();
                                    }, 2000);

                                    $('#catelogNameAdd').val('');
                                    return;
                                }

                                $('#errorMsg').text('');
                                $('#errorMsg').hide();

                                // console.log('res: ', res);

                                $('#filterResult').html(loadHTML);

                                filter();

                                $('#catelogNameAdd').val('');
                            },
                            error: function (err) {
                                console.log("===> err: ", err);
                            }
                        });

                        $('.modal-backdrop').remove();
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

    // process filter
    $('#formFilter').submit(function (e) {
        e.preventDefault();
        filter($('#currentPage').val());
    });

    $('#searchValue').on('input', function (e) {
        const value = e.target.value;
        if (!value)
            filter($('#currentPage').val());
    });

    $('#btnFilter').click(function () {
        filter($('#currentPage').val());
    });
});