/*
Template Name: Adminto - Responsive Bootstrap 5 Admin Dashboard
Author: CoderThemes
Website: https://coderthemes.com/
Contact: support@coderthemes.com
File: Layout
*/

class LeftSidebar {
  constructor() {
    this.body = $("body");
    this.window = $(window);
  }

  initMenu() {
    var self = this;

    // var defaultSidebarSize = sidebar.size ? sidebar.size : 'default';

    // resets everything

    // Left menu collapse
    // $('.button-menu-mobile').on('click', function (event) {
    //     event.preventDefault();
    //     var sidebarSize = self.body.attr('data-sidebar-size');
    //     if (self.window.width() >= 993) {
    //         if (sidebarSize === 'condensed') {
    //             self.changeSize(defaultSidebarSize);
    //             window.dispatchEvent(new Event('adminto.setFluid'));
    //         } else {
    //             self.changeSize('condensed');
    //             window.dispatchEvent(new Event('adminto.setBoxed'));
    //         }
    //     } else {
    //         self.changeSize(defaultSidebarSize);
    //         self.body.toggleClass('sidebar-enable');
    //         window.dispatchEvent(new Event('adminto.setFluid'));
    //     }
    // });

    // sidebar - main menu
    if ($("#side-menu").length) {
      var navCollapse = $("#side-menu li .collapse");

      // open one menu at a time only
      navCollapse.on({
        "show.bs.collapse": function (event) {
          var parent = $(event.target).parents(".collapse.show");
          $("#side-menu .collapse.show").not(parent).collapse("hide");
        },
      });

      // activate the menu in left side bar (Vertical Menu) based on url
      $("#side-menu a").each(function () {
        var pageUrl = window.location.href.split(/[?#]/)[0];
        if (this.href == pageUrl) {
          $(this).addClass("active");
          $(this).parent().addClass("menuitem-active");
          $(this).parent().parent().parent().addClass("show");
          $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .addClass("menuitem-active"); // add active to li of the current link

          var firstLevelParent = $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent();
          if (firstLevelParent.attr("id") !== "sidebar-menu")
            firstLevelParent.addClass("show");

          $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .addClass("menuitem-active");

          var secondLevelParent = $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent();
          if (secondLevelParent.attr("id") !== "wrapper")
            secondLevelParent.addClass("show");

          var upperLevelParent = $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent();
          if (!upperLevelParent.is("body"))
            upperLevelParent.addClass("menuitem-active");
        }
      });
    }

    // handling two columns menu if present
    var twoColSideNav = $("#two-col-sidenav-main");
    if (twoColSideNav.length) {
      var twoColSideNavItems = $("#two-col-sidenav-main .nav-link");
      var sideSubMenus = $(".twocolumn-menu-item");

      // showing/displaying tooltip based on screen size
      // if (this.window.width() >= 585) {
      //     twoColSideNavItems.tooltip('enable');
      // } else {
      //     twoColSideNavItems.tooltip('disable');
      // }

      var nav = $(".twocolumn-menu-item .nav-second-level");
      var navCollapse = $("#two-col-menu li .collapse");

      // open one menu at a time only
      navCollapse.on({
        "show.bs.collapse": function () {
          var nearestNav = $(this).closest(nav).closest(nav).find(navCollapse);
          if (nearestNav.length) nearestNav.not($(this)).collapse("hide");
          else navCollapse.not($(this)).collapse("hide");
        },
      });

      twoColSideNavItems.on("click", function (e) {
        var target = $($(this).attr("href"));

        if (target.length) {
          e.preventDefault();

          twoColSideNavItems.removeClass("active");
          $(this).addClass("active");

          sideSubMenus.removeClass("d-block");
          target.addClass("d-block");

          // showing full sidebar if menu item is clicked
          $.LayoutThemeApp.leftSidebar.changeSize("default");
          return false;
        }
        return true;
      });

      // activate menu with no child
      var pageUrl = window.location.href; //.split(/[?#]/)[0];
      twoColSideNavItems.each(function () {
        if (this.href === pageUrl) {
          $(this).addClass("active");
        }
      });

      // activate the menu in left side bar (Two column) based on url
      $("#two-col-menu a").each(function () {
        if (this.href == pageUrl) {
          $(this).addClass("active");
          $(this).parent().addClass("menuitem-active");
          $(this).parent().parent().parent().addClass("show");
          $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .addClass("menuitem-active"); // add active to li of the current link

          var firstLevelParent = $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent();
          if (firstLevelParent.attr("id") !== "sidebar-menu")
            firstLevelParent.addClass("show");

          $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .addClass("menuitem-active");

          var secondLevelParent = $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent();
          if (secondLevelParent.attr("id") !== "wrapper")
            secondLevelParent.addClass("show");

          var upperLevelParent = $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent();
          if (!upperLevelParent.is("body"))
            upperLevelParent.addClass("menuitem-active");

          // opening menu
          var matchingItem = null;
          var targetEl =
            "#" + $(this).parents(".twocolumn-menu-item").attr("id");
          $("#two-col-sidenav-main .nav-link").each(function () {
            if ($(this).attr("href") === targetEl) {
              matchingItem = $(this);
            }
          });
          if (matchingItem) matchingItem.trigger("click");
        }
      });
    }
  }

  init() {
    this.initMenu();
  }
}

class Topbar {
  constructor() {
    this.body = $("body");
    this.window = $(window);
  }

  toggleRightSideBar() {
    var self = this;
    if (document.body.classList.contains("right-bar-enabled"))
      document.body.classList.remove("right-bar-enabled");
    else document.body.classList.add("right-bar-enabled");
  }

  initMenu() {
    const self = this;
    document
      .querySelector(".right-bar-toggle")
      ?.addEventListener("click", function () {
        self.toggleRightSideBar();
      });

    // Serach Toggle
    $("#top-search").on("click", function (e) {
      $("#search-dropdown").addClass("d-block");
    });

    // hide search on opening other dropdown
    $(".topbar-dropdown").on("show.bs.dropdown", function () {
      $("#search-dropdown").removeClass("d-block");
    });

    //activate the menu in topbar(horizontal menu) based on url
    $(".navbar-nav a").each(function () {
      var pageUrl = window.location.href.split(/[?#]/)[0];
      if (this.href == pageUrl) {
        $(this).addClass("active");
        $(this).parent().addClass("active");
        $(this).parent().parent().addClass("active");

        $(this).parent().parent().parent().addClass("active");
        $(this).parent().parent().parent().parent().addClass("active");
        if (
          $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .hasClass("mega-dropdown-menu")
        ) {
          $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .addClass("active");
          $(this)
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .parent()
            .addClass("active");
        } else {
          var child = $(this)
            .parent()
            .parent()[0]
            .querySelector(".dropdown-item");
          if (child) {
            var pageUrl = window.location.href.split(/[?#]/)[0];
            if (
              child.href == pageUrl ||
              child.classList.contains("dropdown-toggle")
            )
              child.classList.add("active");
          }
        }
        var el = $(this)
          .parent()
          .parent()
          .parent()
          .parent()
          .addClass("active")
          .prev();
        if (el.hasClass("nav-link")) el.addClass("active");
      }
    });

    // Topbar - main menu
    $(".navbar-toggle").on("click", function (event) {
      $(this).toggleClass("open");
      $("#navigation").slideToggle(400);
    });

    //Horizontal Menu (For SM Screen)
    var AllNavs = document.querySelectorAll(
      "ul.navbar-nav .dropdown .dropdown-toggle"
    );

    var isInner = false;

    AllNavs.forEach(function (element) {
      element.addEventListener("click", function (event) {
        if (!element.parentElement.classList.contains("nav-item")) {
          isInner = true;
          element.parentElement.parentElement.classList.add("show");
          var parent =
            element.parentElement.parentElement.parentElement.querySelector(
              ".nav-link"
            );
          parent.ariaExpanded = true;
          parent.classList.add("show");
          bootstrap.Dropdown.getInstance(element).show();
        }
      });

      element.addEventListener("hide.bs.dropdown", function (event) {
        if (isInner) {
          event.preventDefault();
          event.stopPropagation();
          isInner = false;
        }
      });
    });
  }

  init() {
    this.initMenu();
  }
}

class RightSidebar {
  constructor() {
    this.body = $("body");
    this.window = $(window);
  }

  init() {
    var self = this;

    $(document).on("click", "body", function (e) {
      // hiding search bar
      if ($(e.target).closest("#top-search").length !== 1) {
        $("#search-dropdown").removeClass("d-block");
      }
      if ($(e.target).closest(".right-bar-toggle, .right-bar").length > 0) {
        return;
      }

      if (
        $(e.target).closest(".left-side-menu, .side-nav").length > 0 ||
        $(e.target).hasClass("button-menu-mobile") ||
        $(e.target).closest(".button-menu-mobile").length > 0
      ) {
        return;
      }

      $("body").removeClass("right-bar-enabled");
      $("body").removeClass("sidebar-enable");
    });
  }
}

class ThemeCustomizer {
  constructor() {
    this.body = document.body;
    this.defaultConfig = {
      leftbar: {
        color: "light",
        size: "default",
        position: "fixed",
      },
      layout: {
        color: "light",
        size: "fluid",
        mode: "default",
      },
      topbar: {
        color: "light",
      },
      sidebar: {
        user: true,
      },
    };
  }

  initConfig() {
    let config = JSON.parse(JSON.stringify(this.defaultConfig));
    config["leftbar"]["color"] =
      this.body.getAttribute("data-leftbar-color") ??
      this.defaultConfig.leftbar.color;
    config["leftbar"]["size"] =
      this.body.getAttribute("data-leftbar-size") ??
      this.defaultConfig.leftbar.size;
    config["leftbar"]["position"] =
      this.body.getAttribute("data-leftbar-position") ??
      this.defaultConfig.leftbar.position;
    config["layout"]["color"] =
      this.body.getAttribute("data-layout-color") ??
      this.defaultConfig.layout.color;
    config["layout"]["size"] =
      this.body.getAttribute("data-layout-size") ??
      this.defaultConfig.layout.size;
    config["layout"]["mode"] =
      this.body.getAttribute("data-layout-mode") ??
      this.defaultConfig.layout.mode;
    config["topbar"]["color"] =
      this.body.getAttribute("data-topbar-color") ??
      this.defaultConfig.topbar.color;
    config["sidebar"]["user"] =
      this.body.getAttribute("data-sidebar-user") ??
      this.defaultConfig.sidebar.user;
    this.defaultConfig = JSON.parse(JSON.stringify(config));
    this.config = config;
    this.setSwitchFromConfig();
  }

  changeLeftbarColor(color) {
    this.config.leftbar.color = color;
    this.body.setAttribute("data-leftbar-color", color);
    this.setSwitchFromConfig();
  }

  changeLeftbarPosition(position) {
    this.config.leftbar.position = position;
    this.body.setAttribute("data-leftbar-position", position);
    this.setSwitchFromConfig();
  }

  changeLeftbarSize(size) {
    this.config.leftbar.size = size;
    this.body.setAttribute("data-leftbar-size", size);
    this.setSwitchFromConfig();
  }

  changeLayoutMode(mode) {
    this.config.layout.mode = mode;
    this.body.setAttribute("data-layout-mode", mode);
    this.setSwitchFromConfig();
  }

  changeLayoutColor(color) {
    this.config.layout.color = color;
    this.body.setAttribute("data-layout-color", color);
    this.setSwitchFromConfig();
  }

  changeLayoutSize(size) {
    this.config.layout.size = size;
    this.body.setAttribute("data-layout-size", size);
    this.setSwitchFromConfig();
  }

  changeTopbarColor(color) {
    this.config.topbar.color = color;
    this.body.setAttribute("data-topbar-color", color);
    this.setSwitchFromConfig();
  }

  changeSidebarUser(showUser) {
    this.config.sidebar.user = showUser;
    if (showUser) {
      this.body.setAttribute("data-sidebar-user", showUser);
    } else {
      this.body.removeAttribute("data-sidebar-user");
    }
    this.setSwitchFromConfig();
  }

  resetTheme() {
    this.config = JSON.parse(JSON.stringify(this.defaultConfig));
    this.changeLeftbarColor(this.config.leftbar.color);
    this.changeLeftbarPosition(this.config.leftbar.position);
    this.changeLeftbarSize(this.config.leftbar.size);
    this.changeLayoutColor(this.config.layout.color);
    this.changeLayoutSize(this.config.layout.size);
    this.changeLayoutMode(this.config.layout.mode);
    this.changeTopbarColor(this.config.topbar.color);
    this.changeSidebarUser(this.config.sidebar.user);
  }

  initSwitchListener() {
    const self = this;
    document
      .querySelectorAll("input[name=leftbar-color]")
      .forEach(function (element) {
        element.addEventListener("change", function (e) {
          self.changeLeftbarColor(element.value);
        });
      });
    document
      .querySelectorAll("input[name=leftbar-size]")
      .forEach(function (element) {
        element.addEventListener("change", function (e) {
          self.changeLeftbarSize(element.value);
        });
      });
    document
      .querySelectorAll("input[name=leftbar-position]")
      .forEach(function (element) {
        element.addEventListener("change", function (e) {
          self.changeLeftbarPosition(element.value);
        });
      });
    document
      .querySelectorAll("input[name=layout-color]")
      .forEach(function (element) {
        element.addEventListener("change", function (e) {
          self.changeLayoutColor(element.value);
        });
      });
    document
      .querySelectorAll("input[name=layout-size]")
      .forEach(function (element) {
        element.addEventListener("change", function (e) {
          self.changeLayoutSize(element.value);
        });
      });

    document
      .querySelectorAll("input[name=layout-mode]")
      .forEach(function (element) {
        element.addEventListener("change", function (e) {
          self.changeLayoutMode(element.value);
        });
      });
    document
      .querySelectorAll("input[name=topbar-color]")
      .forEach(function (element) {
        element.addEventListener("change", function (e) {
          self.changeTopbarColor(element.value);
        });
      });
    document
      .querySelectorAll("input[name=sidebar-user]")
      .forEach(function (element) {
        element.addEventListener("change", function (e) {
          self.changeSidebarUser(element.checked);
        });
      });
    document
      .querySelector("#resetBtn")
      ?.addEventListener("click", function (e) {
        self.resetTheme();
      });

    document
      .querySelector(".button-menu-mobile")
      ?.addEventListener("click", function () {
        if (self.config.leftbar.size === "default") {
          self.changeLeftbarSize("condensed");
        } else {
          self.changeLeftbarSize("default");
        }
      });
  }

  setSwitchFromConfig() {
    document
      .querySelectorAll(".right-bar input[type=checkbox]")
      .forEach(function (checkbox) {
        checkbox.checked = false;
      });
    let config = this.config;
    if (config) {
      let leftbarColorSwitch = document.querySelector(
        "input[type=checkbox][name=leftbar-color][value=" +
          config.leftbar.color +
          "]"
      );
      let leftbarSizeSwitch = document.querySelector(
        "input[type=checkbox][name=leftbar-size][value=" +
          config.leftbar.size +
          "]"
      );
      let leftbarPositionSwitch = document.querySelector(
        "input[type=checkbox][name=leftbar-position][value=" +
          config.leftbar.position +
          "]"
      );

      let layoutColorSwitch = document.querySelector(
        "input[type=checkbox][name=layout-color][value=" +
          config.layout.color +
          "]"
      );
      let layoutSizeSwitch = document.querySelector(
        "input[type=checkbox][name=layout-size][value=" +
          config.layout.size +
          "]"
      );
      let layoutModeSwitch = document.querySelector(
        "input[type=checkbox][name=layout-mode][value=" +
          config.layout.type +
          "]"
      );

      let topbarColorSwitch = document.querySelector(
        "input[type=checkbox][name=topbar-color][value=" +
          config.topbar.color +
          "]"
      );
      let sidebarUserSwitch = document.querySelector(
        "input[type=checkbox][name=sidebar-user]"
      );

      if (leftbarColorSwitch) leftbarColorSwitch.checked = true;
      if (leftbarSizeSwitch) leftbarSizeSwitch.checked = true;
      if (leftbarPositionSwitch) leftbarPositionSwitch.checked = true;

      if (layoutColorSwitch) layoutColorSwitch.checked = true;
      if (layoutSizeSwitch) layoutSizeSwitch.checked = true;
      if (layoutModeSwitch) layoutModeSwitch.checked = true;

      if (topbarColorSwitch) topbarColorSwitch.checked = true;
      if (sidebarUserSwitch && config.sidebar.user.toString() === "true")
        sidebarUserSwitch.checked = true;
    }
  }

  init() {
    this.initConfig();
    this.initSwitchListener();
    // this.setSwitchFromConfig();
  }
}

class Layout {
  init() {
    this.themeCustomizer = new ThemeCustomizer();
    this.themeCustomizer.init();
    this.leftSidebar = new LeftSidebar();
    this.topbar = new Topbar();
    this.rightSidebar = new RightSidebar(this);
    this.rightSidebar.init();
    this.topbar.init();
    this.leftSidebar.init();
  }
}

window.addEventListener("DOMContentLoaded", function (e) {
  new Layout().init();
});
