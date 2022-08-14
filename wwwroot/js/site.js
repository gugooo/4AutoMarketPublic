
$(document).ready(function () {
    window.setInterval(function () {
        //$('.next').trigger('click');
        $.each($('.SliderWrap .next'), function (i, e) { changeSlide(e); });
        $.each($('.Slider5Wrap .next'), function (i, e) { changeSlide5(e); });
    }, 8000);
    //Submit RegistrationForm
    $("#regForm").ajaxForm({
        beforeSubmit: function (arr, form, options) {
            if ($(form).find('span').hasClass('field-validation-error')) {
                return false;
            }
            $('#registrationSpinner').removeClass('d-none');
        },
        success: function (response) {
            $('#registrationSpinner').addClass('d-none');
            if (response.errors.length) {
                $("#RegFormVal").empty();
                $.each(response.errors, function (i, val) {
                    $("#RegFormVal").append("<div>" + val + "</div>");
                });
            }
            else {
                alert("Thank You for registration.\nPlease check your email and confirm it.");
                location.reload();
            }
        }
    });
    //Submit LoginForm
    $("#loginForm").ajaxForm({
        beforeSubmit: function (arr, form, options) {
            if ($(form).find('span').hasClass('field-validation-error')) {
                return false;
            }
        },
        success: function (response) {
            if (response.errors.length) {
                $("#loginFormVal").empty();
                $.each(response.errors, function (i, val) {
                    $("#loginFormVal").append("<div>" + val + "</div>");
                });
            }
            else {
                location.reload();
            }
        }
    });
    //Rset Password

    $("#fogotForm").ajaxForm({
        beforeSubmit: function (arr, form, options) {
            if ($(form).find('span').hasClass('field-validation-error')) {
                return false;
            }
            $(form).find('.spinner-border').removeClass('d-none');
        },
        success: function (response, st, form) {
            if (response.errors.length) {
                $("#fogotForm").find('.spinner-border').addClass('d-none');
                $("#fogotFormVal").empty();
                $.each(response.errors, function (i, val) {
                    $("#fogotFormVal").append("<div>" + val + "</div>");
                });
            }
            else {
                if (response.massage && response.massage.length) alert(response.massage);
                location.reload();
            }
        }
    });

    var pr = $.cookie("FavoritIds") && JSON.parse($.cookie("FavoritIds"));
    pr && pr.length && $('#favoritesWrap small').removeClass('d-none').text(pr.length > 99 ? '99+' : pr.length);

    var prB = $.cookie("ProductList") && JSON.parse($.cookie("ProductList"));
    if ($.isArray(prB)) {
        var prCount = 0;
        $.each(prB, function () { prCount = prCount + this.count; })
        prB && prB.length && $('#ShoppingBagWrap small').removeClass('d-none').text(prCount > 99 ? '99+' : prCount);
    }
    $('.SliderWrap').data('Index', 0);
    $('.Slider5Wrap').data('Index', 0);

    $('#liveChatStart').click(function () {
        if (!$('#startChatWrap textarea').val()) {
            $('#startChatWrap textarea').addClass('border-danger').focus();
            event.stopPropagation();
            return false;
        }
        $('#startChatWrap').addClass('d-none');
        $('#messagingChatWrap').removeClass('d-none');
        $('#startChatWrap textarea').removeClass('border-danger');
        HubBild();
    });
    $('#serchInput').click(function () { $('#serchInput').trigger('keyup'); });
    $('#serchInput').keyup(function () {

        $.post('/Home/getSerchKeys?val=' + $(this).val() + '&catalogId=' + $('#serchCatalogList').val(), function (data) {
            console.log(data);
            if (data) {
                if (data && data.length) {
                    $('#serchResList').empty();
                    $.each(data, function (i, e) {
                        $('#serchResList').append("<div class='serchKey'>" + e + "</div>");
                    });
                    $('#serchResWrap').removeClass('d-none');
                }
                else {
                    $('#serchResWrap').addClass('d-none');
                }
            }
        });
    });
    $('#serchResList').click(function (e) {
        if ($(e.target).hasClass('serchKey')) {
            $('#serchInput').val($(e.target).text());
            $('#serchSubmit').trigger('click');
        }
    });
    $(document).click(function () {
        $("#serchResWrap").addClass('d-none');
    });
    $("#serchResWrap").click(function (e) {
        $("#serchResWrap").removeClass('d-none');
    });
    $('#serchSubmit').click(function () {
        if ($('#serchInput').val()) {
            $.post('/Home/serchHistory?key=' + $('#serchInput').val());
            $(this).closest('form').submit();
        }
    });
    $('#SubscribeButton').click(function () {
        if ($('#SubscribeNewEmail').val()) {
            $.post('/Home/Subscribe?email=' + $('#SubscribeNewEmail').val(), function (data) {
                if (data && data.res) {
                    $('#SubscribeNewEmail').addClass('bg-success').removeClass('bg-danger');
                }
                else $('#SubscribeNewEmail').removeClass('bg-success').addClass('bg-danger');
            });
        }
        else $('#SubscribeNewEmail').removeClass('bg-success').addClass('bg-danger');
    });

});
function changeSlide(el) {
    var parent = $(el).parent();
    if ($(el).hasClass('next')) ShowSlide(parent.data('Index') + 1, parent);
    else ShowSlide(parent.data('Index') - 1, parent);
}
function changeSlide5(el) {
    var parent = $(el).parent();
    if ($(el).hasClass('next')) ShowSlide5(parent.data('Index') + 1, parent);
    else ShowSlide5(parent.data('Index') - 1, parent);
}

function ShowSlide(n, slide) {
    var items = slide.find(".SliderItem");
    items.removeClass('d-block');
    if (n > (items.length - 1)) n = 0;
    else if (n < 0) n = items.length - 1;
    $(items[n]).addClass('d-block');
    slide.data('Index', n);
}
function ShowSlide5(n, slide) {
    var items = slide.find(".SliderItem");
    if (items.length <= 5) return;
    items.removeClass('d-inline-block');
    if (n > (items.length - 5)) n = 0;
    else if (n < 0) n = items.length - 5;
    for (var i = n; i < n + 5; i++)$(items[i]).addClass('d-inline-block');

    slide.data('Index', n);
}

function popupCenter(url, title, w, h) {
    // Fixes dual-screen position                             Most browsers      Firefox
    //const dualScreenLeft = window.screenLeft !== undefined ? window.screenLeft : window.screenX;
    //const dualScreenTop = window.screenTop !== undefined ? window.screenTop : window.screenY;

    const width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    const height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    //const systemZoom = width / window.screen.availWidth;
    const left = (width - w) / 2;// / systemZoom + dualScreenLeft;
    const top = (height - h) / 2;// / systemZoom + dualScreenTop;
    const newWindow = window.open(url, title,
        `
      scrollbars=yes,
      width=${w}, 
      height=${h}, 
      top=${top}, 
      left=${left}
      `
    )
    return newWindow;
}

function googleLogin() {
    var newPop = popupCenter("http://4auto.am/Account/GoogleLogin", "Google", 600, 600);
    var timer = setInterval(function () {
        if (newPop.closed) {
            clearInterval(timer);
            location.reload();
        }
    }, 1000);
}
function facebookLogin() {
    var newPop = popupCenter("http://4auto.am/Account/FacebookLogin", "Facebook", 600, 600);
    var timer = setInterval(function () {
        if (newPop.closed) {
            clearInterval(timer);
            location.reload();
        }
    }, 1000);
}

function HubBild() {
    const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    hubConnection.on("newMessage", function (name, message) {
        createOpMessage(name, message);
        $('#messagingChat').animate({ scrollTop: $('#messagingChat')[0].scrollHeight }, 1000);
    });

    $("#sendNewMessage").click(function (e) {
        var message = $("#newMessage").val();
        if (!message) {
            $('#newMessage').focus();
            event.stopPropagation();
            return false;
        }
        var name = $('#chatName').val() || "User";
        $("#newMessage").val("");
        createMessage(message);
        $('#messagingChat').animate({ scrollTop: $('#messagingChat')[0].scrollHeight }, 1000);
        hubConnection.invoke("GetMessage", name, message);
    });
    $("#abortChat").click(function () {
        $('#chatName').val("");
        $('#liveChatUserPhone').val("");
        $('#liveChatUserEmail').val("");
        $('#startChatWrap textarea').val("");
        $('#newMessage').val("");
        $('#messagingChat').empty();
        $('#messagingChatWrap').addClass('d-none');
        $('#startChatWrap').removeClass('d-none');
        hubConnection.stop();
    });
    function createMessage(message) {
        var name = $('#chatName').val() || "User";
        var dn = new Date();
        var timeNow = ((dn.getHours() < 10) ? "0" : "") + dn.getHours() + ":" + ((dn.getMinutes() < 10) ? "0" : "") + dn.getMinutes();
        return $('#messagingChat').append('<div class="userMessage"><div><b>' + name + '</b> <span>' + timeNow + '</span><br/><span>' + message + '</span></div></div>');
    }
    function createOpMessage(name, message) {
        var dn = new Date();
        var timeNow = ((dn.getHours() < 10) ? "0" : "") + dn.getHours() + ":" + ((dn.getMinutes() < 10) ? "0" : "") + dn.getMinutes();
        $('#messagingChat').append('<div class="operatorMessage"><div><b>' + name + '</b> <span>' + timeNow + '</span><br/><span>' + message + '</span></div></div>');
    }
    function connectData() {
        var name = $('#chatName').val() || "User";
        hubConnection.invoke("GetMessage", name, ("Phone: " + ($('#liveChatUserPhone').val() ? $('#liveChatUserPhone').val() : "") + ", Email: " + ($('#liveChatUserEmail').val() ? $('#liveChatUserEmail').val() : "")));
        createMessage($('#startChatWrap textarea').val());
        hubConnection.invoke("GetMessage", name, $('#startChatWrap textarea').val());
    }
    hubConnection.start().then(connectData);

}
/*
function isElementInViewport(elem) {
    var $elem = $(elem);

    // Get the scroll position of the page.
    var scrollElem = ((navigator.userAgent.toLowerCase().indexOf('webkit') != -1) ? 'body' : 'html');
    var viewportTop = $(scrollElem).scrollTop();
    var viewportBottom = viewportTop + $(window).height();

    // Get the position of the element on the page.
    var elemTop = Math.round($elem.offset().top);
    var elemBottom = elemTop + $elem.height();

    return ((elemTop < viewportBottom) && (elemBottom > viewportTop));
}

// Check if it's time to start the animation.
function checkAnimation() {
    var $elem = $('.level');
    $.each($elem, function (i, e) {
        if ($(e).hasClass('animate')) return true;

        if (isElementInViewport($elem)) {
            // Start the animation
            $(e).addClass('animate');
            $(e).fadeIn(500);
        }
    });

    // If the animation has already been started

}
*/
// Capture scroll events
