// format date: dd/MM/yyyy HH:mm:ss tt
function formatDate(now) {
    const day = now.getDate() < 10 ? ('0' + now.getDate()) : now.getDate();
    const month = (now.getMonth() + 1) < 10 ? ('0' + (now.getMonth() + 1)) : (now.getMonth() + 1);
    const year = now.getFullYear();
    let hours = now.getHours();
    const minutes = now.getMinutes() < 10 ? ('0' + now.getMinutes()) : now.getMinutes();
    const seconds = now.getSeconds() < 10 ? ('0' + now.getSeconds()) : now.getSeconds();
    const times = hours < 12 ? 'AM' : 'PM';
    //hours = hours % 12;
    //hours = hours ? hours : 12;
    hours = hours < 10 ? '0' + hours : hours;

    return `${day}/${month}/${year} ${hours}:${minutes}:${seconds} ${times}`;
}

// process notification order
const orderHub = $.connection.orderHub;
if (orderHub) {
    orderHub.client.NotifyOrderMsg = function (data) {
        // fromUserName, notificationData, toUserNames
        const res = JSON.parse(data.NotificationData);
        // console.log('[HistoryProductOrder] ===> order info in employee: ', data);
        console.log('[SearchRequest] ===> res in employee: ', res);

        const currentPage = +($('#currentPage').val());
        if (currentPage == 1) {
            $(`#btnAction-${res.orderID}`).empty();
            $(`#status-${res.orderID} span`).attr('class', `badge bg-${res.formatStatus}`);
            console.log('>>>> trạng thái gửi về: ', res.status);
            $(`#status-${res.orderID} span`).text(res.status);
            if (res.nextStatus === 1) {
                $(`#textlink-${res.orderID}`).text(res.textlink);
            }
        }

        if (res.payType.includes('Ví')) {
            let currentCoin = $('#employeeCoin').text().replace(',00 ₫', '').replace('.', '') | 0;
            let newBalance = (res.productPrice + currentCoin).toFixed(2)
                .replace(/\d(?=(\d{3})+\.)/g, '$&.')
                .replaceAll('.', ',')
                .replace(',', '.');
            $('#employeeCoin').text(`${newBalance} ₫`);
        }

        let badgeNotifyOrder = +($('#badgeNotifyOrder').text()) + 1;
        $('#badgeNotifyOrder').text(badgeNotifyOrder);

        $('#activeNotify').show();
        $('#badgeNotifyOrder').show();
    }
}

// load html
const loadHTML = `<p class="text-center text-success" style="font-weight: bold;">
                                            <span class="glyphicon glyphicon-refresh spinning"></span> Đang tải dữ liệu...
                                          </p>`;

function filter() {
    const url = $('#formFilter').attr("action");
    console.log("url: ", url);

    // add load
    $('#filterResult').html(loadHTML);

    // check status button previous pagination
    const currentPage = +($('#currentPage').val());
    const disableStyle = "disabled";
    if (currentPage > 1) {
        $('#btnPrev').removeClass(disableStyle);
    } else {
        $('#btnPrev').addClass(disableStyle);
    }

    const data = {
        DateStart: $('#startDatetime').val(),
        DateEnd: $('#endDatetime').val(),
        OrderStatus: $("#orderStatus").val(),
        SearchValue: $("#searchValue").val(),
        Page: $('#currentPage').val(),
        PageSize: $('#currentPageSize').val()
    };
    console.log("data: ", data);

    $.ajax({
        contentType: 'application/json',
        url: url,
        method: "POST",
        data: JSON.stringify(data),
        success: function (res) {
            if (!res.includes('class="table table-bordered"')) {
                alert(res);
                $('#filterResult').empty();
                return;
            }
            setTimeout(function () {

                $('#filterResult').html(res);

                // clean modal-backdrop
                $('.modal-backdrop').remove();

                $.connection.hub.start()
                    .done(function () {
                        console.log("Đã kết nối Order Hub ở trong khách hàng !!!");

                        // search product and edit
                        $('.searchProductName').on('change', function () {
                            setTimeout(() => {
                                const value = $(this).val();
                                const orderID = $(this).attr('data-orderid');
                                if (!value) {
                                    $(`#listProducts-${orderID}`).empty();
                                    return;
                                }
                                $(`#listProducts-${orderID}`).html(loadHTML);
                                const data = {
                                    searchValue: value.trim()
                                };
                                // call api
                                $.ajax({
                                    contentType: 'application/json',
                                    url: '/HistoryOrderProduct/Products',
                                    method: "POST",
                                    data: JSON.stringify(data),
                                    success: function (res) {
                                        let htmls = ``;
                                        for (let product of res) {
                                            htmls += `
                                                <li id="${product.ProductID}" data-productName="${product.ProductName}" 
                                                    class="productItem" data-productPrice="${product.Price}"
                                                    style="text-align: left; padding: 5px 2px;">
                                                    <img src="/Images/Products/${product.Photo}" style="height: 100px; width: 100px;" 
                                                        alt="${product.ProductName}" >
                                                    ${product.ProductName}
                                                    ( ${product.Price} đ )
                                                    </li>`;
                                        }
                                        setTimeout(() => {
                                            $(`#listProducts-${orderID}`).empty();
                                            $(`#listProducts-${orderID}`).html(htmls);
                                            $('.productItem').click(function () {
                                                $('.productItem').removeClass('bg-success');
                                                $(this).addClass('bg-success');
                                                $('.editProductID').val($(this).attr('id'));
                                                $('.editProductName').val($(this).attr('data-productName'));
                                                $('.editProductPrice').val($(this).attr('data-productPrice'));
                                            });

                                            // edit order
                                            $('.btnEditOrder').click(function () {
                                                const url = $(this).attr('data-url');
                                                const orderID = $(this).attr('data-id');
                                                const productID = $('.editProductID').val();
                                                const productName = $('.editProductName').val();
                                                let productPrice = +($('.editProductPrice').val());
                                                console.log('edit: url, orderID, productID: ', url, orderID, productID);
                                                const employeeUserName = $(this).attr('data-employeeUserName');
                                                const textlink = $(this).attr('data-textlink');
                                                const orderedTime = formatDate(new Date($(this).attr('data-orderedTime')));
                                                const payType = +($(this).attr('data-payType')) === 0 ?
                                                    'Tiền mặt' : 'Ví điện tử';

                                                const fromUserName = $(this).attr('data-fromUserName');
                                                const toUserNames = ['admin', 'admin2'];

                                                const data = {
                                                    OrderID: orderID,
                                                    ProductID: productID
                                                };

                                                const notificationData = {
                                                    isEdit: true,
                                                    isCancel: false,
                                                    employeeUserName: employeeUserName,
                                                    newOrderID: orderID,
                                                    newProductName: productName,
                                                    orderedTime: orderedTime,
                                                    payType: payType,
                                                    textlink: textlink ?? "Sảnh A",
                                                    newProductPrice: productPrice.toFixed(2)
                                                        .replace(/\d(?=(\d{3})+\.)/g, '$&.')
                                                        .replaceAll('.', ',')
                                                        .replace(',', '.'),
                                                    productPrice: productPrice.toFixed(2)
                                                        .replace(/\d(?=(\d{3})+\.)/g, '$&.')
                                                        .replaceAll('.', ',')
                                                        .replace(',', '.'),
                                                };


                                                // call api
                                                $.ajax({
                                                    contentType: 'application/json',
                                                    url: url,
                                                    method: "POST",
                                                    data: JSON.stringify(data),
                                                    success: function (res) {
                                                        console.log("res: ", res);

                                                        if (res) {
                                                            $('#errMsgEditOrder').text(res);
                                                            $(this).attr('data-dismiss', '');
                                                            return;
                                                        }
                                                        $('#errMsgEditOrder').empty();
                                                        $(this).attr('data-dismiss', 'modal');


                                                        // emit event
                                                        orderHub.server.notifyOrder(fromUserName, JSON.stringify(notificationData), toUserNames);

                                                        // filter();
                                                        location.reload();

                                                    },
                                                    error: function (err) {
                                                        console.log("===> err: ", err);
                                                    }
                                                });



                                            });
                                        }, 500)

                                    },
                                    error: function (err) {
                                        console.log("===> err: ", err);
                                    }
                                });
                            }, 500);
                        });

                        // cancel order
                        $('.btnCancelOrder').click(function () {
                            const url = $(this).attr('data-url');
                            const orderID = $(this).attr('data-id');
                            console.log('url, orderID: ', url, orderID);
                            const fromUserName = $(this).attr('data-fromUserName');
                            const toUserNames = ['admin', 'admin2'];

                            const data = {
                                OrderID: orderID
                            };

                            const notificationData = {
                                isEdit: false,
                                isCancel: true,
                                newOrderID: orderID,
                            };


                            // call api
                            $.ajax({
                                contentType: 'application/json',
                                url: url,
                                method: "POST",
                                data: JSON.stringify(data),
                                success: function () {

                                    // emit event
                                    orderHub.server.notifyOrder(fromUserName, JSON.stringify(notificationData), toUserNames);

                                    // filter();
                                    location.reload();
                                },
                                error: function (err) {
                                    console.log("===> err: ", err);
                                }
                            });

                        });
                    })
                    .fail(function () {
                        console.log("Không thể kết nối Order Hub ở trong khách hàng !");
                    });

            }, 500);
             console.log("===> res: ", res);

        },
        error: function (err) {
            console.log("===> err: ", err);
        }
    });
}
filter();

// filter
$('#formFilter').submit(function (e) {
    e.preventDefault();
    filter();
});

$('#searchValue').on('input', function (e) {
    const value = e.target.value;
    if (!value) filter();
});

$('#orderStatus').change(function () {
    filter();
});

$('#btnFilter').click(function () {
    filter();
});

$('#currentPage').change(function () {
    const currentPage = $(this).val();
    if (!currentPage) return;
    filter();
});

$('#btnPrev').click(function () {
    const currentPage = +($('#currentPage').val());
    if (currentPage == 1) return;
    $('#currentPage').val(currentPage - 1);
    filter();
});

$('#btnNext').click(function () {
    const currentPage = +($('#currentPage').val());
    $('#currentPage').val(currentPage + 1);
    filter();
});

// clean modal-backdrop
$('.modal-backdrop').remove();