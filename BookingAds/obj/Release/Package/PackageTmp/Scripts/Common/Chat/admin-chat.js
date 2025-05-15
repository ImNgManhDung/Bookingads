$(document).ready(function () {
    // active menu item
    const slug = window.location.pathname.substring(7);
    console.log('slug: ', window.location.pathname);

    if (!slug && window.location.pathname === '/Admin') {
        $('#tabDashboard').addClass('active');
    }

    if (slug.length > 0
        && !slug.includes('/')
        && $(`#tab${slug}`)
        && $(`#tab${slug}`).length) {
        $(`#tab${slug}`).addClass('active');
    }

    $('#btnIconBell').click(function () {
        $('#activeNotify').hide();
    });

    $('#badgeNotifyOrder').click(function () {
        $('#badgeNotifyOrder').text('0');
        $('#badgeNotifyOrder').hide();
    });

    $('#badgeNotifyMessage').click(function () {
        $('#badgeNotifyMessage').text('0');
        $('#badgeNotifyMessage').hide();
    });

    // Update Avatar
    $('#btnUpdateAvatar').click(function (e) {
        let isUpdateAvatar = window.confirm("Bạn có chắc chắn cập nhật lại ảnh đại diện không?");
        if (!isUpdateAvatar) {
            e.preventDefault();
            return;
        }

        $('#avatarUploaded').change(function () {
            let fileInfo = $(this)[0].files[0];
            let src = window.URL.createObjectURL(fileInfo);
            $('#adminAvatar').attr('src', src);

            // call ajax
            let dataDto = new FormData();
            dataDto.append('avatarUploaded', fileInfo);

            $.ajax({
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data 
                url: '/Admin/Account/Update',
                method: "POST",
                data: dataDto,
                success: function (res) {
                    if (res) {
                        alert(res);
                    }
                },
                error: function (err) {
                    console.log("===> err: ", err);
                }
            });
        });
    });

    // Chat

    const keyLimitLocalStorage = '_limit';
    const keyIsEdit = 'isEdit';

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

    // set position cursor when focus has data
    function setCarat(element) {
        // Place cursor at the end of a content editable div
        if (element.type !== "textarea" && element.getAttribute("contenteditable") === "true") {
            element.focus()
            window.getSelection().selectAllChildren(element)
            window.getSelection().collapseToEnd()
        } else {
            // Place cursor at the end of text areas and input elements
            element.focus()
            element.select()
            window.getSelection().collapseToEnd()
        }
    }

    // get param in url
    function getUrlVars() {
        let vars = [], hash;
        let hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    }

    // remove query param in url when click other username
    function removeQueryParamInUrl() {
        let uri = window.location.href.toString();
        if (uri.indexOf("?") > 0) {
            let clean_uri = uri.substring(0, uri.indexOf("?"));
            window.history.replaceState({}, document.title, clean_uri);
        }
    }

    // get chat history if userName query param is exists
    const otherUserNameQueryParam = getUrlVars().userName;
    if (otherUserNameQueryParam) {
        const itemOtherUser = $('.itemOtherUser');

        $.each(itemOtherUser, function (index, item) {
            const itemOtherUserName = $(item).attr('data-userName');
            if (itemOtherUserName === otherUserNameQueryParam) {
                getChatHistory($(item));
                return;
            }
        });
    }

    // get chat history
    function getChatHistory(itemOtherUser) {
        $('.itemOtherUser').removeClass('active');
        itemOtherUser.addClass('active');

        // show info user
        const employeeUsername = itemOtherUser.attr('data-userName');
        const employeeFullName = itemOtherUser.attr('data-fullName');
        const employeeAvatar = itemOtherUser.attr('data-avatar');
        $('#otherUserName').val(employeeUsername);

        const htmls = `<div class="row">
                            <div class="col-lg-12" style="display: flex; align-items: center; justify-content: center;">
                                <a href="javascript:void(0);" data-toggle="modal" data-target="#view_info">
                                    <img src="/Images/Employees/${employeeAvatar}" alt="avatar">
                                </a>
                                <div class="chat-about">
                                    <h4 class="m-b-0">${employeeFullName}</h4>
                                </div>
                            </div>
                       </div>`;

        // $('#chatHeader').show();
        $('#chatHeader').html(htmls);

        if ($(window).width() < 768) {
            $('#plist').hide();
            $('#plist').removeClass('open');
        }

        // $('#actionChat').show();

        // show chat history
        const loadHTML = `<li class="text-center text-success" style="font-weight: bold;">
                                                        <span class="glyphicon glyphicon-refresh spinning"></span> Đang tải dữ liệu...
                                                      </li>`;
        $('#listChatHistory').html(loadHTML);

        // load more message default
        localStorage.removeItem(keyLimitLocalStorage);
        loadMoreMessage();
    }

    // create message
    function createMessage(hub, isEdit = false) {
        const msg = HtmlToPlainText($('#chatContent').text());
        if (!msg) return;

        $('#chatContent').text('');
        $('#chatContent').focus();
        $('.chat-history').scrollTop(999999);

        // set data for chat hub
        const messageID = +($('#editingMessageID').val());
        const myUserName = $('#currentUserName').val();
        const otherUserName = $('#otherUserName').val();
        const createdTime = formatDate(new Date());
        if (!myUserName || !otherUserName || !msg || !createdTime) {
            return;
        }

        // edit message if flag is true
        if (isEdit) {
            // call ajax
            const dataDto = {
                MessageID: messageID,
                SenderID: myUserName,
                Content: msg,
            }
            console.log('dữ liệu gọi ajax sửa tin nhắn trong quản trị viên: ', dataDto);

            $.ajax({
                contentType: 'application/json',
                url: '/Admin/ManageEmployee/Edit',
                method: "POST",
                data: JSON.stringify(dataDto),
                success: function (res) {
                    console.log("===> res: ", res);
                },
                error: function (err) {
                    console.log("===> err: ", err);
                }
            });

            $(`#content-${messageID}`).text(msg);
            // hub.server.sendPrivate(myUserName, msg, otherUserName, messageID);

            return;
        }

        // hub.server.sendPrivate(myUserName, msg, otherUserName);

        // call ajax
        let dataDto = {
            SenderID: myUserName,
            ReceiverID: otherUserName,
            Content: msg,
            CreatedTime: createdTime,
        }
        console.log('dữ liệu gọi ajax trong quản trị viên: ', dataDto);

        $.ajax({
            contentType: 'application/json',
            url: '/Admin/ManageEmployee/Chat',
            method: "POST",
            data: JSON.stringify(dataDto),
            success: function (newMessageID) {
                console.log("===> newMessageID: ", newMessageID);
                $('#newMessageID').val(newMessageID);
                hub.server.sendPrivate(myUserName, msg, otherUserName);
                // hub.server.sendPrivate(myUserName, msg, otherUserName, newMessageID);
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

        // call ajax
        const dataDto = {
            SenderID: otherUserName,
            ReceiverID: myUserName,
            ReadTime: readTime
        }
        console.log('dữ liệu gọi ajax đọc tin nhắn trong quản trị viên: ', dataDto);

        $.ajax({
            contentType: 'application/json',
            url: '/Admin/ManageEmployee/Read',
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
        const otherUserNameQueryParam = getUrlVars().userName;
        const otherUserName = otherUserNameQueryParam ?? $('#otherUserName').val();
        const countChatHistory = $('#countChatHistory').val();

        const loadHTML = `<li class="text-center text-success" style="font-weight: bold;">
                            <span class="glyphicon glyphicon-refresh spinning"></span> Đang tải dữ liệu...
                          </li>`;

        $('#listChatHistory').html(loadHTML);

        // call ajax
        $.ajax({
            contentType: 'application/json',
            url: '/Admin/ManageEmployee/ChatHistory',
            method: "GET",
            //data: JSON.stringify({
            //    userName: otherUserName,
            //    limit: limit
            //}),
            data: {
                userName: otherUserName,
                limit: limit
            },
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

                    $('#chatHeader').show();
                    $('#actionChat').show();
                    $('#chatContent').focus();
                    $('#listEmployees').css('height', '500px');
                    // readMessage(hub);

                    // toggle item message
                    $('.my-message').mouseover(function () {
                        $($(this).children()[1]).show();
                    });

                    $('.my-message').mouseout(function () {
                        $('.btnIconToolMsg').hide();
                    });


                    // edit message
                    $('.btnEditMessage').click(function () {
                        const messageID = $(this).attr('data-messageID');
                        $('#editingMessageID').val(messageID);
                        const oldMessageContent = $(`#content-${messageID}`).text();

                        $('#chatContent').text('');
                        $('#chatContent').text(oldMessageContent);
                        $('#chatContent').css('height', '100%');
                        setCarat(document.getElementById('chatContent'));

                        $('#editingContent').text(oldMessageContent);
                        $('#areaEditMessage').css('display', 'flex');

                        $(`.my-message`).css('background', '#efefef');
                        $(`#areaMyMessage-${messageID} .my-message`).css('background', 'antiquewhite');

                        localStorage.setItem(keyIsEdit, '1');
                    });

                    // cancel message
                    $('#cancelEditingMessage').click(function () {
                        $('#editingContent').text('');
                        $('#chatContent').text('');
                        $('#areaEditMessage').css('display', 'none');
                        $(`.my-message`).css('background', '#efefef');
                        setCarat(document.getElementById('chatContent'));

                        localStorage.removeItem(keyIsEdit);
                    });

                    // delete message
                    $('.btnDeleteMessage').click(function () {
                        const myUserName = $('#currentUserName').val();
                        const messageID = $(this).attr('data-messageID');
                        const isDelete = window.confirm("Bạn có chắc chắn xóa tin nhắn này không?");
                        if (!isDelete) {
                            return;
                        }

                        const dataDto = {
                            MessageID: messageID,
                            SenderID: myUserName,
                        };

                        $.ajax({
                            contentType: 'application/json',
                            url: '/Admin/ManageEmployee/Delete',
                            method: "POST",
                            data: JSON.stringify(dataDto),
                            success: function (res) {
                                console.log("===> res: ", res);
                            },
                            error: function (err) {
                                console.log("===> err: ", err);
                            }
                        });

                        $(`#areaMyMessage-${messageID}`).remove();
                    });

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
    const orderHub = $.connection.orderHub;

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
            console.log('===> quản trị viên nhận: ', data);

            const myUserName = $('#currentUserName').val();
            const otherUserName = $('#otherUserName').val();
            const newMessageID = $('#newMessageID').val();
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
                                    <li id="areaMyMessage-${newMessageID}" class="chat-my-message clearfix"
                                        title="Đã gửi vào lúc ${data.CreatedTime}">
                                        <div class="message my-message float-right">
                                            <span id="content-${newMessageID}">${data.Content}</span>
                                            <div class="dropdown user user-menu btnIconToolMsg">
                                            <div class="dropdown-toggle glyphicon glyphicon-option-horizontal" 
                                                    data-toggle="dropdown" aria-expanded="true"></div>
                                            <ul class="dropdown-menu sidebar-menu tree" data-widget="tree" >
                                                <li class="btnEditMessage" data-messageID="${newMessageID}">
                                                    <a href="javascript:void(0)">
                                                        <i class="glyphicon glyphicon-edit"></i>
                                                        <span>Sửa tin nhắn</span>
                                                    </a>
                                                </li>

                                                <li class="btnDeleteMessage" data-messageID="${newMessageID}">
                                                    <a href="javascript:void(0)">
                                                        <i class="glyphicon glyphicon-trash"></i>
                                                        <span>Xóa tin nhắn</span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </div>
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

            // edit message
            $('.btnEditMessage').click(function () {
                const messageID = $(this).attr('data-messageID');
                $('#editingMessageID').val(messageID);
                const oldMessageContent = $(`#content-${messageID}`).text();

                $('#chatContent').text('');
                $('#chatContent').text(oldMessageContent);
                $('#chatContent').css('height', '100%');
                setCarat(document.getElementById('chatContent'));

                $('#editingContent').text(oldMessageContent);
                $('#areaEditMessage').css('display', 'flex');

                $(`.my-message`).css('background', '#efefef');
                $(`#areaMyMessage-${messageID} .my-message`).css('background', 'antiquewhite');

                localStorage.setItem(keyIsEdit, '1');
            });

            // cancel message
            $('#cancelEditingMessage').click(function () {
                $('#editingContent').text('');
                $('#chatContent').text('');
                $('#areaEditMessage').css('display', 'none');
                $(`.my-message`).css('background', '#efefef');
                setCarat(document.getElementById('chatContent'));

                localStorage.removeItem(keyIsEdit);
            });

            // delete message
            $('.btnDeleteMessage').click(function () {
                const myUserName = $('#currentUserName').val();
                const messageID = $(this).attr('data-messageID');
                const isDelete = window.confirm("Bạn có chắc chắn xóa tin nhắn này không?");
                if (!isDelete) {
                    return;
                }

                const dataDto = {
                    MessageID: messageID,
                    SenderID: myUserName,
                };

                $.ajax({
                    contentType: 'application/json',
                    url: '/Admin/ManageEmployee/Delete',
                    method: "POST",
                    data: JSON.stringify(dataDto),
                    success: function (res) {
                        console.log("===> res: ", res);
                    },
                    error: function (err) {
                        console.log("===> err: ", err);
                    }
                });

                $(`#areaMyMessage-${messageID}`).remove();
            });
        }

        // read private
        hub.client.ReadMessagePrivateMsg = function (data) {
            console.log('===> quản trị viên đọc: ', data);
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

    // process notification order
    if (orderHub) {
        orderHub.client.NotifyOrderMsg = function (data) {
            // fromUserName, notificationData, toUserNames
            // console.log('[layout] ===> order info in admin: ', data);
            let badgeNotifyOrder = +($('#badgeNotifyOrder').text()) + 1;
            $('#badgeNotifyOrder').text(badgeNotifyOrder);

            $('#activeNotify').show();
            $('#badgeNotifyOrder').show();
        }
    }

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
            console.log("Đã kết nối Hub ở trong quản trị viên !!!");

            // send message
            $('#btnSendMessage').click(function () {
                const isEdit = localStorage.getItem(keyIsEdit);

                // create or edit msg
                createMessage(hub, isEdit && isEdit.length > 0);
                $('#editingContent').text('');
                $('#chatContent').text('');
                $('#areaEditMessage').css('display', 'none');
                $(`.my-message`).css('background', '#efefef');
                setCarat(document.getElementById('chatContent'));

                localStorage.removeItem(keyIsEdit);
            });

            // enter and shift + enter
            $('#chatContent').keypress(function (e) {
                if (e.which === 13 && !e.shiftKey) {
                    e.preventDefault();

                    const isEdit = localStorage.getItem(keyIsEdit);

                    // create or edit msg
                    createMessage(hub, isEdit && isEdit.length > 0);
                    $('#editingContent').text('');
                    $('#chatContent').text('');
                    $('#areaEditMessage').css('display', 'none');
                    $(`.my-message`).css('background', '#efefef');
                    setCarat(document.getElementById('chatContent'));

                    localStorage.removeItem(keyIsEdit);
                }
            });

            // read message
            $('#chatContent').focus(function () {
                readMessage(hub);
            });

            // show info user and chat history
            $('.itemOtherUser').click(function () {
                removeQueryParamInUrl();
                getChatHistory($(this));
            });

            // disconnect
            $('#btnLogout').click(function () {
                const currentUserName = $('#currentUserName').val();
                hub.server.offline(currentUserName);
            });
        })
        .fail(function () {
            console.log("Không thể kết nối hub ở trong quản trị viên !");
        });
});