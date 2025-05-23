﻿/*! AdminLTE app.js
* ================
* Main JS application file for AdminLTE v2. This file
* should be included in all pages. It controls some layout
* options and implements exclusive AdminLTE plugins.
*
* @author Colorlib
* @support <https://github.com/ColorlibHQ/AdminLTE/issues>
* @version 2.4.17
* @repository git://github.com/ColorlibHQ/AdminLTE.git
* @license MIT <http://opensource.org/licenses/MIT>
*/
if ("undefined" == typeof jQuery)
    throw new Error("AdminLTE requires jQuery");
!function (i) {
    "use strict";
    function s(t, e) {
        if (this.element = t,
            this.options = e,
            this.$overlay = i(e.overlayTemplate),
            "" === e.source)
            throw new Error("Source url was not defined. Please specify a url in your BoxRefresh source option.");
        this._setUpListeners(),
            this.load()
    }
    var r = "lte.boxrefresh"
        , a = {
            source: "",
            params: {},
            trigger: ".refresh-btn",
            content: ".box-body",
            loadInContent: !0,
            responseType: "",
            overlayTemplate: '<div class="overlay"><div class="fa fa-refresh fa-spin"></div></div>',
            onLoadStart: function () { },
            onLoadDone: function (t) {
                return t
            }
        }
        , t = '[data-widget="box-refresh"]';
    function e(n) {
        return this.each(function () {
            var t = i(this)
                , e = t.data(r);
            if (!e) {
                var o = i.extend({}, a, t.data(), "object" == typeof n && n);
                t.data(r, e = new s(t, o))
            }
            if ("string" == typeof e) {
                if (void 0 === e[n])
                    throw new Error("No method named " + n);
                e[n]()
            }
        })
    }
    s.prototype.load = function () {
        this._addOverlay(),
            this.options.onLoadStart.call(i(this)),
            i.get(this.options.source, this.options.params, function (t) {
                this.options.loadInContent && i(this.element).find(this.options.content).html(t),
                    this.options.onLoadDone.call(i(this), t),
                    this._removeOverlay()
            }
                .bind(this), "" !== this.options.responseType && this.options.responseType)
    }
        ,
        s.prototype._setUpListeners = function () {
            i(this.element).on("click", this.options.trigger, function (t) {
                t && t.preventDefault(),
                    this.load()
            }
                .bind(this))
        }
        ,
        s.prototype._addOverlay = function () {
            i(this.element).append(this.$overlay)
        }
        ,
        s.prototype._removeOverlay = function () {
            i(this.$overlay).remove()
        }
        ;
    var o = i.fn.boxRefresh;
    i.fn.boxRefresh = e,
        i.fn.boxRefresh.Constructor = s,
        i.fn.boxRefresh.noConflict = function () {
            return i.fn.boxRefresh = o,
                this
        }
        ,
        i(window).on("load", function () {
            i(t).each(function () {
                e.call(i(this))
            })
        })
}(jQuery),
    function (i) {
        "use strict";
        function s(t, e) {
            this.element = t,
                this.options = e,
                this._setUpListeners()
        }
        var r = "lte.boxwidget"
            , a = {
                animationSpeed: 500,
                collapseTrigger: '[data-widget="collapse"]',
                removeTrigger: '[data-widget="remove"]',
                collapseIcon: "fa-minus",
                expandIcon: "fa-plus",
                removeIcon: "fa-times"
            }
            , t = ".box"
            , e = ".collapsed-box"
            , d = ".box-header"
            , l = ".box-body"
            , c = ".box-footer"
            , h = ".box-tools"
            , f = "collapsed-box"
            , p = "collapsing.boxwidget"
            , u = "collapsed.boxwidget"
            , g = "expanding.boxwidget"
            , v = "expanded.boxwidget"
            , o = "removing.boxwidget"
            , n = "removed.boxwidget";
        function b(n) {
            return this.each(function () {
                var t = i(this)
                    , e = t.data(r);
                if (!e) {
                    var o = i.extend({}, a, t.data(), "object" == typeof n && n);
                    t.data(r, e = new s(t, o))
                }
                if ("string" == typeof n) {
                    if (void 0 === e[n])
                        throw new Error("No method named " + n);
                    e[n]()
                }
            })
        }
        s.prototype.toggle = function () {
            !i(this.element).is(e) ? this.collapse() : this.expand()
        }
            ,
            s.prototype.expand = function () {
                var t = i.Event(v)
                    , e = i.Event(g)
                    , o = this.options.collapseIcon
                    , n = this.options.expandIcon;
                i(this.element).removeClass(f),
                    i(this.element).children(d + ", " + l + ", " + c).children(h).find("." + n).removeClass(n).addClass(o),
                    i(this.element).children(l + ", " + c).slideDown(this.options.animationSpeed, function () {
                        i(this.element).trigger(t)
                    }
                        .bind(this)).trigger(e)
            }
            ,
            s.prototype.collapse = function () {
                var t = i.Event(u)
                    , e = i.Event(p)
                    , o = this.options.collapseIcon
                    , n = this.options.expandIcon;
                i(this.element).children(d + ", " + l + ", " + c).children(h).find("." + o).removeClass(o).addClass(n),
                    i(this.element).children(l + ", " + c).slideUp(this.options.animationSpeed, function () {
                        i(this.element).addClass(f),
                            i(this.element).trigger(t)
                    }
                        .bind(this)).trigger(e)
            }
            ,
            s.prototype.remove = function () {
                var t = i.Event(n)
                    , e = i.Event(o);
                i(this.element).slideUp(this.options.animationSpeed, function () {
                    i(this.element).trigger(t),
                        i(this.element).remove()
                }
                    .bind(this)).trigger(e)
            }
            ,
            s.prototype._setUpListeners = function () {
                var e = this;
                i(this.element).on("click", this.options.collapseTrigger, function (t) {
                    return t && t.preventDefault(),
                        e.toggle(i(this)),
                        !1
                }),
                    i(this.element).on("click", this.options.removeTrigger, function (t) {
                        return t && t.preventDefault(),
                            e.remove(i(this)),
                            !1
                    })
            }
            ;
        var m = i.fn.boxWidget;
        i.fn.boxWidget = b,
            i.fn.boxWidget.Constructor = s,
            i.fn.boxWidget.noConflict = function () {
                return i.fn.boxWidget = m,
                    this
            }
            ,
            i(window).on("load", function () {
                i(t).each(function () {
                    b.call(i(this))
                })
            })
    }(jQuery),
    function (i) {
        "use strict";
        function s(t, e) {
            this.element = t,
                this.options = e,
                this.hasBindedResize = !1,
                this.init()
        }
        var r = "lte.controlsidebar"
            , a = {
                controlsidebarSlide: !0
            }
            , e = ".control-sidebar"
            , t = '[data-toggle="control-sidebar"]'
            , o = ".control-sidebar-open"
            , n = ".control-sidebar-bg"
            , d = ".wrapper"
            , l = ".layout-boxed"
            , c = "control-sidebar-open"
            , h = "control-sidebar-hold-transition"
            , f = "collapsed.controlsidebar"
            , p = "expanded.controlsidebar";
        function u(n) {
            return this.each(function () {
                var t = i(this)
                    , e = t.data(r);
                if (!e) {
                    var o = i.extend({}, a, t.data(), "object" == typeof n && n);
                    t.data(r, e = new s(t, o))
                }
                "string" == typeof n && e.toggle()
            })
        }
        s.prototype.init = function () {
            i(this.element).is(t) || i(this).on("click", this.toggle),
                this.fix(),
                i(window).resize(function () {
                    this.fix()
                }
                    .bind(this))
        }
            ,
            s.prototype.toggle = function (t) {
                t && t.preventDefault(),
                    this.fix(),
                    i(e).is(o) || i("body").is(o) ? this.collapse() : this.expand()
            }
            ,
            s.prototype.expand = function () {
                i(e).show(),
                    this.options.controlsidebarSlide ? i(e).addClass(c) : i("body").addClass(h).addClass(c).delay(50).queue(function () {
                        i("body").removeClass(h),
                            i(this).dequeue()
                    }),
                    i(this.element).trigger(i.Event(p))
            }
            ,
            s.prototype.collapse = function () {
                this.options.controlsidebarSlide ? i(e).removeClass(c) : i("body").addClass(h).removeClass(c).delay(50).queue(function () {
                    i("body").removeClass(h),
                        i(this).dequeue()
                }),
                    i(e).fadeOut(),
                    i(this.element).trigger(i.Event(f))
            }
            ,
            s.prototype.fix = function () {
                i("body").is(l) && this._fixForBoxed(i(n))
            }
            ,
            s.prototype._fixForBoxed = function (t) {
                t.css({
                    position: "absolute",
                    height: i(d).height()
                })
            }
            ;
        var g = i.fn.controlSidebar;
        i.fn.controlSidebar = u,
            i.fn.controlSidebar.Constructor = s,
            i.fn.controlSidebar.noConflict = function () {
                return i.fn.controlSidebar = g,
                    this
            }
            ,
            i(document).on("click", t, function (t) {
                t && t.preventDefault(),
                    u.call(i(this), "toggle")
            })
    }(jQuery),
    function (n) {
        "use strict";
        function i(t) {
            this.element = t
        }
        var s = "lte.directchat"
            , t = '[data-widget="chat-pane-toggle"]'
            , e = ".direct-chat"
            , o = "direct-chat-contacts-open";
        function r(o) {
            return this.each(function () {
                var t = n(this)
                    , e = t.data(s);
                e || t.data(s, e = new i(t)),
                    "string" == typeof o && e.toggle(t)
            })
        }
        i.prototype.toggle = function (t) {
            t.parents(e).first().toggleClass(o)
        }
            ;
        var a = n.fn.directChat;
        n.fn.directChat = r,
            n.fn.directChat.Constructor = i,
            n.fn.directChat.noConflict = function () {
                return n.fn.directChat = a,
                    this
            }
            ,
            n(document).on("click", t, function (t) {
                t && t.preventDefault(),
                    r.call(n(this), "toggle")
            })
    }(jQuery),
    function (i) {
        "use strict";
        function s(t) {
            this.options = t,
                this.init()
        }
        var r = "lte.pushmenu"
            , a = {
                collapseScreenSize: 767,
                expandOnHover: !1,
                expandTransitionDelay: 200
            }
            , t = ".sidebar-collapse"
            , e = ".main-sidebar"
            , o = ".content-wrapper"
            , n = ".sidebar-form .form-control"
            , d = '[data-toggle="push-menu"]'
            , l = ".sidebar-mini"
            , c = ".sidebar-expanded-on-hover"
            , h = ".fixed"
            , f = "sidebar-collapse"
            , p = "sidebar-open"
            , u = "sidebar-expanded-on-hover"
            , g = "sidebar-mini-expand-feature"
            , v = "expanded.pushMenu"
            , b = "collapsed.pushMenu";
        function m(n) {
            return this.each(function () {
                var t = i(this)
                    , e = t.data(r);
                if (!e) {
                    var o = i.extend({}, a, t.data(), "object" == typeof n && n);
                    t.data(r, e = new s(o))
                }
                "toggle" === n && e.toggle()
            })
        }
        s.prototype.init = function () {
            (this.options.expandOnHover || i("body").is(l + h)) && (this.expandOnHover(),
                i("body").addClass(g)),
                i(o).click(function () {
                    i(window).width() <= this.options.collapseScreenSize && i("body").hasClass(p) && this.close()
                }
                    .bind(this)),
                i(n).click(function (t) {
                    t.stopPropagation()
                })
        }
            ,
            s.prototype.toggle = function () {
                var t = i(window).width()
                    , e = !i("body").hasClass(f);
                t <= this.options.collapseScreenSize && (e = i("body").hasClass(p)),
                    e ? this.close() : this.open()
            }
            ,
            s.prototype.open = function () {
                i(window).width() > this.options.collapseScreenSize ? i("body").removeClass(f).trigger(i.Event(v)) : i("body").addClass(p).trigger(i.Event(v))
            }
            ,
            s.prototype.close = function () {
                i(window).width() > this.options.collapseScreenSize ? i("body").addClass(f).trigger(i.Event(b)) : i("body").removeClass(p + " " + f).trigger(i.Event(b))
            }
            ,
            s.prototype.expandOnHover = function () {
                i(e).hover(function () {
                    i("body").is(l + t) && i(window).width() > this.options.collapseScreenSize && this.expand()
                }
                    .bind(this), function () {
                        i("body").is(c) && this.collapse()
                    }
                        .bind(this))
            }
            ,
            s.prototype.expand = function () {
                setTimeout(function () {
                    i("body").removeClass(f).addClass(u)
                }, this.options.expandTransitionDelay)
            }
            ,
            s.prototype.collapse = function () {
                setTimeout(function () {
                    i("body").removeClass(u).addClass(f)
                }, this.options.expandTransitionDelay)
            }
            ;
        var y = i.fn.pushMenu;
        i.fn.pushMenu = m,
            i.fn.pushMenu.Constructor = s,
            i.fn.pushMenu.noConflict = function () {
                return i.fn.pushMenu = y,
                    this
            }
            ,
            i(document).on("click", d, function (t) {
                t.preventDefault(),
                    m.call(i(this), "toggle")
            }),
            i(window).on("load", function () {
                m.call(i(d))
            })
    }(jQuery),
    function (i) {
        "use strict";
        function s(t, e) {
            this.element = t,
                this.options = e,
                this._setUpListeners()
        }
        var r = "lte.todolist"
            , a = {
                onCheck: function (t) {
                    return t
                },
                onUnCheck: function (t) {
                    return t
                }
            }
            , e = {
                data: '[data-widget="todo-list"]'
            }
            , o = "done";
        function t(n) {
            return this.each(function () {
                var t = i(this)
                    , e = t.data(r);
                if (!e) {
                    var o = i.extend({}, a, t.data(), "object" == typeof n && n);
                    t.data(r, e = new s(t, o))
                }
                if ("string" == typeof e) {
                    if (void 0 === e[n])
                        throw new Error("No method named " + n);
                    e[n]()
                }
            })
        }
        s.prototype.toggle = function (t) {
            t.parents(e.li).first().toggleClass(o),
                t.prop("checked") ? this.check(t) : this.unCheck(t)
        }
            ,
            s.prototype.check = function (t) {
                this.options.onCheck.call(t)
            }
            ,
            s.prototype.unCheck = function (t) {
                this.options.onUnCheck.call(t)
            }
            ,
            s.prototype._setUpListeners = function () {
                var t = this;
                i(this.element).on("change ifChanged", "input:checkbox", function () {
                    t.toggle(i(this))
                })
            }
            ;
        var n = i.fn.todoList;
        i.fn.todoList = t,
            i.fn.todoList.Constructor = s,
            i.fn.todoList.noConflict = function () {
                return i.fn.todoList = n,
                    this
            }
            ,
            i(window).on("load", function () {
                i(e.data).each(function () {
                    t.call(i(this))
                })
            })
    }(jQuery),
    function (s) {
        "use strict";
        function n(t, e) {
            this.element = t,
                this.options = e,
                s(this.element).addClass(h),
                s(a + o, this.element).addClass(c),
                this._setUpListeners()
        }
        var i = "lte.tree"
            , r = {
                animationSpeed: 500,
                accordion: !0,
                followLink: !1,
                trigger: ".treeview a"
            }
            , a = ".treeview"
            , d = ".treeview-menu"
            , l = ".menu-open, .active"
            , t = '[data-widget="tree"]'
            , o = ".active"
            , c = "menu-open"
            , h = "tree"
            , f = "collapsed.tree"
            , p = "expanded.tree";
        function e(o) {
            return this.each(function () {
                var t = s(this);
                if (!t.data(i)) {
                    var e = s.extend({}, r, t.data(), "object" == typeof o && o);
                    t.data(i, new n(t, e))
                }
            })
        }
        n.prototype.toggle = function (t, e) {
            var o = t.next(d)
                , n = t.parent()
                , i = n.hasClass(c);
            n.is(a) && (this.options.followLink && "#" !== t.attr("href") || e.preventDefault(),
                i ? this.collapse(o, n) : this.expand(o, n))
        }
            ,
            n.prototype.expand = function (t, e) {
                var o = s.Event(p);
                if (this.options.accordion) {
                    var n = e.siblings(l)
                        , i = n.children(d);
                    this.collapse(i, n)
                }
                e.addClass(c),
                    t.slideDown(this.options.animationSpeed, function () {
                        s(this.element).trigger(o),
                            e.height("auto")
                    }
                        .bind(this))
            }
            ,
            n.prototype.collapse = function (t, e) {
                var o = s.Event(f);
                e.removeClass(c),
                    t.slideUp(this.options.animationSpeed, function () {
                        s(this.element).trigger(o),
                            e.find(a).removeClass(c).find(d).hide()
                    }
                        .bind(this))
            }
            ,
            n.prototype._setUpListeners = function () {
                var e = this;
                s(this.element).on("click", this.options.trigger, function (t) {
                    e.toggle(s(this), t)
                })
            }
            ;
        var u = s.fn.tree;
        s.fn.tree = e,
            s.fn.tree.Constructor = n,
            s.fn.tree.noConflict = function () {
                return s.fn.tree = u,
                    this
            }
            ,
            s(window).on("load", function () {
                s(t).each(function () {
                    e.call(s(this))
                })
            })
    }(jQuery),
    function (a) {
        "use strict";
        function i(t) {
            this.options = t,
                this.bindedResize = !1,
                this.activate()
        }
        var s = "lte.layout"
            , r = {
                slimscroll: !0,
                resetHeight: !0
            }
            , d = ".wrapper"
            , l = ".content-wrapper"
            , c = ".layout-boxed"
            , h = ".main-footer"
            , f = ".main-header"
            , t = ".main-sidebar"
            , e = "slimScrollDiv"
            , p = ".sidebar"
            , u = ".control-sidebar"
            , o = ".sidebar-menu"
            , n = ".main-header .logo"
            , g = "fixed"
            , v = "hold-transition";
        function b(n) {
            return this.each(function () {
                var t = a(this)
                    , e = t.data(s);
                if (!e) {
                    var o = a.extend({}, r, t.data(), "object" == typeof n && n);
                    t.data(s, e = new i(o))
                }
                if ("string" == typeof n) {
                    if (void 0 === e[n])
                        throw new Error("No method named " + n);
                    e[n]()
                }
            })
        }
        i.prototype.activate = function () {
            this.fix(),
                this.fixSidebar(),
                a("body").removeClass(v),
                this.options.resetHeight && a("body, html, " + d).css({
                    height: "auto",
                    "min-height": "100%"
                }),
                this.bindedResize || (a(window).resize(function () {
                    this.fix(),
                        this.fixSidebar(),
                        a(n + ", " + p).one("webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend", function () {
                            this.fix(),
                                this.fixSidebar()
                        }
                            .bind(this))
                }
                    .bind(this)),
                    this.bindedResize = !0),
                a(o).on("expanded.tree", function () {
                    this.fix(),
                        this.fixSidebar()
                }
                    .bind(this)),
                a(o).on("collapsed.tree", function () {
                    this.fix(),
                        this.fixSidebar()
                }
                    .bind(this))
        }
            ,
            i.prototype.fix = function () {
                a(c + " > " + d).css("overflow", "hidden");
                var t = a(h).outerHeight() || 0
                    , e = a(f).outerHeight() || 0
                    , o = e + t
                    , n = a(window).height()
                    , i = a(p).outerHeight() || 0;
                if (a("body").hasClass(g))
                    a(l).css("min-height", n - t);
                else {
                    var s;
                    s = i + e <= n ? (a(l).css("min-height", n - o),
                        n - o) : (a(l).css("min-height", i),
                            i);
                    var r = a(u);
                    void 0 !== r && r.height() > s && a(l).css("min-height", r.height())
                }
            }
            ,
            i.prototype.fixSidebar = function () {
                a("body").hasClass(g) ? this.options.slimscroll && void 0 !== a.fn.slimScroll && 0 === a(t).find(e).length && a(p).slimScroll({
                    height: a(window).height() - a(f).height() + "px"
                }) : void 0 !== a.fn.slimScroll && a(p).slimScroll({
                    destroy: !0
                }).height("auto")
            }
            ;
        var m = a.fn.layout;
        a.fn.layout = b,
            a.fn.layout.Constuctor = i,
            a.fn.layout.noConflict = function () {
                return a.fn.layout = m,
                    this
            }
            ,
            a(window).on("load", function () {
                b.call(a("body"))
            })
    }(jQuery);
