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
  orderHub.client.NotifyOrderMsg = function(data) {
    // fromUserName, notificationData, toUserNames
    const res = JSON.parse(data.NotificationData);
    // console.log('[HistoryProductOrder] ===> order info in employee: ', data);
    console.log('[HistoryProductOrder] ===> res in employee: ', res);

    const currentPage = +($('#currentPage').val());
    if (currentPage == 1) {
        $(`#btnAction-${res.orderID}`).empty();

        $(`#btnAction-${res.orderID}`).html(`
            <a href="/HistoryOrderProduct/DetailsMess?orderID=${res.orderID}" class="btn btn-info" style="font-size: 9px;" role="button">
                <svg width="20px" height="20px" viewBox="0 0 512 512" xmlns="http://www.w3.org/2000/svg" fill="#000000">
                    <g id="SVGRepo_bgCarrier" stroke-width="0"></g>
                    <g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g>
                    <g id="SVGRepo_iconCarrier">
                        <defs>
                            <style>
                                .cls-1 {
                                    fill: none;
                                    stroke: #ffffff;
                                    stroke-linecap: round;
                                    stroke-linejoin: round;
                                    stroke-width: 20px;
                                }
                            </style>
                        </defs>
                        <g data-name="Layer 2" id="Layer_2"> 
                            <g data-name="E425, History, log, manuscript" id="E425_History_log_manuscript">
                                <path class="cls-1" d="M75.11,117h0A21.34,21.34,0,0,1,53.83,95.57V31.39A21.34,21.34,0,0,1,75.11,10h0A21.34,21.34,0,0,1,96.39,31.39V95.57A21.34,21.34,0,0,1,75.11,117Z"></path>
                                <rect class="cls-1" height="64.17" width="319.22" x="96.39" y="31.39"></rect>
                                <rect class="cls-1" height="320.87" width="319.22" x="96.39" y="95.57"></rect>
                                <path class="cls-1" d="M34.34,39.08H53.83a0,0,0,0,1,0,0v48.8a0,0,0,0,1,0,0H34.34A24.34,24.34,0,0,1,10,63.54v-.13A24.34,24.34,0,0,1,34.34,39.08Z"></path>
                                <path class="cls-1" d="M436.89,117h0a21.34,21.34,0,0,0,21.28-21.39V31.39A21.34,21.34,0,0,0,436.89,10h0a21.34,21.34,0,0,0-21.28,21.39V95.57A21.34,21.34,0,0,0,436.89,117Z"></path>
                                <path class="cls-1" d="M482.51,39.08H502a0,0,0,0,1,0,0v48.8a0,0,0,0,1,0,0H482.51a24.34,24.34,0,0,1-24.34-24.34v-.13a24.34,24.34,0,0,1,24.34-24.34Z" transform="translate(960.17 126.96) rotate(-180)"></path>
                                <path class="cls-1" d="M75.11,395h0a21.34,21.34,0,0,0-21.28,21.39v64.18A21.34,21.34,0,0,0,75.11,502h0a21.34,21.34,0,0,0,21.28-21.39V416.43A21.34,21.34,0,0,0,75.11,395Z"></path>
                                <rect class="cls-1" height="64.17" width="319.22" x="96.39" y="416.43"></rect>
                                <path class="cls-1" d="M34.34,424.12H53.83a0,0,0,0,1,0,0v48.8a0,0,0,0,1,0,0H34.34A24.34,24.34,0,0,1,10,448.58v-.13A24.34,24.34,0,0,1,34.34,424.12Z"></path>
                                <path class="cls-1" d="M436.89,395h0a21.34,21.34,0,0,1,21.28,21.39v64.18A21.34,21.34,0,0,1,436.89,502h0a21.34,21.34,0,0,1-21.28-21.39V416.43A21.34,21.34,0,0,1,436.89,395Z"></path>
                                <path class="cls-1" d="M482.51,424.12H502a0,0,0,0,1,0,0v48.8a0,0,0,0,1,0,0H482.51a24.34,24.34,0,0,1-24.34-24.34v-.13a24.34,24.34,0,0,1,24.34-24.34Z" transform="translate(960.17 897.04) rotate(-180)"></path>
                                <line class="cls-1" x1="143.41" x2="256" y1="140.11" y2="140.11"></line>
                                <line class="cls-1" x1="143.41" x2="371.26" y1="186.47" y2="186.47"></line>
                                <line class="cls-1" x1="143.41" x2="371.26" y1="232.82" y2="232.82"></line>
                                <line class="cls-1" x1="143.41" x2="371.26" y1="279.18" y2="279.18"></line>
                                <line class="cls-1" x1="143.41" x2="371.26" y1="325.53" y2="325.53"></line>
                                <line class="cls-1" x1="256" x2="371.26" y1="371.89" y2="371.89"></line>
                            </g>
                        </g>
                    </g>
                </svg>
                Details
            </a>
        `);


      $(`#status-${res.orderID} span`).attr('class', `badge bg-${res.formatStatus}`);
      console.log('>>>> trạng thái gửi về: ', res.status);
      $(`#status-${res.orderID} span`).text(res.status);
      if (res.nextStatus === 99) {
          $(`#textlink-${res.orderID}`).html(` <th id="textlink-${res.orderID}" style="text-align: center; vertical-align: middle;">
                        <a href="${res.textlink}" class="button-link" style=" display: inline-block; background-color: #4CAF50; color: white; padding: 6px 15px; text-align: center; text-decoration: none; font-size: 9px; border: none; border-radius: 5px; cursor: pointer;"><svg width="20px" height="20px" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <path d="M15.7281 3.88396C17.1624 2.44407 19.2604 2.41383 20.4219 3.57981C21.5856 4.74798 21.5542 6.85922 20.1189 8.30009L17.6951 10.7333C17.4028 11.0268 17.4037 11.5017 17.6971 11.794C17.9906 12.0863 18.4655 12.0854 18.7578 11.7919L21.1816 9.35869C23.0929 7.43998 23.3329 4.37665 21.4846 2.5212C19.6342 0.663551 16.5776 0.905664 14.6653 2.82536L9.81768 7.69182C7.90639 9.61053 7.66643 12.6739 9.5147 14.5293C9.80702 14.8228 10.2819 14.8237 10.5754 14.5314C10.8688 14.2391 10.8697 13.7642 10.5774 13.4707C9.41376 12.3026 9.4451 10.1913 10.8804 8.75042L15.7281 3.88396Z" fill="#1C274C"></path> <path opacity="0.5" d="M14.4846 9.4707C14.1923 9.17724 13.7174 9.17632 13.4239 9.46864C13.1305 9.76097 13.1296 10.2358 13.4219 10.5293C14.5856 11.6975 14.5542 13.8087 13.1189 15.2496L8.27129 20.1161C6.83696 21.556 4.73889 21.5862 3.57742 20.4202C2.41376 19.2521 2.4451 17.1408 3.8804 15.6999L6.30424 13.2666C6.59657 12.9732 6.59565 12.4983 6.30219 12.206C6.00873 11.9137 5.53386 11.9146 5.24153 12.208L2.81769 14.6413C0.906387 16.56 0.666428 19.6234 2.5147 21.4788C4.36518 23.3365 7.42173 23.0944 9.334 21.1747L14.1816 16.3082C16.0929 14.3895 16.3329 11.3262 14.4846 9.4707Z" fill="#1C274C"></path> </g></svg>Link</a>

                    </th>`);
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
    success: function(res) {
      if (!res.includes('class="table table-bordered"')) {
        alert(res);
        $('#filterResult').empty();
        return;
      }
      setTimeout(function() {

        $('#filterResult').html(res);

        // clean modal-backdrop
        $('.modal-backdrop').remove();

        $.connection.hub.start()
          .done(function() {
            console.log("Đã kết nối Order Hub ở trong khách hàng !!!");

            // search product and edit
            $('.searchProductName').on('change', function() {
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
                  success: function(res) {
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
                      $('.productItem').click(function() {
                        $('.productItem').removeClass('bg-success');
                        $(this).addClass('bg-success');
                        $('.editProductID').val($(this).attr('id'));
                        $('.editProductName').val($(this).attr('data-productName'));
                        $('.editProductPrice').val($(this).attr('data-productPrice'));
                      });

                      // edit order
                      $('.btnEditOrder').click(function() {
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
                          textlink: textlink ?? "A",
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
                          success: function(res) {
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
                          error: function(err) {
                            console.log("===> err: ", err);
                          }
                        });



                      });
                    }, 500)

                  },
                  error: function(err) {
                    console.log("===> err: ", err);
                  }
                });
              }, 500);
            });

            // cancel order
            $('.btnCancelOrder').click(function() {
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
                success: function() {

                  // emit event
                  orderHub.server.notifyOrder(fromUserName, JSON.stringify(notificationData), toUserNames);

                  // filter();
                  location.reload();
                },
                error: function(err) {
                  console.log("===> err: ", err);
                }
              });

            });
          })
          .fail(function() {
            console.log("Không thể kết nối Order Hub ở trong khách hàng !");
          });

      }, 500);
      // console.log("===> res: ", res);

    },
    error: function(err) {
      console.log("===> err: ", err);
    }
  });
}
filter();

// filter
$('#formFilter').submit(function(e) {
  e.preventDefault();
  filter();
});

$('#searchValue').on('input', function(e) {
  const value = e.target.value;
  if (!value) filter();
});

$('#orderStatus').change(function() {
  filter();
});

$('#btnFilter').click(function() {
  filter();
});

$('#currentPage').change(function() {
  const currentPage = $(this).val();
  if (!currentPage) return;
  filter();
});

$('#btnPrev').click(function() {
  const currentPage = +($('#currentPage').val());
  if (currentPage == 1) return;
  $('#currentPage').val(currentPage - 1);
  filter();
});

$('#btnNext').click(function() {
  const currentPage = +($('#currentPage').val());
  $('#currentPage').val(currentPage + 1);
  filter();
});

// clean modal-backdrop
$('.modal-backdrop').remove();