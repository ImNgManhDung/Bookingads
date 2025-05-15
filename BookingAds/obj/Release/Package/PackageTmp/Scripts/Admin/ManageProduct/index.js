$(document).ready(function () {
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
            SortField: $('#sort').val().split('-')[0],
            SortType: $('#sort').val().split('-')[1],
            CatelogProductsID: $('#CatelogProductsID').val(),
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

                    // restore or soft delete product
                    $('.btnUpdateProductStatus').click(function () {
                        const url = $(this).attr('data-url');
                        const data = {
                            productID: +($(this).attr('data-id')),
                        };
                        $(`#delete-${data.productID}`).removeClass("in");

                        $.ajax({
                            contentType: 'application/json',
                            url: url,
                            method: "POST",
                            data: JSON.stringify(data),
                            success: function (res) {
                                console.log("===> res: ", res);
                                filter();
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

    // process filter
    $('#sort').change(function () {
        filter($('#currentPage').val());
    });
    $('#CatelogProductsID').change(function () {
        filter($('#currentPage').val());
    });

    $('#btnFilter').click(function () {
        filter($('#currentPage').val());
    });
});