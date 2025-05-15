$(document).ready(function () {
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
            console.log('===> [manage order] order info in admin: ', data);
            let res = JSON.parse(data.NotificationData);
            console.log('===> [manage order] res: ', res);
            let orderJson = res.orderItem;
            // console.log('===> [manage order] orderJson: ', orderJson);

            const currentPage = +($('#currentPage').val());
            if (currentPage == 1) {
                if (res.isCancel && !res.isEdit) {
                    $(`#status-${res.newOrderID}`).removeClass('text-primary');
                    $(`#status-${res.newOrderID}`).addClass('text-muted');
                    $(`#status-${res.newOrderID} b`).text('Đơn hàng bị hủy');
                    $(`#btnAction-${res.newOrderID}`).empty();
                    $(`#btnDeleteOrder-${res.newOrderID}`).removeClass('disabled');
                    $(`#dropdown-${res.newOrderID} .dropdown-toggle`).addClass('disabled');
                    return;
                }

                let productPriceNum = res.productPrice.replace(',00', '').replace('.', '') | 0;

                if (!res.isCancel && res.isEdit) {
                    $(`#productName-${res.newOrderID}`).text(res.newProductName);
                    $(`#productPrice-${res.newOrderID}`).text(`${res.newProductPrice} ₫`);
                    productPriceNum = res.newProductPrice.replace(',00', '').replace('.', '') | 0;
                    $(`#btnProcessOrder-${res.newOrderID}`).attr('data-productPrice', productPriceNum);
                    return;
                }

                

                const newOrderHTML = `
                            <tr class="backgroundAnimated">
                                <td>${res.fullName}</td>
                                <td id="productName-${res.newOrderID}">${res.productName}</td>
                                <td>${res.orderedTime}</td>
                                <td id="status-${res.newOrderID}" class="text-primary"><b>Đang xử lý</b></td>
                                <td>${res.orderedPayType}</td>
                                <td id="productPrice-${res.newOrderID}">${res.productPrice} ₫</td>

                                <td>
                                    <div id="dropdown-${res.newOrderID}" class="dropdown">
                                        <button style="width:100%" class="btn btn-primary dropdown-toggle" type="button" 
                                        id="btnProcessOrder" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                            <span>Xử lý đơn</span>
                                            <span class="caret"></span>
                                        </button>
                        
                                        <ul id="btnAction-${res.newOrderID}" class="dropdown-menu" aria-labelledby="btnProcessOrder">
                                            <li class="btnActionOrder" 
                                                data-nextStatus="1"
                                                data-url="/Admin/ManageOrder/Approve">
                                                <a style="color: green !important;" href="#" data-toggle="modal"
                                                    data-target="#process-${res.newOrderID}">
                                                    Phê duyệt
                                                </a>
                                            </li>
                                            <li role="separator" class="divider"></li>
                                            <li class="btnActionOrder" 
                                                data-nextStatus="-1"
                                                data-url="/Admin/ManageOrder/Reject">
                                                <a style="color: red !important;" href="#" data-toggle="modal"
                                                    data-target="#process-${res.newOrderID}">
                                                    Từ chối
                                                </a>
                                            </li>
                                        </ul>
                        
                                    </div>
                                </td>
                                <td class="text-center">
                                    <p data-placement="top" data-toggle="tooltip" title="Xóa vĩnh viễn">
                                        <button id="btnDeleteOrder-${res.newOrderID}" 
                                                class="btn btn-danger btn-xs disabled" 
                                                data-toggle="modal" 
                                                data-target="#delete-${res.newOrderID}">
                                            <span class="glyphicon glyphicon-trash"></span>
                                        </button>
                                    </p>
                                </td>
                            </tr>
                        `;

                const processModalHTML = `
                            <div class="modal fade" id="process-${res.newOrderID}" tabindex="-1" role="dialog" aria-labelledby="edit" 
                                    aria-hidden="true">
                                <div class="modal-dialog">
                                    <div class="modal-content" style="border-radius: 10px;">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                                                <span class="glyphicon glyphicon-remove" aria-hidden="true"></span>
                                            </button>
                                            <h4 class="modal-title custom_align" id="Heading">Xử lý đơn đặt của khách hàng</h4>
                                        </div>

                                        <div class="modal-body alert alert-info" style="margin-bottom: 0px;">
                                            <strong>
                                                <i class="glyphicon glyphicon-info-sign"></i></strong> 
                                                Bạn có chắc chắn <span class="actionNameOrder"></span> với đơn này không?
                                        </div>
                                        <div class="modal-footer">
                                            <button id="btnProcessOrder-${res.newOrderID}" type="button" 
                                                    class="btnProcessOrder btn btn-success" 
                                                    data-id="${res.newOrderID}" 
                                                    data-employeeUserName="${orderJson.Employee.UserName}"
                                                    data-productName="${orderJson.Product.ProductName}"
                                                    data-orderedTime="${parseInt(orderJson.OrderedTime.substr(6, 13))}"
                                                    data-payType="${orderJson.Type}"
                                                    data-textlink="${orderJson.Textlink}"
                                                    data-productPrice="${productPriceNum}"
                                                    data-action="" 
                                                    data-url="" 
                                                    data-nextStatus=""
                                                    data-dismiss="modal">
                                                <span class="glyphicon glyphicon-ok-sign"></span> Đồng ý
                                            </button>
                                            <button type="button" class="btn btn-default" data-dismiss="modal">
                                                <span class="glyphicon glyphicon-remove"></span>Từ chối
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        `;

                const deleteModalModalHTML = `
                            <div class="modal fade" id="delete-${res.newOrderID}" tabindex="-1" role="dialog" aria-labelledby="edit" 
                                    aria-hidden="true">
                                <div class="modal-dialog">
                                    <div class="modal-content" style="border-radius: 10px;">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                                                <span class="glyphicon glyphicon-remove" aria-hidden="true"></span>
                                            </button>
                                            <h4 class="modal-title custom_align" id="Heading">Xóa đơn đặt của khách hàng</h4>
                                        </div>
                                        <div class="alert alert-danger" style="margin-bottom: 0px;">

                                            <span class="glyphicon glyphicon-exclamation-sign"></span>
                                            Bạn có chắc chắn xóa vĩnh viễn đơn này không?

                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" class="btnDeleteOrder btn btn-success" 
                                                    data-id="${res.newOrderID}" 
                                                    data-url="/Admin/ManageOrder/Delete" data-dismiss="modal">
                                                <span class="glyphicon glyphicon-ok-sign"></span>
                                                Đồng ý
                                            </button>
                                            <button type="button" class="btn btn-default" data-dismiss="modal">
                                                <span class="glyphicon glyphicon-remove"></span>
                                                Từ chối
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        `;

                $('#tbBodyOrder').prepend(newOrderHTML);
                $('#filterResult').prepend(processModalHTML);
                $('#filterResult').prepend(deleteModalModalHTML);

                // emit event order from admin to employee
                $.connection.hub.start()
                    .done(function () {
                        console.log("Đã kết nối Order Hub ở trong quản trị viên !!!");

                        // process order
                        $('.btnActionOrder').click(function () {
                            const url = $(this).attr('data-url');
                            const nextStatus = $(this).attr('data-nextStatus');
                            const action = $(this).children().text().trim().toLowerCase();
                            console.log('=====> btnActionOrder: ', nextStatus);
                            $('.actionNameOrder').empty().text(action);
                            $('.btnProcessOrder').attr('data-action', action);
                            $('.btnProcessOrder').attr('data-url', url);
                            $('.btnProcessOrder').attr('data-nextStatus', nextStatus);
                        });


                        //sussssecx
                        $('.btnSucsessOrder').click(function () {


                            var inputValue = $("#linkInput").val();

                            // Regex để kiểm tra dữ liệu nhập vào có phải là một link hay không
                            var regex = /\b(?:https?|ftp|file):\/\/[-A-Za-z0-9+&@#\/%?=~_|!:,.;]*[-A-Za-z0-9+&@#\/%=~_|]/;

                            // Kiểm tra dữ liệu nhập vào với regex
                            if (regex.test(inputValue)) {
                                // Nếu đúng, bạn có thể làm điều gì đó với link ở đây
                                console.log("Link hợp lệ:", inputValue);
                            } else {
                                // Nếu sai, hiển thị một cảnh báo
                                alert("Link không hợp lệ!");
                            }




                            const action = $(this).attr('data-action');
                            const url = $(this).attr('data-url');
                            let orderID = $(this).attr('data-id');
                            const nextStatus = +($(this).attr('data-nextStatus'));
                            console.log(`${action} với đơn có mã orderID = ${orderID} tại url : ${url} 
                                            và nextStatus: ${nextStatus}`);

                            const fromUserName = $('#fromUserName').val();

                            const employeeUserName = $(this).attr('data-employeeUserName');
                            const productName = $(this).attr('data-productName');
                            let productPrice = +($(this).attr('data-productPrice'));
                            const textlink = $(this).attr('data-textlink');
                            const orderedTime = formatDate(new Date($(this).attr('data-orderedTime')));
                            const payType = +($(this).attr('data-payType')) === 0 ? 'Tiền mặt' : 'Ví điện tử';
                            const toUserNames = [employeeUserName];
                            let formatStatus = "";
                            let status = "";


                            switch (nextStatus) {
                                case 0:
                                    formatStatus = "primary";
                                    break;
                                case 1:
                                case 2:
                                    formatStatus = "yellow";
                                    break;
                                case 99:
                                    formatStatus = "green";
                                    break;
                                case -2:
                                case -1:
                                case -99:
                                    formatStatus = "red";
                                    break;
                            }

                            switch (nextStatus) {
                                case -2:
                                    status = "Đơn hàng bị hủy";
                                    break;
                                case -1:
                                    status = "Đơn hàng bị từ chối";
                                    break;
                                case 0:
                                    status = "Đang xử lý";
                                    break;
                                case 1:
                                    status = "Đã duyệt đơn. Chờ hoàn thành";
                                    break;
                                case 2:
                                    status = "Lấy quảng cáo thành công. Chờ thanh toán";
                                    break;
                                case 99:
                                    status = "Đã thanh toán. Đơn hàng đã đặt thành công";
                                    break;
                                case -99:
                                    status = "Không được thanh toán. Đơn hàng đã đặt thất bại";
                                    break;
                            }


                            let notificationData = {
                                productName: productName,
                                productPrice: productPrice,
                                orderedTime: orderedTime,
                                payType: payType,
                                status: status,
                                nextStatus: nextStatus,
                                formatStatus: formatStatus,
                                textlink: inputValue === "null" ? "" : inputValue,
                                orderID: orderID,
                            };

                            console.log('===> fromUserName: ', fromUserName);
                            console.log('===> notificationData admin: ', notificationData);


                            orderHub.server.notifyOrder(fromUserName, JSON.stringify(notificationData), toUserNames);

                            // call api
                            const data = {
                                OrderID: orderID
                            };
                            $.ajax({
                                contentType: 'application/json',
                                url: url,
                                method: "POST",
                                data: JSON.stringify(data),
                                success: function (res) {
                                    console.log('===> res: 3333 ', res);
                                    filter($('#currentPage').val());
                                },
                                error: function (err) {
                                    console.log("===> err: ", err);
                                }
                            });
                        });





                        $('.btnProcessOrder').click(function () {
                            const action = $(this).attr('data-action');
                            const url = $(this).attr('data-url');
                            let orderID = $(this).attr('data-id');
                            const nextStatus = +($(this).attr('data-nextStatus'));
                            console.log(`${action} với đơn có mã orderID = ${orderID} tại url : ${url} 
                                            và nextStatus: ${nextStatus}`);

                            const fromUserName = $('#fromUserName').val();

                            const employeeUserName = $(this).attr('data-employeeUserName');
                            const productName = $(this).attr('data-productName');
                            let productPrice = +($(this).attr('data-productPrice'));
                            const textlink = $(this).attr('data-textlink');
                            const orderedTime = formatDate(new Date($(this).attr('data-orderedTime')));
                            const payType = +($(this).attr('data-payType')) === 0 ? 'Tiền mặt' : 'Ví điện tử';
                            const toUserNames = [employeeUserName];
                            let formatStatus = "";
                            let status = "";
                            

                            switch (nextStatus) {
                                case 0:
                                    formatStatus = "primary";
                                    break;
                                case 1:
                                case 2:
                                    formatStatus = "yellow";
                                    break;
                                case 99:
                                    formatStatus = "green";
                                    break;
                                case -2:
                                case -1:
                                case -99:
                                    formatStatus = "red";
                                    break;
                            }

                            switch (nextStatus) {
                                case -2:
                                    status = "Đơn hàng bị hủy";
                                    break;
                                case -1:
                                    status = "Đơn hàng bị từ chối";
                                    break;
                                case 0:
                                    status = "Đang xử lý";
                                    break;
                                case 1:
                                    status = "Đã duyệt đơn. Chờ hoàn thành";
                                    break;
                                case 2:
                                    status = "Sản phẩm đang trong quá trình hoàn thânh";
                                    break;
                                case 99:
                                    status = "Đã hoàn thành dự án";
                                    break;
                                case -99:
                                    status = "Không được thanh toán. Đơn hàng đã đặt thất bại";
                                    break;
                            }
                           

                            let notificationData = {
                                productName: productName,
                                productPrice: productPrice,
                                orderedTime: orderedTime,
                                payType: payType,
                                status: status,
                                nextStatus: nextStatus,
                                formatStatus: formatStatus,
                                textlink: textlink === "AAAAAAA" ? "AAAAAA" : textlink,
                                orderID: orderID,
                            };

                            console.log('===> fromUserName: ', fromUserName);
                            console.log('===> notificationData admin lor: ', notificationData);


                            orderHub.server.notifyOrder(fromUserName, JSON.stringify(notificationData), toUserNames);

                            // call api
                            const data = {
                                OrderID: orderID
                            };
                            $.ajax({
                                contentType: 'application/json',
                                url: url,
                                method: "POST",
                                data: JSON.stringify(data),
                                success: function (res) {
                                    console.log('===> res: 4444 ', res);
                                    filter($('#currentPage').val());
                                },
                                error: function (err) {
                                    console.log("===> err: ", err);
                                }
                            });
                        });

                        // Delete order
                        $('.btnDeleteOrder').click(function () {
                            const orderID = $(this).attr('data-id');
                            const url = $(this).attr('data-url');
                            console.log(`Xóa mặt hàng có mã orderID = ${orderID} với url: ${url}`);

                            const data = {
                                OrderID: orderID
                            };
                            // call api
                            $.ajax({
                                contentType: 'application/json',
                                url: url,
                                method: "POST",
                                data: JSON.stringify(data),
                                success: function (res) {
                                    console.log('===> res: ', res);
                                    filter($('#currentPage').val());
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
            }

            let badgeNotifyOrder = +($('#badgeNotifyOrder').text()) + 1;
            $('#badgeNotifyOrder').text(badgeNotifyOrder);

            $('#activeNotify').show();
            $('#badgeNotifyOrder').show();
        }
    }

    const loadHTML = `<p class="text-center text-success" style="font-weight: bold;">
                                    <span class="glyphicon glyphicon-refresh spinning"></span> Đang tải dữ liệu...
                                  </p>`;

    function filter(page = 1, isLoad = true) {
        $('#filterResult').empty();
        const url = $('#formFilter').attr("action");
        console.log("url: ", url);

        if (isLoad) {
            $('#filterResult').html(loadHTML);
        }

        const data = {
            Status: $('#orderStatus').val(),
            FromDatetime: $('#fromDatetime').val(),
            ToDatetime: $('#toDatetime').val(),
            SearchField: $('#searchField-0').is(':checked')
                ? $('#searchField-0').val()
                : ($('#searchField-1').is(':checked') ? $('#searchField-1').val() : $('#searchField-0').val()),
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
                // console.log('===> Manage Order res: ', res);
                if (!res.includes('id="mytable"')) {
                    $('#clientErrMsg').text(res);
                    $('#clientErrMsg').show();
                    $('#filterResult').empty();
                    return;
                }

                $('#clientErrMsg').text('');
                $('#clientErrMsg').hide();

                if (!isLoad) {
                    $('#filterResult').empty();
                    $('#filterResult').html(res);
                    return;
                }
                setTimeout(function () {
                    $('#filterResult').html(res);

                    // emit event order from admin to employee
                    $.connection.hub.start()
                        .done(function () {
                            console.log("Đã kết nối Order Hub ở trong quản trị viên !!!");

                            // process order
                            $('.btnActionOrder').click(function () {
                                const url = $(this).attr('data-url');
                                const nextStatus = $(this).attr('data-nextStatus');
                                const action = $(this).children().text().trim().toLowerCase();
                                $('.actionNameOrder').empty().text(action);
                                $('.btnProcessOrder').attr('data-action', action);
                                $('.btnProcessOrder').attr('data-url', url);
                                $('.btnProcessOrder').attr('data-nextStatus', nextStatus);
                            });

                            $('.btnProcessOrder').click(function () {
                                const action = $(this).attr('data-action');
                                const url = $(this).attr('data-url');
                                let orderID = $(this).attr('data-id');
                                const nextStatus = +($(this).attr('data-nextStatus'));
                                console.log(`${action} với đơn có mã orderID = ${orderID} tại url : ${url} 
                                                và nextStatus: ${nextStatus}`);

                                const fromUserName = $('#fromUserName').val();

                                const employeeUserName = $(this).attr('data-employeeUserName');
                                const productName = $(this).attr('data-productName');
                                const textlink = $(this).attr('data-textlink');
                                const orderedTime = formatDate(new Date($(this).attr('data-orderedTime')));
                                const payType = +($(this).attr('data-payType')) === 0 ? 'Tiền mặt' : 'Ví điện tử';
                                const toUserNames = [employeeUserName];
                                let formatStatus = "";
                                let status = "";

                                switch (nextStatus) {
                                    case 0:
                                        formatStatus = "primary";
                                        break;
                                    case 1:
                                    case 2:
                                        formatStatus = "yellow";
                                        break;
                                    case 99:
                                        formatStatus = "green";
                                        break;
                                    case -2:
                                    case -1:
                                    case -99:
                                        formatStatus = "red";
                                        break;
                                }

                                switch (nextStatus) {
                                    case -2:
                                        status = "Đơn hàng bị hủy";
                                        break;
                                    case -1:
                                        status = "Đơn hàng bị từ chối";
                                        break;
                                    case 0:
                                        status = "Đang xử lý";
                                        break;
                                    case 1:
                                        status = "Đã duyệt đơn. Chờ hoàn thành ";
                                        break;
                                    case 2:
                                        status = "Lấy quảng cáo thành công. Chờ thanh toán";
                                        break;
                                    case 99:
                                        status = "Đã thanh toán. Đơn hàng đã đặt thành công";
                                        break;
                                    case -99:
                                        status = "Không được thanh toán. Đơn hàng đã đặt thất bại";
                                        break;
                                }

                                const notificationData = {
                                    productName: productName,
                                    orderedTime: orderedTime,
                                    payType: payType,
                                    status: status,
                                    nextStatus: nextStatus,
                                    formatStatus: formatStatus,
                                    textlink: textlink === "null" ? "Sảnh A" : textlink,
                                    orderID: orderID,
                                };

                                console.log('===> fromUserName: ', fromUserName);
                                console.log('===> notificationData: Log của admin 1 ', notificationData);


                                orderHub.server.notifyOrder(fromUserName, JSON.stringify(notificationData), toUserNames);

                                // call api
                                const data = {
                                    OrderID: orderID
                                };
                                $.ajax({
                                    contentType: 'application/json',
                                    url: url,
                                    method: "POST",
                                    data: JSON.stringify(data),
                                    success: function (res) {
                                        console.log('===> res: 11111 ', res);
                                        filter($('#currentPage').val());
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

                    // check action paginate
                    const currentPage = +($('#currentPage').val());
                    const disableStyle = "disabled";
                    if (currentPage > 1) {
                        $('#btnPrev').removeClass(disableStyle);
                    } else {
                        $('#btnPrev').addClass(disableStyle);
                    }

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

                    // delete order
                    $('#btnDeleteAllOrders').click(function () {
                        const url = $(this).attr('data-url');
                        console.log(`Xóa tất cả đơn hàng ở trạng thái bị từ chối hoặc bị hủy với url: ${url}`);

                        // call api
                        $.ajax({
                            contentType: 'application/json',
                            url: url,
                            method: "POST",
                            success: function (res) {
                                console.log('===> res: ', res);
                                filter($('#currentPage').val());
                            },
                            error: function (err) {
                                console.log("===> err: ", err);
                            }
                        });
                    });
                    $('.btnDeleteOrder').click(function () {
                        const orderID = $(this).attr('data-id');
                        const url = $(this).attr('data-url');
                        console.log(`Xóa mặt hàng có mã orderID = ${orderID} với url: ${url}`);

                        const data = {
                            OrderID: orderID
                        };
                        // call api
                        $.ajax({
                            contentType: 'application/json',
                            url: url,
                            method: "POST",
                            data: JSON.stringify(data),
                            success: function (res) {
                                console.log('===> res: 2222 ', res);
                                filter($('#currentPage').val());
                            },
                            error: function (err) {
                                console.log("===> err: ", err);
                            }
                        });
                    });

                }, 500);
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

    // process choose
    $('#orderStatus').change(function () {
        filter($('#currentPage').val());
    });
    $('#fromDatetime').change(function () {
        const fromDatetime = new Date($('#fromDatetime').val());
        const toDatetime = new Date($('#toDatetime').val());

        if (fromDatetime > toDatetime) {
            $('#clientErrMsg').text('Ngày bắt đầu không được lớn hơn ngày kết thúc !!!');
            $('#clientErrMsg').show();
            $(this).val('');
            setTimeout(() => {
                $('#clientErrMsg').empty();
                $('#clientErrMsg').hide();
            }, 3000);
            return;
        }
        filter($('#currentPage').val());
    });
    $('#toDatetime').change(function () {
        const fromDatetime = new Date($('#fromDatetime').val());
        const toDatetime = new Date($('#toDatetime').val());

        if (fromDatetime > toDatetime) {
            $('#clientErrMsg').text('Ngày bắt đầu không được lớn hơn ngày kết thúc !!!');
            $('#clientErrMsg').show();
            $(this).val('');
            setTimeout(() => {
                $('#clientErrMsg').empty();
                $('#clientErrMsg').hide();
            }, 3000);
            return;
        }
        filter($('#currentPage').val());
    });

    $('#btnFilter').click(function () {
        filter($('#currentPage').val());
    });
});