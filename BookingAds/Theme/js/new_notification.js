function socket_notification($date_socket) {
  var $url = $date_socket.url;
  var $title = $date_socket.title;
  var $tag = $date_socket.tag;
  var $header = $date_socket.header;
  var $opt = { body: $title, tag: $tag, timeout: "7000", url: $url };
  alarm($header, $opt);
}
function call_notification($data_socket) {
  $from = $data_socket.from;
  $data_socket.action = "get_call";
  $.post("ajax_getinfo.php", $data_socket, function ($data) {
    $("#call_phone").hide();
    $("#call_notification").show();
    $("#call_notification div[rel='info_user']").html($data);
  });
}
function support_notification($type) {
  return true;
  $.post(
    "ajax_getinfo.php",
    { action: "get_notification", type: $type },
    function ($data) {
      if ($data.code == 1) {
        $("#support_notification > ul ").prepend($data.notification);
        var $new_comment = $(
          "#support_notification ul > .new_notification"
        ).length;
        if ($new_comment > 0) {
          $(".total_comment sup").show();
          $(".total_comment i").addClass("icon-comment-click");
          $(".new_message span").html($new_comment);
        } else {
          $(".total_comment sup").hide();
          $(".new_message span").html("");
        }
        if ($new_comment > 9) {
          $new_comment = "9+";
        }
        $(".total_comment sup").text($new_comment);
      }
      var $new_message = $data.new_message;
      if ($type == "new") {
        if ($new_message > 0) {
          var $date_socket = $data.date_socket;
          $.each($date_socket, function ($key, $data) {
            socket_notification($data);
          });
        }
        setTimeout(function () {
          support_notification("new");
        }, 150000);
      } else {
        if ($new_message > 0) {
          $date_socket = {};
          $date_socket.url = "#";
          $date_socket.title = "BẠN có " + $new_message + " tin nhắn chưa đọc";
          $date_socket.tag = "1";
          $date_socket.header = "TIN NHẮN MỚI";
          socket_notification($date_socket);
        }
      }
    },
    "json"
  );
}
jQuery(function ($) {
  setTimeout(function () {
    support_notification("full");
  }, 10000);
  setTimeout(function () {
    support_notification("new");
  }, 120000);
  $(".call_display").on("click", function () {
    $(this).next().show();
    $(this).hide();
  });
  $("#call_phone").on("click", function () {
    var $data_socket = {};
    $data_socket.from = $(this).attr("rel");
    $data_socket.to = $to_ext;
    console.log($data_socket);
    $(this).html("<p>Đợi chút ^^!</p>");
    $(this).attr("rel", "");
    $("ul[rel='info_user'").html("");
    $("div[rel='job'").remove();
    call_notification($data_socket);
  });
  $("#btn_note_call").on("click", function () {
    var $user_id = $("#frm_note_call input[name='user_id']").val();
    var $phone = "";
    var $user_realname = "";
    var $address = "";
    var $email = "";
    var $type_user = $("#frm_note_call input[name='user_id']").attr(
      "type_user"
    );

    if ($type_user == "user_spam") {
      $phone = $("#frm_note_call input[name='user_phone']").val();
      $user_realname = $("#frm_note_call input[name='user_realname']").val();
      $address = $("#frm_note_call input[name='user_address']").val();
      $email = $("#frm_note_call input[name='user_email']").val();
    }

    var $note = $("#frm_note_call textarea[name='user_note']").val();
    var $data_socket = {};
    $data_socket.action = "note_user_call";
    $data_socket.phone = $phone;
    $data_socket.realname = $user_realname;
    $data_socket.address = $address;
    $data_socket.email = $email;
    $data_socket.note = $note;
    $data_socket.user_id = $user_id;
    $data_socket.type_user = $type_user;

    $.post(
      "ajax_getinfo.php",
      $data_socket,
      function ($data) {
        if ($data.code == 0) {
          alert($data.msg);
        } else {
          $("#frm_note_call textarea[name='user_note']").val("");
          $("#rs_note").prepend($data.note);
        }
      },
      "json"
    );
    return false;
  });
  $("#call_close").on("click", function () {
    $("#call_notification").hide();
    var $data_socket = {};
    $data_socket.action = "call_close";
    $data_socket.to = $to_ext;
    socket.emit("message", $data_socket);
  });
  $("#support_notification").on("click", "span", function () {
    var $id_notification = $(this).attr("rel");
    var $t = $(this);
    $t.parent().remove();
    $.post("ajax_getinfo.php", {
      action: "remove_notification",
      id_notification: $id_notification,
    });
  });
  $(".total_comment").bind("click", function () {
    if ($("#support_notification").is(":visible")) {
      // $("#support_notification").hide();
      $(".total_comment sup").text("").hide();
      $(".new_message span").html("");
      $(".total_comment i").removeClass("icon-comment-click");
      $("#support_notification ul > li").removeClass("new_notification");
    } else {
      var $new_comment = $(
        "#support_notification ul > .new_notification"
      ).length; /*if($new_comment>0)
    {$(".new_message").show();}else
    {$(".new_message").hide();}*/
      // $("#support_notification").show();
      $(".total_comment sup").hide();
      $(".total_comment i").addClass("icon-comment-click");
      return true;
      $.post("ajax_getinfo.php", { action: "read_notification" });
    }
  });
});
