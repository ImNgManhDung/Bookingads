/**
 * Để sử dụng các hàm bên dưới, cần có thư viện jquery-ui, vui lòng import script jquery-ui trước khi import file này
 * @private
 */
(function ($) {
  $.ui.autocomplete.prototype._resizeMenu = function () {
    var ul = this.menu.element;
    ul.outerWidth(this.element.outerWidth() + 100);
  };

  /**
   * Hàm để lọc dữ liệu từ multi-select drop down list
   * Để sử dụng, tạo 1 input có class là quickSearch và có attribute data-target chứa đối tượng dropDownlist cần tương tác
   *
   * Ví dụ: <input class="quickSearch" data-target="#dropDown">
   */
  $.fn.quickSearch = function (target) {
    this.each(function () {
      $(this).keyup(function () {
        var valthis = $(this).val().toLowerCase();
        var num = 0;
        $("select" + target + ">option").each(function () {
          var text = $(this).text().toLowerCase();
          if (text.indexOf(valthis) !== -1) {
            $(this).show();
            //$(this).prop('selected',true);
          } else {
            $(this).hide();
          }
        });
      });
    });
  };

  /**
   * Hàm sử dụng để chuyển đổi khung select thành dạng combobox (cho phép gõ input và autocomplete)
   * Để sử dụng, dùng cú pháp
   *
   * jQuery("selector").combobox({
   *  selected: function()
   * })
   */

  $.widget("ui.combobox", {
    _create: function () {
      var self = this;
      var select = this.element.hide(),
        theWidth = select.width(),
        selected = select.children(":selected"),
        value = selected.val() ? selected.text() : "",
        options = $.extend({}, this.options, {
          minLength: 0,
          delay: 0,
          source: function (request, response) {
            if (select.children("option").length > 0) {
              var matcher = new RegExp(
                $.ui.autocomplete.escapeRegex(request.term),
                "i"
              );
              response(
                select.children("option").map(function () {
                  var text = $(this).text();
                  if (this.value && (!request.term || matcher.test(text))) {
                    return {
                      label: text.replace(
                        new RegExp(
                          "(?![^&;]+;)(?!<[^<>]*)(" +
                            $.ui.autocomplete.escapeRegex(request.term) +
                            ")(?![^<>]*>)(?![^&;]+;)",
                          "gi"
                        ),
                        "<strong>$1</strong>"
                      ),
                      value: text,
                      option: this,
                    };
                  }
                })
              );
            } else {
              if (!request.term.length) {
                if (typeof self.options.initialValues === "function") {
                  self.options.initialValues(request, response);
                } else {
                  response(self.options.initialValues);
                }
              } else {
                if (typeof self.options.source === "function") {
                  self.options.source(request, response);
                } else if (typeof self.options.source === "string") {
                  $.ajax({
                    url: self.options.source,
                    data: request,
                    dataType: "json",
                    success: function (data, status) {
                      response(data);
                    },
                    error: function () {
                      response([]);
                    },
                  });
                }
              }
            }
          },
          select: function (event, ui) {
            ui.item.option.selected = true;
            self._trigger("selected", event, {
              item: ui.item.option,
            });
          },
          change: function (event, ui) {
            if (!ui.item) {
              var matcher = new RegExp(
                  "^" + $.ui.autocomplete.escapeRegex($(this).val()) + "$",
                  "i"
                ),
                valid = false;
              select.children("option").each(function () {
                if (this.value.match(matcher)) {
                  this.selected = valid = true;
                  return false;
                }
              });
              if (!valid) {
                // remove invalid value, as it didn't match anything
                $(this).val("");
                select.val("");
                self._trigger("cleared");
                return false;
              }
            }
          },
          close: function (event, ui) {
            if (input.val() === "") {
              self._trigger("cleared");
            }
          },
        });

      if (this.options.width !== undefined) {
        theWidth = this.options.width;
      }

      var input = $('<input style="width:' + (theWidth - 30) + 'px;"/>')
        .insertAfter(select)
        .val(value)
        .autocomplete(options)
        .addClass("pa-autocomplete-input");

      input.data("autocomplete")._renderItem = function (ul, item) {
        ul.addClass("pa-autocomplete");
        return $("<li></li>")
          .data("item.autocomplete", item)
          .append("<a>" + item.label + "</a>")
          .appendTo(ul);
      };

      $(
        '<button type="button" style="position: absolute; height: 31px; border-radius: 0"> </button>'
      )
        .attr("tabIndex", -1)
        .attr("title", "Show All Items")
        .insertAfter(input)
        .button({
          icons: {
            primary: "ui-icon-triangle-1-s",
          },
          text: false,
        })
        .removeClass("ui-corner-all")
        .addClass("ui-corner-right ui-button-icon pa-autocomplete-button")
        .click(function () {
          // close if already visible
          if (input.autocomplete("widget").is(":visible")) {
            input.autocomplete("close");
            select.trigger("close");
            return;
          }
          // pass empty string as value to search for, displaying all results
          input.autocomplete("search", "");
          input.focus();
        });
    },
    _removeIfInvalid: function (event, ui) {
      if (ui.item) {
        return;
      }

      var value = this.input.val(),
        valueLowerCase = value.toLowerCase(),
        valid = false;
      this.element.children("option").each(function () {
        if ($(this).text().toLowerCase() === valueLowerCase) {
          this.selected = valid = true;
          return false;
        }
      });

      if (valid) {
        this._trigger("change", event, {
          item: null,
        });

        return;
      }

      this.input
        .val("")
        .attr("title", value + " didn't match any item")
        .tooltip("open");
      this.element.val("");
      this._delay(function () {
        this.input.tooltip("close").attr("title", "");
      }, 2500);
      this.input.data("ui-autocomplete").term = "";

      this._trigger("change", event, {
        item: null,
      });
    },
  });
})(jQuery);
