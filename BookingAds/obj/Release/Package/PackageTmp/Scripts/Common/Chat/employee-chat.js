$(document).ready(function () {
    // setting notification
    $('#btnIconBell').click(function () {
        $('#activeNotify').hide();
    });

    $('#badgeNotifyOrder').click(function () {
        $('#badgeNotifyOrder').text('0');
        $('#badgeNotifyOrder').hide();
    });

    $('#badgeNotifyMesseger').click(function () {
        $('#badgeNotifyMesseger').text('0');
        $('#badgeNotifyMesseger').hide();
    });

    const keyLimitLocalStorage = '_limit';

    const chatGPT = 'ChatGPT';

    // format date: dd/MM/yyyy HH:mm:ss tt
    function formatDate(now) {
        const day = now.getDate() < 10 ? ('0' + now.getDate()) : now.getDate();
        const month = (now.getMonth() + 1) < 10 ? ('0' + (now.getMonth() + 1)) : (now.getMonth() + 1);
        const year = now.getFullYear();
        let hours = now.getHours();
        const minutes = now.getMinutes() < 10 ? ('0' + now.getMinutes()) : now.getMinutes();
        const seconds = now.getSeconds() < 10 ? ('0' + now.getSeconds()) : now.getSeconds();
        const times = hours < 12 ? 'AM' : 'PM';
        hours = hours % 12;
        hours = hours ? hours : 12;
        hours = hours < 10 ? '0' + hours : hours;

        return `${day}/${month}/${year} ${hours}:${minutes}:${seconds} ${times}`;
    }

    // convert html to plain text
    function HtmlToPlainText(html) {
        html = html.replace(/<style([\s\S]*?)<\/style>/gi, '');
        html = html.replace(/<script([\s\S]*?)<\/script>/gi, '');
        html = html.replace(/<\/div>/ig, '\n');
        html = html.replace(/<\/li>/ig, '\n');
        html = html.replace(/<li>/ig, '  *  ');
        html = html.replace(/<\/ul>/ig, '\n');
        html = html.replace(/<\/p>/ig, '\n');
        html = html.replace(/<br\s*[\/]?>/gi, "\n");
        html = html.replace(/<[^>]+>/ig, '');

        return html;
    }

    // console.log('date: ', formatDate(new Date()));

    // create message
    function createMessage(hub) {
        const msg = HtmlToPlainText($('#chatContent').text());
        if (!msg) return;

        $('#chatContent').text('');
        $('#chatContent').focus();
        $('.chat-history').scrollTop(999999);

        // set data for chat hub
        const myUserName = $('#currentUserName').val();
        const otherUserName = $('#otherUserName').val();
        const createdTime = formatDate(new Date());

        if (!myUserName || !otherUserName || !msg || !createdTime) {
            return;
        }

        hub.server.sendPrivate(myUserName, msg, otherUserName);

        // if otherUserName is AI
        if (otherUserName === chatGPT) {
            $('#chatGPTTyping').show();
        }

        // call ajax
        const dataDto = {
            SenderID: myUserName,
            ReceiverID: otherUserName,
            Content: msg,
            CreatedTime: createdTime
        }
        // console.log('dữ liệu gọi ajax tạo tin nhắn trong khách hàng: ', dataDto);

        $.ajax({
            contentType: 'application/json',
            url: '/Messenger/Chat',
            method: "POST",
            data: JSON.stringify(dataDto),
            success: function (res) {
                console.log("===> res: ", res);

                // when chat gpt create answer successfully and send realtime to user
                if (res.SenderID === chatGPT) {
                    setTimeout(() => {
                        $('#chatGPTTyping').hide();
                        hub.server.sendPrivate(res.SenderID, res.Content, res.ReceiverID);
                    }, 2000);
                }
            },
            error: function (err) {
                console.log("===> err: ", err);
            }
        });
    }

    // read message
    function readMessage(hub) {
        const myUserName = $('#currentUserName').val();
        const otherUserName = $('#otherUserName').val();
        const readTime = formatDate(new Date());
        const myMessages = $('.message-sender');
        let countMyMessage = 0;
        const ourMessages = $('.message-receiver');
        let countOurMessage = 0;

        $.each(myMessages, function (index, item) {
            const hasChildren = $(item).children().length > 0;
            if (!hasChildren) {
                countMyMessage++;
            }
        });

        $.each(ourMessages, function (index, item) {
            const title = $(item).attr('title');
            if (!title) {
                countOurMessage++;
            }
        });

        if (!myUserName
            || !otherUserName
            || !readTime
            || countMyMessage == 0
            && countOurMessage == 0) {
            return;
        }

        hub.server.readMessagePrivate(otherUserName, myUserName);

        // if otherUserName is AI
        if (otherUserName === chatGPT) {
            return;
        }

        // call ajax
        const dataDto = {
            SenderID: otherUserName,
            ReceiverID: myUserName,
            ReadTime: readTime
        }
        // console.log('dữ liệu gọi ajax đọc tin nhắn trong khách hàng: ', dataDto);

        $.ajax({
            contentType: 'application/json',
            url: '/Messenger/Read',
            method: "POST",
            data: JSON.stringify(dataDto),
            success: function (res) {
                // console.log("===> res: ", res);
            },
            error: function (err) {
                console.log("===> err: ", err);
            }
        });
    }

    // load more message
    function loadMoreMessage(limit = 5) {
        const otherUserName = $('#otherUserName').val();
        const countChatHistory = $('#countChatHistory').val();

        const loadHTML = `<li class="text-center text-success" style="font-weight: bold;">
                                                    <span class="glyphicon glyphicon-refresh spinning"></span> Đang tải dữ liệu...
                                                    </li>`;

        $('#listChatHistory').html(loadHTML);

        // call ajax
        $.ajax({
            contentType: 'application/json',
            url: '/Messenger/ChatHistory',
            method: "POST",
            data: JSON.stringify({
                userName: otherUserName,
                limit: limit
            }),
            success: function (res) {
                setTimeout(() => {
                    $('#listChatHistory').empty();
                    $('#listChatHistory').html(res);

                    if (limit != 5) {
                        $('#oldCountChatHistory').val(countChatHistory);
                    }

                    if (limit === 5) {
                        $('.chat-history').scrollTop(999999);
                    }
                }, 500);
            },
            error: function (err) {
                console.log('===> err: ', err);
            }
        });
    }

    // scroll top
    $('.chat-history').on('scroll', function () {

        // when scroll will auto load more 5 message
        if ($(this).scrollTop() === 0) {
            // check load all message
            const countChatHistory = $('#countChatHistory').val();
            const oldCountChatHistory = $('#oldCountChatHistory').val();

            if (countChatHistory
                && oldCountChatHistory
                && oldCountChatHistory === countChatHistory) {
                localStorage.removeItem(keyLimitLocalStorage);
                return;
            }

            const _limitLocalStorage = localStorage.getItem(keyLimitLocalStorage);
            if (!_limitLocalStorage) {
                localStorage.setItem(keyLimitLocalStorage, 10);
                loadMoreMessage(10);
            } else {
                const newLimit = +(_limitLocalStorage) + 5;
                loadMoreMessage(newLimit);
                localStorage.setItem(keyLimitLocalStorage, newLimit);
            }
        }
    });

    // get data from client via hub
    const hub = $.connection.chatHub;

    if (hub) {
        // get user online
        hub.client.Online = function (onlines) {
            console.log('===> người dùng đang hoạt động: ', onlines);

            // set offline
            $('.statusUser i').removeClass('online');
            $(`.statusUser i`).addClass('offline');
            $(`.statusUser span`).text('ngoại tuyến');
            $(`.statusUser span`).removeClass('text-success');

            // set online
            for (online of onlines) {
                const onlineID = `#status-${online}`;
                $(`${onlineID} i`).removeClass('offline');
                $(`${onlineID} i`).addClass('online');
                $(`${onlineID} span`).text('trực tuyến');
                $(`${onlineID} span`).addClass('text-success');
            }
        }

        // send private
        hub.client.SendPrivateMsg = function (data) {
            console.log('===> khách hàng nhận: ', data);

            const myUserName = $('#currentUserName').val();
            const otherUserName = $('#otherUserName').val();
            let receiverHtmls = '';

            // show noti
            if (myUserName === data.OtherUserName) {
                let badgeNotifyMessage = +($('#badgeNotifyMessage').text()) + 1;
                $('#badgeNotifyMessage').text(badgeNotifyMessage);

                $('#activeNotify').show();
                $('#badgeNotifyMessage').show();
            }

            // no choose conversation
            if (!otherUserName) {
                return;
            }

            // show message for me with I am sender
            if (myUserName === data.MyUserName && otherUserName === data.OtherUserName) {
                receiverHtmls += `
                                <li class="chat-my-message clearfix" title="Đã gửi vào lúc ${data.CreatedTime}">
                                    <div class="message my-message float-right">
                                        ${data.Content}
                                    </div>
                                    <div class="message-data float-right message-sender"></div>
                                </li>
                            `;
            }

            // show message for me with I am receiver
            if (myUserName === data.OtherUserName && otherUserName === data.MyUserName) {
                receiverHtmls += `
                                <li class="clearfix message-receiver">
                                    <div class="message-data">
                                        <span class="message-data-time">${data.CreatedTime}</span>
                                    </div>
                                    <div class="message other-message">${data.Content}</div>
                                </li>
                            `;
            }

            console.log('Tôi là: ', myUserName, ' Hiện tại đang ở trong tin nhắn của: ', otherUserName);
            console.log('Tin nhắn của người gửi: ', data.MyUserName, ' được chuyển đến: ', data.OtherUserName);

            $('#listChatHistory').append(receiverHtmls);
            $('.chat-history').scrollTop(999999);
        }

        // read private
        hub.client.ReadMessagePrivateMsg = function (data) {
            console.log('===> khách hàng đọc: ', data);
            const myUserName = $('#currentUserName').val();
            const otherUserName = $('#otherUserName').val();
            let readHtmls = '';
            let readTitle = '';

            // no choose conversation
            if (!otherUserName) {
                return;
            }

            // show message for me with I am sender
            if (myUserName === data.MyUserName && otherUserName === data.OtherUserName) {
                readHtmls += `
                                <small class="message-data-time">
                                    <i class="fa fa-check"></i>
                                    ${data.ReadTime}
                                </small>
                            `;

                const myMessages = $('.message-sender');
                $.each(myMessages, function (index, item) {
                    const hasChildren = $(item).children().length > 0;
                    if (!hasChildren) {
                        // console.log('===> Tin nhắn của tôi gửi mà họ chưa đọc là: ', item);
                        $(item).html(readHtmls);
                    }
                });
            }

            // show message for me with I am receiver
            if (myUserName === data.OtherUserName && otherUserName === data.MyUserName) {
                readTitle = `Bạn đã xem vào lúc ${data.ReadTime}`;
                const ourMessages = $('.message-receiver');
                $.each(ourMessages, function (index, item) {
                    const title = $(item).attr('title');
                    if (!title) {
                        // console.log('===> Tin nhắn chưa đọc của tôi là: ', item);
                        $(item).attr('title', readTitle);
                    }
                });
            }

            $('.chat-history').scrollTop(999999);

        }
    }


    //function togglePlistVisibility() {
    //    var screenWidth = window.innerWidth;
    //    var plist = document.getElementById('plist');

    //    if (plist) {
    //        if (screenWidth <= 760) {
    //            plist.style.display = 'none';
    //        } else {
    //            plist.style.display = 'block';
    //        }
    //    }
    //}
    //window.addEventListener('load', togglePlistVisibility);
    //window.addEventListener('resize', togglePlistVisibility);



    // show list employees
    $('#btnListEmployeesReponsive').click(function () {
        if (!$('#plist').hasClass('open')) {
            $('#plist').show();
            $('#plist').addClass('open');
        } else {
            $('#plist').hide();
            $('#plist').removeClass('open');
        }
    });

    // set data from server via hub
    $.connection.hub.start()
        .done(function () {
            console.log("Đã kết nối Hub ở trong khách hàng !!!");

            // send message
            $('#btnSendMessage').click(function () {

                // create msg
                createMessage(hub);
            });

            // enter and shift + enter
            $('#chatContent').keypress(function (e) {
                if (e.which === 13 && !e.shiftKey) {
                    e.preventDefault();

                    // create msg
                    createMessage(hub);
                }
            });

            // read message
            $('#chatContent').focus(function () {
                readMessage(hub);
            });

            // show list admins
            $('#btnListAdminsReponsive').click(function () {
                if (!$('#plist').hasClass('open')) {
                    $('#plist').show();
                    $('#plist').addClass('open');
                } else {
                    $('#plist').hide();
                    $('#plist').removeClass('open');
                }
            });

            // show info user and chat history
            $('.itemOtherUser').click(function () {
                $('.itemOtherUser').removeClass('active');
                $(this).addClass('active');

                // show info user
                const userName = $(this).attr('data-userName');
                const avatar = $(this).attr('data-avatar');
                $('#otherUserName').val(userName);

                const htmls = `
                            <div class="row">
                                    <div class="col-lg-12" style="display: flex; align-items: center; justify-content: center;">
                                        <a href="javascript:void(0);" data-toggle="modal" data-target="#view_info">
                                            <img src="/Images/Admin/${avatar}" alt="avatar">
                                        </a>
                                        <div class="chat-about">
                                            <h4 class="m-b-0">${userName}</h4>
                                        </div>
                                    </div>
                                </div>
                            `;
                $('#chatHeader').show();
                $('#listChats').css('height', '500px');

                if ($(window).width() < 768) {
                    $('#plist').hide();
                    $('#plist').removeClass('open');
                }

                if (userName === chatGPT) {
                    const htmls = $(this).html();
                    $('#chatHeader').html(htmls);
                } else {
                    $('#chatHeader').html(htmls);
                }


                $('#actionChat').show();

                // load more message default
                localStorage.removeItem(keyLimitLocalStorage);
                loadMoreMessage();
            });

            // disconnect
            $('#btnLogout').click(function () {
                const currentUserName = $('#currentUserName').val();
                hub.server.offline(currentUserName);
            });
        })
        .fail(function () {
            console.log("Không thể kết nối hub ở trong khách hàng !");
        });

    // scroll top
    $("#areaMovePageTop").on("click", function () {
        if ('scrollBehavior' in document.documentElement.style) {
            window.scrollTo({ top: 0, behavior: 'smooth' }); //Edge not working 'smooth'
        } else {
            $("html, body").animate({ scrollTop: 0 }, 500);
        }
        var scrollTopValue = $(window).scrollTop();
        console.log(scrollTopValue);
    });

    $(window).on("scroll", handleWindowScroll);
    handleWindowScroll();

    function handleWindowScroll() {
        var pageWidth = $(document).height(); // Lấy chiều rộng của trang web
        var areaWidth = pageWidth / 3; // Tính chiều rộng cần hiển thị #areaMovePageTop

        if ($(window).scrollTop() <= areaWidth) {
            $("#areaMovePageTop").addClass("hide");
        } else {
            $("#areaMovePageTop").removeClass("hide");
        }
    }
});