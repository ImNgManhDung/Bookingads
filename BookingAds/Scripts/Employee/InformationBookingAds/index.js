// process notification order
const orderHub = $.connection.orderHub;
if (orderHub) {
    orderHub.client.NotifyOrderMsg = function (data) {
        // fromUserName, notificationData, toUserNames
        // console.log('[Information] ===> order info in employee: ', data);
        let badgeNotifyOrder = +($('#badgeNotifyOrder').text()) + 1;
        $('#badgeNotifyOrder').text(badgeNotifyOrder);

        $('#activeNotify').show();
        $('#badgeNotifyOrder').show();
    }
}

// format date: dd/MM/yyyy HH:mm:ss tt
function formatDate(now) {
    const day = now.getDate() < 10 ? ('0' + now.getDate()) : now.getDate();
    const month = (now.getMonth() + 1) < 10 ? ('0' + (now.getMonth() + 1)) : (now.getMonth() + 1);
    const year = now.getFullYear();
    let hours = now.getHours();
    const minutes = now.getMinutes() < 10 ? ('0' + now.getMinutes()) : now.getMinutes();
    const seconds = now.getSeconds() < 10 ? ('0' + now.getSeconds()) : now.getSeconds();
    const times = hours < 12 ? 'AM' : 'PM';
    hours = hours < 10 ? '0' + hours : hours;

    return `${day}/${month}/${year} ${hours}:${minutes}:${seconds} ${times}`;
}

function filter(page = 1) {
    const url = $('#formFilter').attr("action");
    console.log("url: ", url);

    //// add load
    //$('#filterResult').html(loadHTML);

    // check status button previous pagination
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
        SearchValue: $("#searchValue").val(),
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

                $('.radioPayType').click(function () {
                    $(this).children('.payType').prop('checked', true);
                });

                $.connection.hub.start()
                    .done(function () {
                        console.log("Đã kết nối Order Hub ở trong khách hàng !!!");

                        $('.btnSubmitEvent').click(function () {
                            const fromUserName = $('#fromUserName').val();
                            const dataEmployeeID = $(this).attr('data-employeeID');
                            const dataEmployeeFullName = $(this).attr('data-employeeFullName');
                            const dataProduct = JSON.parse($(this).attr('data-product'));
                            const payType = $('.payType:checked').val();

                            if (!payType) {
                                alert('Vui lòng chọn phương thức thanh toán');
                                $(this).attr('data-dismiss', '');
                                return;
                            }

                            $(this).attr('data-dismiss', 'modal');

                            let orderItem;
                            const data = {
                                employeeID: dataEmployeeID,
                                productID: dataProduct.ProductID,
                                payType: payType
                            };

                            // call ajax
                            $.ajax({
                                contentType: 'application/json',
                                url: '/InformationBookingAds/OrderProduct',
                                method: 'POST',
                                data: JSON.stringify(data),
                                success: function (res) {
                                    console.log('===> res in create order: ', res);
                                    orderItem = res;

                                    if (+res === 0) {
                                        location.reload();
                                        return;
                                    }

                                    const notificationData = {
                                        orderItem: orderItem,
                                        newOrderID: orderItem.OrderID,
                                        fullName: dataEmployeeFullName,
                                        productName: dataProduct.ProductName,
                                        orderedTime: formatDate(new Date()),
                                        orderedPayType: payType === 'WALLET' ? 'Ví điện tử' : 'Tiền mặt',
                                        productPrice: (dataProduct.Price).toFixed(2)
                                            .replace(/\d(?=(\d{3})+\.)/g, '$&.')
                                            .replaceAll('.', ',')
                                            .replace(',', '.'),
                                    };

                                    const toUserNames = ['admin', 'admin2'];
                                    console.log('===> fromUserName: ', fromUserName);
                                    console.log('===> notificationData: ', notificationData);

                                    // emit event
                                    orderHub.server.notifyOrder(fromUserName, JSON.stringify(notificationData), toUserNames);

                                    window.location.href = '/HistoryOrderProduct/Index';
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

                // restore and soft delete product
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

// process filter
$('#formFilter').submit(function (e) {
    e.preventDefault();
    filter($('#currentPage').val());
});

$('#searchValue').on('input', function (e) {
    const value = e.target.value;
    if (!value) filter($('#currentPage').val());
});

// action filter
$('#sort').change(function () {
    filter($('#currentPage').val());
});
$('#CatelogProductsID').change(function () {
    filter($('#currentPage').val());
});

$('#btnFilter').click(function () {
    filter($('#currentPage').val());
});

$(document).ready(function () {
    setTimeout(() => {
        $('#error-msg').remove();
        $('#success-msg').remove();
    }, 5000);
});

document.addEventListener("DOMContentLoaded", function () {
    var editLink = document.getElementById("editlink");
    if (editLink) {
        editLink.setAttribute("data-title", "Edit");
        editLink.setAttribute("data-toggle", "modal");
        editLink.setAttribute("data-target", "#edit");
    }
});

document.querySelectorAll('input[type="checkbox"]').forEach(function (checkbox) {
    checkbox.addEventListener('change', function () {
        // Nếu checkbox đang xem xét là option1
        if (this.id === 'option1') {
            // Bỏ chọn checkbox option2 nếu option1 được chọn
            document.getElementById('option2').checked = false;
        } else if (this.id === 'option2') {
            // Bỏ chọn checkbox option1 nếu option2 được chọn
            document.getElementById('option1').checked = false;
        }
    });
});