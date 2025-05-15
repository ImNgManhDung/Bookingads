jQuery(document).ready(function () {
  jQuery("#accordion .keyword_collapse").on("click", function (e) {
    e.preventDefault();
    var target = jQuery(this).attr("data-target");
    if (jQuery("#" + target).is(":hidden") == true) {
      jQuery(this).removeClass("collapsed");
      jQuery("#" + target).slideDown("slow");
    } else {
      jQuery(this).addClass("collapsed");
      jQuery("#" + target).slideUp("slow");
    }
  });
});
function flashWrite(url, w, h, id, bg, vars, win) {
  var flashStr =
    "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' codebase='http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=8,0,0,0' width='" +
    w +
    "' height='" +
    h +
    "' id='" +
    id +
    "' align='middle'>" +
    "<param name='allowScriptAccess' value='always' />" +
    "<param name='movie' value='" +
    url +
    "' />" +
    "<param name='FlashVars' value='" +
    vars +
    "' />" +
    "<param name='wmode' value='" +
    win +
    "' />" +
    "<param name='menu' value='false' />" +
    "<param name='quality' value='high' />" +
    "<param name='bgcolor' value='" +
    bg +
    "' />" +
    "<embed src='" +
    url +
    "' FlashVars='" +
    vars +
    "' wmode='" +
    win +
    "' menu='false' quality='high' bgcolor='" +
    bg +
    "' width='" +
    w +
    "' height='" +
    h +
    "' name='" +
    id +
    "' align='middle' allowScriptAccess='always' type='application/x-shockwave-flash' pluginspage='http://www.macromedia.com/go/getflashplayer' />" +
    "</object>";
  document.write(flashStr);
}
function openBox(fileSrc, winWidth, winHeight) {
  var w = (screen.availWidth - winWidth) / 2;
  var h = (screen.availHeight - winHeight) / 2;
  newParameter =
    "width=" +
    winWidth +
    ",height=" +
    winHeight +
    ",addressbar=no,scrollbars=yes,toolbar=no,top=" +
    h +
    ",left=" +
    w +
    ", resizable=no";
  newWindow = window.open(fileSrc, "a", newParameter);
  newWindow.focus();
}
function modelessDialogShow(url, width, height) {
  var w = (screen.availWidth - width) / 2;
  var h = (screen.availHeight - height) / 2;
  return window.showModalDialog(
    url,
    window,
    "dialogWidth:" +
      width +
      "px; dialogHeight:" +
      height +
      "px; center:1; dialogLeft:" +
      w +
      "px; dialogTop:" +
      h +
      "px; help:off; resizable:on; status:off;"
  );
}
function newWindow(mypage, myname, w, h, scrolla) {
  var winl = (screen.availWidth - w) / 2;
  var wint = (screen.availHeight - h) / 2;
  winprops =
    "height=" +
    h +
    ",width=" +
    w +
    ",top=" +
    wint +
    ",left=" +
    winl +
    ",scrollbars=" +
    scrolla +
    ",resizable=1,toolbar=yes,statusbar=yes";
  win = window.open(mypage, myname, winprops);
}
function changeLang(returnURL, lang) {
  var theExprire = 1000 * 60 * 60 * 24;
  Set_Cookie("lang", lang, theExprire, "/");
  window.location = returnURL;
}
function changeFor(returnURL, value) {
  var theExprire = 1000 * 60 * 60 * 24;
  Set_Cookie("filter", value, theExprire, "/");
  window.location = returnURL;
}
function changeSort(returnURL, value) {
  var theExprire = 1000 * 60 * 60 * 24;
  Set_Cookie("sort", value, theExprire, "/");
  window.location = returnURL;
}
$.fn.CheckBox = function ($options) {
  return this.each(function () {
    var $empty = {};
    var $settings = jQuery.extend(true, $empty, $optionsDefault, $options);
    var $parent = $(this);
    if ($parent.attr("data-default-value") != "undefined") {
      var $optionsDefault = {
        defaultValue: $parent.attr("data-default-value"),
      };
    } else {
      var $optionsDefault = { defaultValue: "0" };
    }
    $parent.on("click", "a.option", function (event) {
      event.preventDefault();
      event.stopPropagation();
      var $obj = $(this);
      var $input = $("input", $obj);
      $type = "CHECKBOX";
      if ($input.attr("type") != null) {
        $type = $input.attr("type").toUpperCase();
      }
      if ($obj.hasClass("selected")) {
        $obj.removeClass("selected");
        if ($type == "CHECKBOX") {
          $("input", $obj).removeAttr("checked").trigger("ChangeEvent");
        } else if ($type == "RADIO") {
          $("input", $obj).removeAttr("checked").trigger("ChangeEvent");
        } else {
          $("input", $obj).val($settings.defaultValue).trigger("ChangeEvent");
        }
      } else {
        $obj.addClass("selected");
        if ($type == "CHECKBOX") {
          $("input", $obj).attr("checked", "checked").trigger("ChangeEvent");
        } else if ($type == "RADIO") {
          $("input", $obj).attr("checked", "checked").trigger("ChangeEvent");
        } else {
          $("input", $obj).val($obj.attr("data-value")).trigger("ChangeEvent");
        }
      }
    });
  });
};
$.fn.RadioButton = function ($options) {
  return this.each(function () {
    var $empty = {};
    var $optionsDefault = { defaultValue: "0" };
    var $settings = jQuery.extend(true, $empty, $optionsDefault, $options);
    var $parent = $(this);
    if ($parent.attr("data-default-value") != "undefined") {
      var $optionsDefault = {
        defaultValue: $parent.attr("data-default-value"),
      };
    } else {
      var $optionsDefault = { defaultValue: "0" };
    }
    $parent.on("click", ".option", function (event) {
      event.preventDefault();
      event.stopPropagation();
      var $obj = $(this);
      var $input = $("input", $obj);
      $type = "RADIO";
      if ($input.attr("type") != null) {
        $type = $input.attr("type").toUpperCase();
      }
      var $other_radio = $obj.siblings(".option");
      if ($other_radio.html() == null) {
        $other_radio = $(
          ".option[data-value!='" + $obj.attr("data-value") + "']",
          $parent
        );
      }
      $obj.addClass("selected");
      if ($type == "CHECKBOX") {
        $("input", $obj).attr("checked", "checked").trigger("ChangeEvent");
      } else if ($type == "RADIO") {
        $("input", $obj).attr("checked", "checked").trigger("ChangeEvent");
      } else {
        $("input", $obj).val($obj.attr("data-value")).trigger("ChangeEvent");
      }
      if ($other_radio.html != null) {
        $other_radio.removeClass("selected");
        if ($type == "CHECKBOX") {
          $("input", $other_radio).removeAttr("checked").trigger("ChangeEvent");
        } else if ($type == "RADIO") {
          $("input", $other_radio).removeAttr("checked").trigger("ChangeEvent");
        } else {
          $("input", $other_radio)
            .val($settings.defaultValue)
            .trigger("ChangeEvent");
        }
      }
    });
  });
};
function alarm($title, $options) {
  if (typeof Notification == "undefined") {
    return false;
  }
  if (Notification.permission == "granted") {
    notice($title, $options);
  } else if (Notification.permission != "denied") {
    Notification.requestPermission(function (permission) {
      if (permission == "granted") {
        notice($title, $options);
      }
    });
  }
}
function notice($title, $options) {
  var $sound_file = "/sound/sound.wav";
  var $icon_file = "/css/images/alarm.png";
  var $empty = {};
  var d = new Date();
  var $optionsDefault = {
    body: "",
    tag: "tag-" + d.getSeconds(),
    icon: $icon_file,
    sound: false,
    timeout: 10000,
    url: "",
  };
  $info = $.extend(true, $empty, $optionsDefault, $options);
  var n = new Notification($title, {
    body: $info.body,
    icon: $info.icon,
    tag: $info.tag,
  });
  if ($info.sound) {
    $audio = new Audio($sound_file).play();
  }
  if ($info.timeout > 0) {
    setTimeout(n.close.bind(n), $info.timeout);
  }
  if ($info.url.length > 0) {
    n.addEventListener("click", function () {
      window.open($info.url);
    });
  }
}
function isMST($mst, $required) {
  if ($required == null || typeof $required == "undefined") {
    return true;
  }
  if ($mst == "" || /\s+/gi.test($mst) || /[^0-9\-]+/gi.test($mst)) {
    return false;
  }
  return true;
}
function isIP(ipaddress) {
  if (
    /^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(
      ipaddress
    )
  ) {
    return true;
  }
  return false;
}
function isDomain(domain) {
  domain = RemoveUnicode(domain);
  if (
    domain.indexOf("_") != -1 ||
    domain.indexOf("..") != -1 ||
    (domain.indexOf("--") != -1 && domain.indexOf("xn--") == -1)
  )
    return false;
  return /^[a-zA-Z0-9][a-zA-Z0-9\-\.]{0,63}[\.][a-zA-Z]{2,20}$/i.test(domain);
}
function isCNAME(domain) {
  if (
    domain.indexOf("_") != -1 ||
    domain.indexOf("..") != -1 ||
    (domain.indexOf("--") != -1 && domain.indexOf("xn--") == -1)
  )
    return false;
  domain = RemoveUnicode(domain);
  return /^[a-zA-Z0-9\-\_][a-zA-Z0-9\-\.\_]{0,200}[\.][a-zA-Z\-\.\_]{2,20}$/i.test(
    domain
  );
}
function isDomainMX(domain) {
  if (
    domain.indexOf("_") != -1 ||
    domain.indexOf("..") != -1 ||
    (domain.indexOf("--") != -1 && domain.indexOf("xn--") == -1)
  )
    return false;
  return /^[a-zA-Z0-9][a-zA-Z0-9\-\.]{0,200}[\.][a-zA-Z]{2,20}$/i.test(domain);
}
function IsValidEmail(email) {
  var pattern =
    /^([a-zA-Z0-9_\.\-])+@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
  return pattern.test(email);
}
function ValidPhone($phone) {
  if ($phone.length < 7) return false;
  return /^\+?[0-9\-]+$/.test($phone);
}
function ValidMobile($mobile) {
  if ($mobile.length < 10) return false;
  return /^0[1|9][0-9]{8,10}$/.test($mobile);
}
function setCookie(name, value, expires, path, domain, secure) {
  Set_Cookie(name, value, expires, path, domain, secure);
}
function Set_Cookie(name, value, expires, path, domain, secure) {
  expires = expires * 60 * 60 * 24 * 1000;
  var today = new Date();
  var expires_date = new Date(today.getTime() + expires);
  var cookieString =
    name +
    "=" +
    escape(value) +
    (expires ? ";expires=" + expires_date.toGMTString() : "") +
    (path ? ";path=" + path : "") +
    (domain ? ";domain=" + domain : "") +
    (secure ? ";secure" : "");
  document.cookie = cookieString;
}
function bookmark(title, url) {
  if (window.sidebar) {
    window.sidebar.addPanel(title, url, "");
  } else if (window.external) {
    window.external.AddFavorite(url, title);
  } else if (window.opera && window.print) {
    return true;
  }
}
function getCookie(Name) {
  var search = Name + "=";
  var CookieString = document.cookie;
  var result = null;
  if (CookieString.length > 0) {
    offset = CookieString.indexOf(search);
    if (offset != -1) {
      offset += search.length;
      end = CookieString.indexOf(";", offset);
      if (end == -1) {
        end = CookieString.length;
      }
      result = unescape(CookieString.substring(offset, end));
    }
  }
  return result;
}
function deleteCookie(Name, Path) {
  setCookie(Name, "Deleted", -1, Path);
}
var http_request = false;
function makerequest_vote(diem, macauhoi, key) {
  http_request = false;
  if (window.XMLHttpRequest) {
    http_request = new XMLHttpRequest();
    if (http_request.overrideMimeType) {
      http_request.overrideMimeType("text/html");
    }
  } else if (window.ActiveXObject) {
    try {
      http_request = new ActiveXObject("Msxml2.XMLHTTP");
    } catch (e) {
      try {
        http_request = new ActiveXObject("Microsoft.XMLHTTP");
      } catch (e) {}
    }
  }
  if (!http_request) {
    alert("Cannot create XMLHTTP instance");
    return false;
  }
  var a = Math.floor(Math.random() * 1000);
  http_request.onreadystatechange = alertContents_vote;
  http_request.open(
    "GET",
    "ajax_vote.php?key=" +
      key +
      "&diem=" +
      diem +
      "&macauhoi=" +
      macauhoi +
      "&a=" +
      a,
    true
  );
  http_request.send(null);
}
function alertContents_vote() {
  if (http_request.readyState == 4) {
    if (http_request.status == 200) {
      result = http_request.responseText;
      document.getElementById("div_vote").innerHTML = result;
      alert(
        "Việc đánh giá của quý khách đã được chúng tôi ghi nhận, xin cảm ơn."
      );
    } else {
      alert("There was a problem with the request.");
    }
  }
}
var controlbill;
function makerequest_flagbill(ctbill, billid, key) {
  http_request = false;
  if (window.XMLHttpRequest) {
    http_request = new XMLHttpRequest();
    if (http_request.overrideMimeType) {
      http_request.overrideMimeType("text/html");
    }
  } else if (window.ActiveXObject) {
    try {
      http_request = new ActiveXObject("Msxml2.XMLHTTP");
    } catch (e) {
      try {
        http_request = new ActiveXObject("Microsoft.XMLHTTP");
      } catch (e) {}
    }
  }
  if (!http_request) {
    alert("Cannot create XMLHTTP instance");
    return false;
  }
  var a = Math.floor(Math.random() * 1000);
  controlbill = ctbill;
  http_request.onreadystatechange = alertContents_flagbill;
  http_request.open(
    "GET",
    "ajax_flagbill.php?key=" +
      key +
      "&billid=" +
      billid +
      "&flag=" +
      ctbill.alt +
      "&a=" +
      a,
    true
  );
  http_request.send(null);
}
function alertContents_flagbill() {
  if (http_request.readyState == 4) {
    if (http_request.status == 200) {
      result = http_request.responseText;
      controlbill.alt = result;
      controlbill.src = "images/flag" + result + ".gif";
    } else {
      alert("There was a problem with the request.");
    }
  }
}
function load_tech3(url, maphong, maloai, key) {
  if (maphong == "-1" || maloai == "-1") {
    document.getElementById("div_tech").innerHTML =
      '<select class="box_input" name="subject" ><option value="0">--Chọn chủ đề cần hỗ trợ--</option></select>';
    return;
  }
  http_request = false;
  if (window.XMLHttpRequest) {
    http_request = new XMLHttpRequest();
    if (http_request.overrideMimeType) {
      http_request.overrideMimeType("text/html");
    }
  } else if (window.ActiveXObject) {
    try {
      http_request = new ActiveXObject("Msxml2.XMLHTTP");
    } catch (e) {
      try {
        http_request = new ActiveXObject("Microsoft.XMLHTTP");
      } catch (e) {}
    }
  }
  if (!http_request) {
    alert("Cannot create XMLHTTP instance");
    return false;
  }
  http_request.onreadystatechange = alertContents_Tech3;
  http_request.open(
    "GET",
    url + "?key=" + key + "&maphong=" + maphong + "&maloai=" + maloai,
    true
  );
  http_request.send(null);
}
function alertContents_Tech3() {
  if (http_request.readyState == 4) {
    if (http_request.status == 200) {
      result = http_request.responseText;
      document.getElementById("div_tech").innerHTML = result;
    } else {
      alert("There was a problem with the request.");
    }
  }
}
function load_tech2(url, maphong, maloai, key) {
  if (maphong == "-1" || maloai == "-1") {
    document.getElementById("div_tech").innerHTML =
      '<select class="box_input" name="subject" ><option value="0">--Chọn chủ đề cần hỗ trợ--</option></select>';
    return;
  }
  http_request = false;
  if (window.XMLHttpRequest) {
    http_request = new XMLHttpRequest();
    if (http_request.overrideMimeType) {
      http_request.overrideMimeType("text/html");
    }
  } else if (window.ActiveXObject) {
    try {
      http_request = new ActiveXObject("Msxml2.XMLHTTP");
    } catch (e) {
      try {
        http_request = new ActiveXObject("Microsoft.XMLHTTP");
      } catch (e) {}
    }
  }
  if (!http_request) {
    alert("Cannot create XMLHTTP instance");
    return false;
  }
  http_request.onreadystatechange = alertContents_Tech2;
  http_request.open(
    "GET",
    url + "?key=" + key + "&maphong=" + maphong + "&maloai=" + maloai,
    true
  );
  http_request.send(null);
}
function alertContents_Tech2() {
  if (http_request.readyState == 4) {
    if (http_request.status == 200) {
      result = http_request.responseText;
      document.getElementById("div_tech").innerHTML = result;
      load_service(
        "ajax_service.php",
        document.frmAddMessage.techkind.value,
        document.frmAddMessage.makh.value,
        document.frmAddMessage.sky_group.value
      );
    } else {
      alert("There was a problem with the request.");
    }
  }
}
function load_tech(url, maphong, maloai, key) {
  if (maphong == "-1" || maloai == "-1") {
    document.getElementById("div_tech").innerHTML =
      '<select class="box_input" name="subject" ><option value="0">--Chọn chủ đề cần hỗ trợ--</option></select>';
    return;
  }
  http_request = false;
  if (window.XMLHttpRequest) {
    http_request = new XMLHttpRequest();
    if (http_request.overrideMimeType) {
      http_request.overrideMimeType("text/html");
    }
  } else if (window.ActiveXObject) {
    try {
      http_request = new ActiveXObject("Msxml2.XMLHTTP");
    } catch (e) {
      try {
        http_request = new ActiveXObject("Microsoft.XMLHTTP");
      } catch (e) {}
    }
  }
  if (!http_request) {
    alert("Cannot create XMLHTTP instance");
    return false;
  }
  http_request.onreadystatechange = alertContents_Tech;
  http_request.open(
    "GET",
    url + "?key=" + key + "&maphong=" + maphong + "&maloai=" + maloai,
    true
  );
  http_request.send(null);
}
function alertContents_Tech() {
  if (http_request.readyState == 4) {
    if (http_request.status == 200) {
      result = http_request.responseText;
      document.getElementById("div_tech").innerHTML = result;
      load_service(
        "ajax_service.php",
        document.frmAddMessage.techkind.value,
        document.frmAddMessage.sky_id.value,
        document.frmAddMessage.sky_group.value
      );
    } else {
      alert("There was a problem with the request.");
    }
  }
}
function load_service(url, maloai, manguoidung, manhom) {
  if (maloai == "-1") {
    document.getElementById("div_service").innerHTML =
      '<select class="box_input" name="chonluadichvu" ><option value="">--Lựa chọn dịch vụ--</option></select>';
    return;
  }
  http_request = false;
  if (window.XMLHttpRequest) {
    http_request = new XMLHttpRequest();
    if (http_request.overrideMimeType) {
      http_request.overrideMimeType("text/html");
    }
  } else if (window.ActiveXObject) {
    try {
      http_request = new ActiveXObject("Msxml2.XMLHTTP");
    } catch (e) {
      try {
        http_request = new ActiveXObject("Microsoft.XMLHTTP");
      } catch (e) {}
    }
  }
  if (!http_request) {
    alert("Cannot create XMLHTTP instance");
    return false;
  }
  http_request.onreadystatechange = alertContents_service;
  http_request.open(
    "GET",
    url +
      "?maloai=" +
      maloai +
      "&manguoidung=" +
      manguoidung +
      "&manhom=" +
      manhom,
    true
  );
  http_request.send(null);
}
function alertContents_service() {
  if (http_request.readyState == 4) {
    if (http_request.status == 200) {
      result = http_request.responseText;
      document.getElementById("div_service").innerHTML = result;
      load_staff("ajax_staff.php", document.frmAddMessage.dept.value);
    } else {
      alert("There was a problem with the request.");
    }
  }
}
function load_staff(url, maphong) {
  if (document.frmAddMessage.nguoigiaiquyet == null) {
    return;
  }
  http_request = false;
  if (window.XMLHttpRequest) {
    http_request = new XMLHttpRequest();
    if (http_request.overrideMimeType) {
      http_request.overrideMimeType("text/html");
    }
  } else if (window.ActiveXObject) {
    try {
      http_request = new ActiveXObject("Msxml2.XMLHTTP");
    } catch (e) {
      try {
        http_request = new ActiveXObject("Microsoft.XMLHTTP");
      } catch (e) {}
    }
  }
  if (!http_request) {
    alert("Cannot create XMLHTTP instance");
    return false;
  }
  http_request.onreadystatechange = alertContents_staff;
  http_request.open("GET", url + "?maphong=" + maphong, true);
  http_request.send(null);
}
function alertContents_staff() {
  if (http_request.readyState == 4) {
    if (http_request.status == 200) {
      result = http_request.responseText;
      document.getElementById("div_staff").innerHTML = result;
    } else {
      alert("There was a problem with the request.");
    }
  }
}
var h1;
var h2;
var h3;
var h4;
var h5;
function vote_di() {
  document.getElementById("vote1").src = h1;
  document.getElementById("vote2").src = h2;
  document.getElementById("vote3").src = h3;
  document.getElementById("vote4").src = h4;
  document.getElementById("vote5").src = h5;
}
function vote_den(diem) {
  h1 = document.getElementById("vote1").src;
  h2 = document.getElementById("vote2").src;
  h3 = document.getElementById("vote3").src;
  h4 = document.getElementById("vote4").src;
  h5 = document.getElementById("vote5").src;
  if (diem == 1) {
    document.getElementById("vote1").src = "images2/star3.png";
  } else if (diem == 2) {
    document.getElementById("vote1").src = "images2/star3.png";
    document.getElementById("vote2").src = "images2/star3.png";
  } else if (diem == 3) {
    document.getElementById("vote1").src = "images2/star3.png";
    document.getElementById("vote2").src = "images2/star3.png";
    document.getElementById("vote3").src = "images2/star3.png";
  } else if (diem == 4) {
    document.getElementById("vote1").src = "images2/star3.png";
    document.getElementById("vote2").src = "images2/star3.png";
    document.getElementById("vote3").src = "images2/star3.png";
    document.getElementById("vote4").src = "images2/star3.png";
  } else if (diem == 5) {
    document.getElementById("vote1").src = "images2/star3.png";
    document.getElementById("vote2").src = "images2/star3.png";
    document.getElementById("vote3").src = "images2/star3.png";
    document.getElementById("vote4").src = "images2/star3.png";
    document.getElementById("vote5").src = "images2/star3.png";
  }
}
function JT_init() {
  $("a.jTip")
    .hover(
      function () {
        JT_show(this.href, this.id, this.name);
      },
      function () {
        $("#JT").remove();
      }
    )
    .click(function () {
      return false;
    });
}
function JT_show(url, linkId, title) {
  if (title == false) title = "&nbsp;";
  var de = document.documentElement;
  var w =
    self.innerWidth || (de && de.clientWidth) || document.body.clientWidth;
  var hasArea = w - getAbsoluteLeft(linkId);
  var clickElementy = getAbsoluteTop(linkId) - 3;
  var queryString = url.replace(/^[^\?]+\??/, "");
  var params = parseQuery(queryString);
  if (params["width"] === undefined) {
    params["width"] = 250;
  }
  if (params["link"] !== undefined) {
    $("#" + linkId).bind("click", function () {
      window.location = params["link"];
    });
    $("#" + linkId).css("cursor", "pointer");
  }
  if (hasArea > params["width"] * 1 + 75) {
    $("body").append(
      "<div id='JT' style='width:" +
        params["width"] * 1 +
        "px'><div id='JT_arrow_left'></div><div id='JT_close_left'>" +
        title +
        "</div><div id='JT_copy'><div class='JT_loader'><div></div></div>"
    );
    var arrowOffset = getElementWidth(linkId) + 11;
    var clickElementx = getAbsoluteLeft(linkId) + arrowOffset;
  } else {
    $("body").append(
      "<div id='JT' style='width:" +
        params["width"] * 1 +
        "px'><div id='JT_arrow_right' style='left:" +
        (params["width"] * 1 + 1) +
        "px'></div><div id='JT_close_right'>" +
        title +
        "</div><div id='JT_copy'><div class='JT_loader'><div></div></div>"
    );
    var clickElementx = getAbsoluteLeft(linkId) - (params["width"] * 1 + 15);
  }
  $("#JT").css({ left: clickElementx + "px", top: clickElementy + "px" });
  $("#JT").show();
  $("#JT_copy").load(url);
}
function getElementWidth(objectId) {
  x = document.getElementById(objectId);
  return x.offsetWidth;
}
function getAbsoluteLeft(objectId) {
  o = document.getElementById(objectId);
  oLeft = o.offsetLeft;
  while (o.offsetParent != null) {
    oParent = o.offsetParent;
    oLeft += oParent.offsetLeft;
    o = oParent;
  }
  return oLeft;
}
function getAbsoluteTop(objectId) {
  o = document.getElementById(objectId);
  oTop = o.offsetTop;
  while (o.offsetParent != null) {
    oParent = o.offsetParent;
    oTop += oParent.offsetTop;
    o = oParent;
  }
  return oTop;
}
function parseQuery(query) {
  var Params = new Object();
  if (!query) return Params;
  var Pairs = query.split(/[;&]/);
  for (var i = 0; i < Pairs.length; i++) {
    var KeyVal = Pairs[i].split("=");
    if (!KeyVal || KeyVal.length != 2) continue;
    var key = unescape(KeyVal[0]);
    var val = unescape(KeyVal[1]);
    val = val.replace(/\+/g, " ");
    Params[key] = val;
  }
  return Params;
}
function blockEvents(evt) {
  if (evt.target) {
    evt.preventDefault();
  } else {
    evt.returnValue = false;
  }
}
var mau_chon = "#FFFDD5";
var mau_ht;
function mouse_den(ct) {
  mau_ht = ct.bgColor;
  ct.bgColor = mau_chon;
}
function mouse_di(ct) {
  ct.bgColor = mau_ht;
}
var css_1 = "css_1";
var css_2 = "css_2";
var css_11 = "css_11";
var css_21 = "css_21";
var is_data_admin_manager = true;
jQuery(document).ready(function () {
  jQuery("div.right_menu div.title").mouseenter(function () {
    if (jQuery(this).next().is(":visible")) {
      jQuery(this)
        .addClass("css_11")
        .removeClass("css_21")
        .removeClass("css_1")
        .removeClass("css_2");
    } else {
      jQuery(this)
        .addClass("css_21")
        .removeClass("css_11")
        .removeClass("css_1")
        .removeClass("css_2");
    }
  });
  jQuery("div.right_menu div.title").mouseleave(function () {
    if (jQuery(this).next().is(":visible")) {
      jQuery(this)
        .addClass("css_1")
        .removeClass("css_2")
        .removeClass("css_11")
        .removeClass("css_21");
    } else {
      jQuery(this)
        .addClass("css_2")
        .removeClass("css_1")
        .removeClass("css_11")
        .removeClass("css_21");
    }
  });
  var mark_default = jQuery(".right_menu div.item").filter(function () {
    return jQuery(this);
  });
  mark_default.each(function () {
    var el = jQuery(this);
    el.hide();
  });
  var cookies_pa_id_item_value = getCookie("cookies_pa_id_item");
  if (cookies_pa_id_item_value != "") {
    var mark_default = jQuery("#" + cookies_pa_id_item_value)
      .parent()
      .find("div.item")
      .filter(function () {
        return jQuery(this);
      });
    mark_default.each(function () {
      var el = jQuery(this);
      el.prev("div.title:first")
        .addClass("css_1")
        .removeClass("css_2")
        .removeClass("css_11")
        .removeClass("css_21");
      el.show();
    });
  }
  jQuery("div.title", jQuery("div.section")).bind("click", function () {
    jQuery(".right_menu div.title")
      .addClass("css_2")
      .removeClass("css_1")
      .removeClass("css_11")
      .removeClass("css_21");
    if (jQuery(this).parent().find("div.item").is(":visible") == false) {
      var filter_menu_value = jQuery("#filter_menu").val().toLowerCase();
      if (filter_menu_value != "" && filter_menu_value != "tìm kiếm menu..") {
        jQuery(".right_menu div.item").hide();
        jQuery(".right_menu div.title").hide();
        var mark = jQuery(".right_menu div.item").filter(function () {
          var el = jQuery(this);
          var val = el.attr("lang").toLowerCase();
          return val.search(filter_menu_value) > -1;
        });
        mark.each(function () {
          var el = jQuery(this);
          el.show().parent().children(":first").show();
          f_visible = true;
        });
      } else {
        var mark_hide = jQuery(".right_menu div.item").filter(function () {
          return jQuery(this);
        });
        mark_hide.each(function () {
          var el = jQuery(this);
          el.hide();
        });
        var mark = jQuery(this)
          .parent()
          .find("div.item")
          .filter(function () {
            return jQuery(this);
          });
        mark.each(function () {
          var el = jQuery(this);
          el.show().parent().children(":first").show();
        });
      }
      jQuery(this)
        .addClass("css_1")
        .removeClass("css_2")
        .removeClass("css_11")
        .removeClass("css_21");
    } else {
      var mark = jQuery(this)
        .parent()
        .find("div.item")
        .filter(function () {
          return jQuery(this);
        });
      mark.each(function () {
        jQuery(this).hide();
      });
      jQuery(this)
        .addClass("css_2")
        .removeClass("css_1")
        .removeClass("css_11")
        .removeClass("css_21");
    }
  });
  var filter_menu_value = "";
  jQuery("#filter_menu").keyup(function (event) {
    jQuery("#admin_no_data").html("");
    filter_menu_value = jQuery(this).val().toLowerCase();
    if (filter_menu_value == "") {
      jQuery(".right_menu div").show();
      var mark_default = jQuery(".right_menu div.item").filter(function () {
        return jQuery(this);
      });
      mark_default.each(function () {
        var el = jQuery(this);
        el.hide();
      });
    } else {
      jQuery(".right_menu div.item").hide();
      jQuery(".right_menu div.title").hide();
      var mark = jQuery(".right_menu div.item").filter(function () {
        var el = jQuery(this);
        var val = el.attr("lang").toLowerCase();
        return val.search(filter_menu_value) > -1;
      });
      mark.each(function () {
        var el = jQuery(this);
        el.show().parent().children(":first").show();
        f_visible = true;
      });
    }
    var mark_nodata = jQuery(".right_menu div.item").filter(function () {
      if (jQuery(this).is(":visible") == false) {
        return jQuery(this);
      }
    });
  });
  jQuery(".item").bind("click", function () {
    setCookie("cookies_pa_id_item", jQuery(this).attr("id"), 1);
  });
  jQuery("#filter_menu").bind("focus", function () {
    if (jQuery(this).val() == "Tìm kiếm menu...") {
      jQuery(this).val("");
    }
  });
  jQuery("#filter_menu").bind("blur", function () {
    if (jQuery(this).val() == "") {
      jQuery(this).val("Tìm kiếm menu...");
    }
  });
  $(".change_status").bind("click", function () {
    var $IDaccount = $(this).attr("id");
    var $this = $(this);
    if (!confirm("Bạn xác nhận dịch vụ này đã được xử lý xong")) {
      return false;
    }
    $this.html("<img src='http://pavietnam.vn/images/loading.gif'/>");
    $.post(
      "ajax_getinfo.php",
      { action: "CHANGE_STATUS", IDaccount: $IDaccount },
      function ($data) {
        $this.html($data.msg);
      },
      "json"
    );
    return false;
  });
  $(".process_whois").bind("click", function () {
    if (!confirm("Bạn muốn thực hiện điều này ^^! ")) {
      return false;
    }
    var $reason = prompt("Nhập lý do :");
    $info = $(this).attr("rel").split("_");
    $IDaccount = $info[0];
    $action = $info[1];
    if ($action == "delete") {
      if (!confirm("Bạn muốn xóa dịch vụ này thật chứ >.<! ")) {
        return false;
      }
    }
    $("#error-" + $IDaccount).html(
      "<img src='http://pavietnam.vn/images/loading.gif'/>"
    );
    $.post(
      "ajax_getinfo.php",
      {
        action: "PROCESS_WHOIS",
        IDaccount: $IDaccount,
        type: $action,
        reason: $reason,
      },
      function ($data) {
        $("#error-" + $IDaccount).html($data);
      }
    );
  });
  $(".check_pa").bind("click", function () {
    var $IDaccount = $(this).attr("rel");
    var $this = $(this);
    $this.html(
      "<img style='width:16px;height:16px;border:1px solid #CCC; padding:1px;' src='images/loading1.gif'/> chờ chút xíu..."
    );
    $.post(
      "ajax_getinfo.php",
      { action: "CHECK_PA", IDaccount: $IDaccount },
      function ($data) {
        $this.html($data);
      }
    );
    return false;
  });
});
function getCookie(c_name) {
  if (document.cookie.length > 0) {
    c_start = document.cookie.indexOf(c_name + "=");
    if (c_start != -1) {
      c_start = c_start + c_name.length + 1;
      c_end = document.cookie.indexOf(";", c_start);
      if (c_end == -1) c_end = document.cookie.length;
      return unescape(document.cookie.substring(c_start, c_end));
    }
  }
  return "";
}
function setCookie(c_name, value, expiredays) {
  if (value != null && value.length <= 1930) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + expiredays);
    document.cookie =
      c_name +
      "=" +
      escape(value) +
      (expiredays == null ? "" : ";expires=" + exdate.toUTCString());
  }
}
function hidelayer(lay) {
  var $obj = jQuery(document.getElementById(lay));
  $obj.css({ opacity: "1.0" }).fadeOut(200, function () {
    $(this).css({ display: "none" });
  });
}
function showlayer(lay) {
  var $obj = jQuery(document.getElementById(lay));
  $obj.css({ opacity: "1.0" }).fadeIn(200, function () {
    $(this).css({ display: "" });
  });
}
function chon_option_select(dieukhien, giatri) {
  var n = dieukhien.options.length;
  for (i = 0; i < n; ++i) {
    if (dieukhien.options[i].value == giatri) {
      dieukhien.options[i].selected = true;
      break;
    }
  }
}
function trim($str) {
  return $str.replace(/^\s+/g, "").replace(/\s+$/g, "");
}
jQuery(function ($) {
  JT_init();
  $.fn.BindFocus = function ($option) {
    $optiondefault = { key: "link" };
    $options = $.extend($option, $optiondefault);
    return this.each(function () {
      $(this).val($(this).attr($options["key"]));
      $(this).bind("focus", function () {
        if ($(this).val() == $(this).attr($options["key"])) {
          $(this).val("");
        }
      });
      $(this).bind("blur", function () {
        if ($(this).val() == "") {
          $(this).val($(this).attr($options["key"]));
        }
      });
    });
  };
  $.fn.BindCheckBox = function ($options) {
    var $empty = {};
    var $optionsDefault = {
      CheckBoxAll: "input.chkall",
      CheckBoxOne: "input.chkone",
      CheckAll: "a.chkall",
      Container: document.body,
    };
    var $settings = $.extend(true, $optionsDefault, $options);
    $($settings.CheckBoxAll, $($settings.Container)).bind(
      "change",
      function (event) {
        event.stopPropagation();
        var $obj = $(this);
        $($settings.CheckBoxOne).attr("checked", $obj.attr("checked"));
      }
    );
    $($settings.CheckAll, $($settings.Container)).bind(
      "click",
      function (event) {
        event.stopPropagation();
        $($settings.CheckBoxAll, $($settings.Container)).trigger("change");
      }
    );
    $($settings.CheckBoxOne, $($settings.Container)).bind(
      "change",
      function (event) {
        event.stopPropagation();
        var $obj = $(this);
        var $ok = true;
        var $listcheck = $($settings.CheckBoxOne, $($settings.Container));
        $.each($listcheck, function () {
          if (!$(this).attr("checked")) {
            $ok = false;
            return false;
          }
        });
        $($settings.CheckBoxAll, $($settings.Container)).attr("checked", $ok);
      }
    );
  };
  $.fn.BindAnimation = function ($options) {
    var $empty = {};
    var $optionsDefault = {
      speed: 500,
      easing: "swing",
      animateenter: { opacity: 0.6 },
      animateleave: { opacity: 1.0 },
    };
    var $settings = $.extend(true, $empty, $optionsDefault, $options);
    return this.each(function () {
      var $this = $(this);
      $this.bind("mouseenter", function () {
        $(this).clearQueue();
        $(this).stop();
        $(this)
          .css($settings.animateleave)
          .animate(
            $settings.animateenter,
            $settings.speed,
            $settings.easing,
            function () {
              $this.css($settings.animateenter);
            }
          );
      });
      $this.bind("mouseleave", function () {
        $(this).clearQueue();
        $(this).stop();
        $(this)
          .css($settings.animateenter)
          .animate(
            $settings.animateleave,
            $settings.speed,
            $settings.easing,
            function () {
              $this.css($settings.animateleave);
            }
          );
      });
    });
  };
  $.fn.BindPagingSubmit = function ($options) {
    var $empty = {};
    var $optionsDefault = { FormID: "frm", HiddenID: "pageno", action: "" };
    var $settings = $.extend(true, $empty, $optionsDefault, $options);
    return this.each(function () {
      $(this).bind("click", function (event) {
        event.preventDefault();
        var $pageno = $(this).attr("rel");
        $pageno = $pageno.replace(/([^0-9])/g, "");
        if (isNaN($pageno) || $pageno.length <= 0 || $pageno < 0) {
          alert("Số trang không hợp lệ!");
          return false;
        }
        $("input#" + $settings.HiddenID).val($pageno);
        $("form#" + $settings.FormID).attr("action", $settings.action);
        $("form#" + $settings.FormID).submit();
      });
    });
  };
  $.fn.BindDNS = function ($options) {
    var $empty = {};
    var $optionsDefault = {
      src_1: "/images/command/control.jpg",
      src_2: "/images/loading1.gif",
    };
    var $settings = $.extend(true, $empty, $optionsDefault, $options);
    return this.each(function () {
      $(this).one("click", function (event) {
        event.preventDefault();
        var $obj = $(this);
        var $href = $obj.attr("href");
        var $idaccount = $obj.attr("rel");
        $idaccount = $idaccount.replace(/([^0-9]+)/g, "");
        if ($idaccount.length <= 0) {
          alert("Dịch vụ không tìm thấy!");
          return false;
        }
        $obj
          .children("img")
          .attr("src", $settings.src_2)
          .attr("disabled", "disabled");
        $.post(
          "assign_session.php",
          { idaccount: $idaccount },
          function (data, status) {
            if (status == "success" && data.msg == "Success") {
              window.location = $href;
            } else {
              alert("Dịch vụ không tìm thấy!");
            }
            $obj
              .children("img")
              .attr("src", $settings.src_1)
              .removeAttr("disabled");
          },
          "json"
        );
      });
    });
  };
  $.fn.BindDatePicker = function ($options) {
    var $src = "/images/calc.jpg";
    var $arrayDayName = ["CN", "Hai", "Ba", "Tư", "Năm", "Sáu", "Bảy"];
    var $arrayMonthName = [
      "Tháng 1",
      "Tháng 2",
      "Tháng 3",
      "Tháng 4",
      "Tháng 5",
      "Tháng 6",
      "Tháng 7",
      "Tháng 8",
      "Tháng 9",
      "Tháng 10",
      "Tháng 11",
      "Tháng 12",
    ];
    var $img = $(
      '<img src="' +
        $src +
        '" width="10" height="10" style="margin-left:-15px;margin-right:5px" border="0" alt=""/>'
    );
    var $empty = {};
    var $optionsDefault = {
      changeMonth: true,
      changeYear: true,
      dateFormat: "dd-mm-yy",
      showAnim: "slideDown",
      monthNamesShort: $arrayMonthName,
      dayNamesMin: $arrayDayName,
      Icon: false,
      minDate: "-30Y",
      maxDate: "+50Y",
    };
    var $settings = $.extend(true, $empty, $optionsDefault, $options);
    return this.each(function () {
      var $this = $(this);
      $this.datepicker({
        changeMonth: $settings.changeMonth,
        changeYear: $settings.changeYear,
        dateFormat: $settings.dateFormat,
        showAnim: $settings.showAnim,
        monthNamesShort: $settings.monthNamesShort,
        dayNamesMin: $settings.dayNamesMin,
        minDate: $settings.minDate,
        maxDate: $settings.maxDate,
      });
      if ($settings.Icon) {
        $imgclone = $img.clone().css({ cursor: "pointer" });
        $imgclone.insertAfter($(this));
        $imgclone.bind("click", function (event) {
          event.stopPropagation();
          $this.trigger("focus");
        });
      }
    });
  };
});
function Check_One(formName) {
  try {
    var $form = eval("document." + formName);
    var len = $form.chk.length;
  } catch (e) {
    return false;
  }
  var ok = true;
  if (len > 1) {
    for (i = 0; i < len; ++i) {
      if (!$form.chk[i].checked && $form.chk[i].value.length > 0) {
        ok = false;
        break;
      }
    }
  } else {
    if (!$form.chk.checked) ok = false;
  }
  $form.chkall.checked = ok;
  Get_StringID();
}
function Check_All(status, _from, formName) {
  try {
    var $form = eval("document." + formName);
    var len = $form.chk.length;
  } catch (e) {
    return false;
  }
  if (len > 1) {
    for (i = 0; i < len; ++i) {
      $form.chk[i].checked = status;
    }
  } else {
    $form.chk.checked = status;
  }
  if (_from > 1) $form.chkall.checked = status;
  Get_StringID();
}
function Check_Chon(formName) {
  var $form = eval("document." + formName);
  var arrayid = $form.hidden_id;
  arrayid.value = trim(arrayid.value);
  if (arrayid.value.length <= 0) {
    alert("Chưa chọn mẫu tin nào muốn xóa!");
    return false;
  } else {
    return confirm("Xác nhận xóa các mục bạn đã chọn!");
  }
  return false;
}
function Get_StringID() {
  var strID = "";
  try {
    var $form = eval("document." + formName);
    var len = $form.chk.length;
  } catch (e) {}
  if (len > 1) {
    for (i = 0; i < len; ++i) {
      if ($form.chk[i].checked) strID += $form.chk[i].value + ",";
    }
  } else {
    if ($form.chk.checked) strID = $form.chk.value + ",";
  }
  $form.hidden_id.value = strID;
}
function RemoveUnicode(text) {
  return removeUnicode(text);
}
function removeUnicode(text) {
  var $unicode = [
    { key: "a", value: "á|à|ả|ã|ạ|ă|ắ|ặ|ằ|ẳ|ẵ|â|ấ|ầ|ẩ|ẫ|ậ" },
    { key: "d", value: "đ" },
    { key: "e", value: "é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ" },
    { key: "i", value: "í|ì|ỉ|ĩ|ị" },
    { key: "o", value: "ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ" },
    { key: "u", value: "ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự" },
    { key: "y", value: "ý|ỳ|ỷ|ỹ|ỵ" },
    { key: "A", value: "Á|À|Ả|Ã|Ạ|Ă|Ắ|Ặ|Ằ|Ẳ|Ẵ|Â|Ấ|Ầ|Ẩ|Ẫ|Ậ" },
    { key: "D", value: "Đ" },
    { key: "E", value: "É|È|Ẻ|Ẽ|Ẹ|Ê|Ế|Ề|Ể|Ễ|Ệ" },
    { key: "I", value: "Í|Ì|Ỉ|Ĩ|Ị" },
    { key: "O", value: "Ó|Ò|Ỏ|Õ|Ọ|Ô|Ố|Ồ|Ổ|Ỗ|Ộ|Ơ|Ớ|Ờ|Ở|Ỡ|Ợ" },
    { key: "U", value: "Ú|Ù|Ủ|Ũ|Ụ|Ư|Ứ|Ừ|Ử|Ữ|Ự" },
    { key: "Y", value: "Ý|Ỳ|Ỷ|Ỹ|Ỵ" },
  ];
  for ($vars in $unicode) {
    text = text.replace(
      new RegExp($unicode[$vars].value, "g"),
      $unicode[$vars].key
    );
  }
  return text;
}
var $ctrldown = false;
jQuery(function ($) {
  $(window).keydown(function (event) {
    if (event.keyCode == 17) {
      $ctrldown = true;
    }
  });
});
function inputNumber(e) {
  var keynum;
  if (window.event) {
    keynum = e.keyCode;
  } else if (e.which) {
    keynum = e.which;
  }
  if (
    (keynum > 45 && keynum < 58) ||
    (keynum > 95 && keynum < 106) ||
    keynum == 8 ||
    ($ctrldown && (keynum == 86 || keynum == 67)) ||
    keynum == 9 ||
    keynum == 190 ||
    keynum == 39 ||
    keynum == 37 ||
    keynum == 38 ||
    keynum == 40 ||
    keynum == 189
  )
    return true;
  else return false;
}
function formatInt(ctrl) {
  var $start = $(ctrl).caret().start;
  var $end = $(ctrl).caret().end;
  var separator = ",";
  var int = ctrl.value.replace(new RegExp(separator, "g"), "");
  var regexp = new RegExp("\\B(\\d{3})(" + separator + "|$)");
  do {
    int = int.replace(regexp, separator + "$1");
  } while (int.search(regexp) >= 0);
  if (regexp.test(ctrl.value)) {
    $start = $start + 1;
    $end = $end + 1;
  }
  ctrl.value = int;
  $(ctrl).caret($start, $end);
}
function danhgia_di() {
  document.getElementById("vote1").src = h1;
  document.getElementById("vote2").src = h2;
}
function danhgia_den(diem) {
  if (diem == 1) {
    document.getElementById("vote1").src = "images/like.png";
    document.getElementById("vote1").setAttribute("title", "Hài lòng");
  } else if (diem == 2) {
    document.getElementById("vote1").src = "images/unlike.png";
    document.getElementById("vote1").setAttribute("title", "Không hài lòng");
  }
}
function makerequest_danhgia(macauhoi, loai, uid, nguoigiaiquyet) {
  http_request = false;
  if (window.XMLHttpRequest) {
    http_request = new XMLHttpRequest();
    if (http_request.overrideMimeType) {
      http_request.overrideMimeType("text/html");
    }
  } else if (window.ActiveXObject) {
    try {
      http_request = new ActiveXObject("Msxml2.XMLHTTP");
    } catch (e) {
      try {
        http_request = new ActiveXObject("Microsoft.XMLHTTP");
      } catch (e) {}
    }
  }
  if (!http_request) {
    alert("Cannot create XMLHTTP instance");
    return false;
  }
  var a = Math.floor(Math.random() * 1000);
  http_request.onreadystatechange = alertContents_danhgia;
  http_request.open(
    "GET",
    "ajax_danhgiahailong.php?macauhoi=" +
      macauhoi +
      "&loai=" +
      loai +
      "&uid=" +
      uid +
      "&nguoigiaiquyet=" +
      nguoigiaiquyet +
      "&a=" +
      a,
    true
  );
  http_request.send(null);
}
function alertContents_danhgia() {
  if (http_request.readyState == 4) {
    if (http_request.status == 200) {
      result = http_request.responseText;
      document.getElementById("div_vote").innerHTML = result;
      alert(
        "Việc đánh giá của quý khách đã được chúng tôi ghi nhận, xin cảm ơn."
      );
    } else {
      alert("There was a problem with the request.");
    }
  }
}
function slideSwitch() {
  var $active = $("#slideshow a.active");
  if ($active.length == 0) $active = $("#slideshow a:last");
  var $next = $active.next().length ? $active.next() : $("#slideshow a:first");
  $active.fadeOut(0).removeClass("active last-active");
  $next
    .addClass("active")
    .css({ opacity: 1.0 })
    .fadeOut(0)
    .fadeIn(500, function () {
      $(this).delay(100).animate({ opacity: 1.0 }, 1);
    });
  $("div.controls>a").removeClass("selected");
  $("div.controls>a[rel=" + $next.attr("rel") + "]").addClass("selected");
}
function open_box(url, cache, callback) {
  if (cache == null) {
    $("#alert-tmp").remove();
    cache = "";
  } else {
    cache = "-" + cache;
    if ($("#alert-tmp" + cache)[0]) {
      showBox("alert-tmp" + cache);
      return false;
    }
  }
  $.get(url, function (dat) {
    $(document.body).prepend(
      '<div class="pop" id="alert-tmp' + cache + '">' + dat + "</div>"
    );
    ab = document.getElementById("alert-tmp" + cache);
    $("button[type=reset]:first", ab).click(function () {
      closeBox();
    });
    if ($.isFunction(callback)) {
      callback();
    }
    showBox("alert-tmp" + cache);
  });
  return false;
}
$.fn.open_box = function (cache, callback) {
  open_box($(this).attr("href"), cache, callback);
  return false;
};
$.fn.alert_box = function (cache, callback) {
  if (cache == null) {
    $("#alert-tmp").remove();
    cache = "";
  } else {
    cache = "-" + cache;
    if ($("#alert-tmp" + cache)[0]) {
      showBox("alert-tmp" + cache);
      return false;
    }
  }
  $.get($(this).attr("href"), function (dat) {
    $(document.body).prepend(
      '<div class="pop" id="alert-tmp' +
        cache +
        '"><div class="mbo"><h3 class="hd">THÔNG BÁO</h3><form><div class="c1" style="color:#222;">' +
        dat +
        '</div><div class="clear"></div><div class="b"><button type="reset">Đóng</button></form></div>'
    );
    $("#alert-tmp" + cache + " button[type=reset]").click(function () {
      closeBox();
    });
    if ($.isFunction(callback)) {
      callback();
    }
    showBox("alert-tmp" + cache);
  });
  return false;
};
$.fn.BindToolTip = function ($options) {
  var $empty = {};
  var $optionsDefault = {
    container: "container",
    MouseOut: false,
    left: 5,
    top: 0,
    EventIn: "mouseenter",
    EventOut: "mouseleave",
    SpeedOut: 100,
    SpeedIn: 300,
    Focus: false,
  };
  var $settings = jQuery.extend(true, $empty, $optionsDefault, $options);
  $settings.left = parseInt($settings.left);
  $settings.top = parseInt($settings.top);
  if (isNaN($settings.left)) {
    $settings.left = 0;
  }
  if (isNaN($settings.top)) {
    $settings.top = 0;
  }
  return this.each(function () {
    $(this).bind($settings.EventIn, function (event) {
      $($settings.container).clearQueue();
      $($settings.container).stop();
      $($settings.container).css({
        opacity: 1.0,
        cursor: "default",
        display: "none",
      });
      if ($(event.target).hasClass("skip")) return true;
      if (
        $(event.target).hasClass("ignore") ||
        event.target.nodeName == "INPUT" ||
        event.target.nodeName == "SELECT" ||
        event.target.nodeName == "TEXTAREA"
      )
        return false;
      event.preventDefault();
      event.stopPropagation();
      $(this).css({ position: "relative" });
      $($settings.container).appendTo($(this));
      if ($($settings.container).is(":visible")) return false;
      var $wwin = $(window).width();
      var $wcon = $($settings.container).width();
      var $pos = $(this).offset();
      var $xleft = $settings.left + $(this).width();
      if ($pos.left + $(this).width() + $wcon + $settings.left + 50 >= $wwin) {
        $xleft =
          $wwin - $wcon - $pos.left - $(this).width() - $settings.left - 5;
      }
      var $xtop = $settings.top + $(this).height();
      $($settings.container).css({ left: $xleft + "px", top: $xtop + "px" });
      $($settings.container).fadeIn($settings.SpeedIn, function () {
        $(this).css({ display: "block" });
      });
      if ($settings.Focus != false) {
        $($settings.Focus, $($settings.container)).focus();
      }
    });
    if ($settings.MouseOut == true) {
      $(this).bind("mouseleave", function (event) {
        event.preventDefault();
        event.stopPropagation();
        $($settings.container).clearQueue();
        $($settings.container).stop();
        $($settings.container)
          .delay($settings.SpeedOut)
          .fadeOut(100, function () {
            $(this).css({ display: "none" });
          });
      });
    } else if ($settings.EventOut != false) {
      $($settings.container).bind($settings.EventOut, function (event) {
        event.preventDefault();
        event.stopPropagation();
        $(this).clearQueue();
        $(this).stop();
        $(this)
          .delay($settings.SpeedOut)
          .fadeOut(100, function () {
            $(this).css({ display: "none" });
          });
      });
    }
    $($settings.container).bind($settings.EventIn, function (event) {
      event.preventDefault();
      event.stopPropagation();
    });
    $($settings.container).bind("mouseenter", function (event) {
      event.preventDefault();
      event.stopPropagation();
      $(this).clearQueue();
      $(this).stop();
      $(this).css({ display: "block", opacity: 1.0 });
    });
  });
};
$.fn.BindToolTipAbsolute = function ($options) {
  var $empty = {};
  var $optionsDefault = {
    container: "container",
    MouseOut: false,
    left: 5,
    top: 0,
    EventIn: "mouseenter",
    EventOut: "mouseleave",
    SpeedOut: 100,
    SpeedIn: 300,
    Focus: false,
  };
  var $settings = jQuery.extend(true, $empty, $optionsDefault, $options);
  $settings.left = parseInt($settings.left);
  $settings.top = parseInt($settings.top);
  if (isNaN($settings.left)) {
    $settings.left = 0;
  }
  if (isNaN($settings.top)) {
    $settings.top = 0;
  }
  $wwin = $(window).width();
  $hwin = $(window).height();
  return this.each(function () {
    $(this).bind($settings.EventIn, function (event) {
      event.stopPropagation();
      if ($(event.target).parent($settings.container).is(":visible"))
        return true;
      event.preventDefault();
      if (
        event.target.nodeName == "INPUT" ||
        event.target.nodeName == "SELECT" ||
        event.target.nodeName == "TEXTAREA"
      )
        return false;
      $($settings.container).appendTo($(this));
      $(this).css({ position: "relative" });
      var $position = $(this).offset();
      var $scrolltop = $(window).scrollTop();
      var $scrollleft = $(window).scrollLeft();
      var $wcontainer = $($settings.container).outerWidth();
      var $hcontainer = $($settings.container).outerHeight();
      if ($($settings.container).is(":visible")) return false;
      var $xleft =
        $settings.left + $(this).width() + $position.left - $scrollleft;
      var $xtop = $settings.top + $position.top - $scrolltop - $hcontainer;
      if ($xleft + $wcontainer >= $wwin) {
        $xleft = $xleft + $wwin - ($xleft + $wcontainer) - 15;
      } else if ($xleft < 1) {
        $xleft = 0;
      }
      if ($xtop + $hcontainer >= $hwin) {
        $xtop = $xtop - $hcontainer;
      } else if ($xtop < 1) {
        $xtop = 0;
      }
      $($settings.container).css({
        cursor: "default",
        display: "none",
        left: $xleft + "px",
        top: $xtop + "px",
      });
      $($settings.container).clearQueue();
      $($settings.container).stop();
      $($settings.container).fadeIn($settings.SpeedIn, function () {
        $(this).css({ display: "block" });
      });
      if ($settings.Focus != false) {
        $($settings.Focus, $($settings.container)).focus();
      }
    });
    if ($settings.MouseOut == true) {
      $(this).bind("mouseleave", function (event) {
        event.preventDefault();
        event.stopPropagation();
        $($settings.container).clearQueue();
        $($settings.container).stop();
        $($settings.container)
          .css({ opacity: 1.0 })
          .delay($settings.SpeedOut)
          .fadeOut(100, function () {
            $(this).css({ display: "none" });
          });
      });
    } else if ($settings.EventOut != false) {
      $($settings.container).bind($settings.EventOut, function () {
        event.preventDefault();
        event.stopPropagation();
        $(this).clearQueue();
        $(this).stop();
        $(this)
          .css({ opacity: 1.0 })
          .delay($settings.SpeedOut)
          .fadeOut(100, function () {
            $(this).css({ display: "none" });
          });
      });
    }
    $($settings.container).bind("mouseenter", function (event) {
      event.preventDefault();
      event.stopPropagation();
      $(this).clearQueue();
      $(this).stop();
      $(this).css({ display: "block", opacity: 1.0 });
    });
  });
};
function open_post_box(url, params, cache) {
  if (cache == null) {
    cache = "";
  } else {
    if ($("#alert-tmp-" + cache)[0]) {
      showBox("alert-tmp-" + cache);
      return false;
    }
  }
  $.post(url, params, function (dat) {
    $("#alert-tmp").remove();
    $(document.body).prepend(
      '<div class="pop" id="alert-tmp-' + cache + '">' + dat + "</div>"
    );
    showBox("alert-tmp-" + cache);
  });
  return false;
}
$.fn.post_link = function (callback) {
  $.post($(this).attr("href"), function (dat) {
    if (callback == null) {
      alert(dat);
    } else {
      callback(dat);
    }
  });
  return false;
};
var openBox;
function showCover() {
  cov = $("#cover");
  if (!cov[0]) {
    $(document.body).prepend('<div id="cover" onclick="closeBox()"></div>');
    cov = $("#cover");
  }
  var height = 0;
  if ($(document).height() < $(window).height()) {
    height = $(window).height();
  } else {
    height = $(document).height();
  }
  cov.height(height);
  cov.show();
}
function hideCover() {
  $("#cover").hide();
}
function isCover() {
  return $("#cover").css("display") != "none";
}
function centerBox(id) {
  var scrOfX = 0,
    scrOfY = 0;
  width = document.documentElement.clientWidth;
  height = document.documentElement.clientHeight;
  if (typeof window.pageYOffset == "number") {
    scrOfY = window.pageYOffset;
    scrOfX = window.pageXOffset;
  } else if (
    document.body &&
    (document.body.scrollLeft || document.body.scrollTop)
  ) {
    scrOfY = document.body.scrollTop;
    scrOfX = document.body.scrollLeft;
    width = document.body.clientWidth;
    height = document.body.clientHeight;
  } else if (
    document.documentElement &&
    (document.documentElement.scrollLeft || document.documentElement.scrollTop)
  ) {
    scrOfY = document.documentElement.scrollTop;
    scrOfX = document.documentElement.scrollLeft;
  }
  var Left = Math.round(width / 2 - $("#" + id).width() / 2);
  var Top = Math.round(height / 2 - $("#" + id).height() / 2);
  if ($("#" + id).css("position") == "absolute") {
    Left = Left + scrOfX;
    Top = Top + scrOfY;
  }
  $("#" + id).css("top", Top);
  $("#" + id).css("left", Left);
}
function showBox(id, speed) {
  if (speed == null) {
    speed = "slow";
  }
  showCover();
  openBox = id;
  centerBox(id);
  $("#" + id).show();
  $("body").keyup(function (e) {
    if (e.keyCode == "27") {
      closeBox();
    }
  });
  return;
}
function closeBox(speed) {
  if (speed == null) {
    speed = "slow";
  }
  $("#" + openBox).hide();
  $("#cover").hide();
  $("body").keyup(null);
}
$.fn.link = function (callback) {
  $.get($(this).attr("href"), { ajax: 1 }, function (dat) {
    if (callback == null) {
      alert(dat);
    } else {
      callback(dat);
    }
  });
  return false;
};
$.fn.submit_form = function (_callback, _before) {
  var ele = this;
  dat = $(ele).serialize() + "&ajax=1";
  if (_before != null && $.isFunction(_before)) {
    _before(ele);
  }
  $.post($(this).attr("action"), dat, function (dat) {
    if (_callback == null) {
      closeBox();
    } else if ($.isFunction(_callback)) {
      _callback(dat, ele);
    }
  });
  return false;
};
function submit_callback(dat) {
  try {
    var obj = $.parseJSON(dat);
  } catch (ex) {
    alert(dat);
  }
}
jQuery(function () {
  $("#host th:first").attr("style", "width:70px");
  $("#server th:first").attr("style", "width:70px");
  $("table tr:odd").addClass("light");
  $("#cbxHost").click(function () {
    var status = $(this).attr("checked");
    if (status === true) {
      $("#host input").attr("checked", "checked");
    } else {
      $(this).attr("checked");
      $("#host input").attr("checked", "");
    }
  });
  $("#cbxServer").click(function () {
    var status = $(this).attr("checked");
    if (status === true) {
      $("#server input").attr("checked", "checked");
    } else {
      $(this).attr("checked");
      $("#server input").attr("checked", "");
    }
  });
  $(".view_reason").hover(function () {
    var w = $(this).width();
    $(this).find(".reason").css("left", w).show();
  });
  $(".view_reason").mouseleave(function () {
    $(this).find(".reason").hide();
  });
  $("span.viewtip_danhgia").each(function () {
    $(this).BindToolTip({
      container: "div.viewinfo_danhgia" + $(this).attr("lang"),
      MouseOut: true,
      left: -8,
    });
  });
  $("span.viewtip_quydinh").each(function () {
    $(this).BindToolTip({ container: "div.viewinfo_quydinh", MouseOut: true });
  });
  $("span.viewtip_nhanvien").each(function () {
    $(this).BindToolTip({ container: "div.viewinfo_nhanvien", MouseOut: true });
  });
});
function convert_ascii(text) {
  var unicode = [
    { key: "a", value: "á|à|ả|ã|ạ|ă|ắ|ặ|ằ|ẳ|ẵ|â|ấ|ầ|ẩ|ẫ|ậ" },
    { key: "d", value: "đ" },
    { key: "e", value: "é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ" },
    { key: "i", value: "í|ì|ỉ|ĩ|ị" },
    { key: "o", value: "ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ" },
    { key: "u", value: "ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự" },
    { key: "y", value: "ý|ỳ|ỷ|ỹ|ỵ" },
    { key: "A", value: "Á|À|Ả|Ã|Ạ|Ă|Ắ|Ặ|Ằ|Ẳ|Ẵ|Â|Ấ|Ầ|Ẩ|Ẫ|Ậ" },
    { key: "D", value: "Đ" },
    { key: "E", value: "É|È|Ẻ|Ẽ|Ẹ|Ê|Ế|Ề|Ể|Ễ|Ệ" },
    { key: "I", value: "Í|Ì|Ỉ|Ĩ|Ị" },
    { key: "O", value: "Ó|Ò|Ỏ|Õ|Ọ|Ô|Ố|Ồ|Ổ|Ỗ|Ộ|Ơ|Ớ|Ờ|Ở|Ỡ|Ợ" },
    { key: "U", value: "Ú|Ù|Ủ|Ũ|Ụ|Ư|Ứ|Ừ|Ử|Ữ|Ự" },
    { key: "Y", value: "Ý|Ỳ|Ỷ|Ỹ|Ỵ" },
  ];
  for (vars in unicode) {
    text = text.replace(
      new RegExp(unicode[vars].value, "g"),
      unicode[vars].key
    );
  }
  return text;
}
