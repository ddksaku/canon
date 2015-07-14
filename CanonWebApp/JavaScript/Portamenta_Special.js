try {
    HTMLElement.prototype.click = function() {
        var evt = this.ownerDocument.createEvent('MouseEvents');
        evt.initMouseEvent('click', true, true, this.ownerDocument.defaultView, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
        this.dispatchEvent(evt);
    }
}
catch (err) {
}

function TogglePanel(id) {
    var o = document.getElementById(id);
    if (o.style.display == "block" || o.style.display == "") {
        o.style.display = "none";
    }
    else {
        o.style.display = "block";
    }
    return false;
}

function XBrowserAddHandler(target, eventName, handlerName) {
    if (target.addEventListener)
        target.addEventListener(eventName, handlerName, false);
    else if (target.attachEvent)
        target.attachEvent("on" + eventName, handlerName);
    else
        target["on" + eventName] = handlerName;
}

function GenerateSelect(s, values) {
    arrValues = values.toString().split('$$$')
    var objSelect = document.getElementById(s);
    objSelect.innerHTML = "";
    for (i = 0; i < arrValues.length; i++) {
        var objOption = document.createElement("option");
        arrOption = arrValues[i].split('===');
        theText = document.createTextNode(arrOption[0]);
        objOption.appendChild(theText);
        objOption.setAttribute("value", arrOption[1]);
        if (arrOption[2] == "True") {
            objOption.setAttribute("selected", "selected");
        }
        if (arrOption[3] == "False") {
            objOption.setAttribute("disabled", "disabled");
        }

        objSelect.appendChild(objOption);
    }
}

function getStyle(oElm, strCssRule) {
    var strValue = "";
    if (document.defaultView && document.defaultView.getComputedStyle) {
        strValue = document.defaultView.getComputedStyle(oElm, "").getPropertyValue(strCssRule);
    }
    else if (oElm.currentStyle) {
        strCssRule = strCssRule.replace(/\-(\w)/g, function(strMatch, p1) {
            return p1.toUpperCase();
        });
        strValue = oElm.currentStyle[strCssRule];
    }
    return strValue;
}

function getElementsByClassName(oElm, strTagName, oClassNames) {
    var arrElements = (strTagName == "*" && oElm.all) ? oElm.all : oElm.getElementsByTagName(strTagName);
    var arrReturnElements = new Array();
    var arrRegExpClassNames = new Array();
    if (typeof oClassNames == "object") {
        for (var i = 0; i < oClassNames.length; i++) {
            arrRegExpClassNames.push(new RegExp("(^|\\s)" + oClassNames[i].replace(/\-/g, "\\-") + "(\\s|$)"));
        }
    }
    else {
        arrRegExpClassNames.push(new RegExp("(^|\\s)" + oClassNames.replace(/\-/g, "\\-") + "(\\s|$)"));
    }
    var oElement;
    var bMatchesAll;
    for (var j = 0; j < arrElements.length; j++) {
        oElement = arrElements[j];
        bMatchesAll = true;
        for (var k = 0; k < arrRegExpClassNames.length; k++) {
            if (!arrRegExpClassNames[k].test(oElement.className)) {
                bMatchesAll = false;
                break;
            }
        }
        if (bMatchesAll) {
            arrReturnElements.push(oElement);
        }
    }
    return (arrReturnElements)
}

function GetClientID(id) {
    if (id.indexOf('_') > 0) {
        return id.split('_')[id.split('_').length - 1];
    }
    return id;
}

function limitText(limitField, limitNum) {
    if (limitField.value.length > limitNum) {
        limitField.value = limitField.value.substring(0, limitNum);
    }
}

function ScrollerTop() {
    var sc = 0;
    if (typeof (window.pageYOffset) == 'number') {
        // Netscape
        sc = window.pageYOffset;
    } else if (document.body && (document.body.scrollLeft || document.body.scrollTop)) {
        // DOM
        sc = document.body.scrollTop;
    } else if (document.documentElement && (document.documentElement.scrollLeft || document.documentElement.scrollTop)) {
        // IE6 standards compliant mode
        sc = document.documentElement.scrollTop;
    }
    return sc;
}

function getDocHeight() {
    var D = document;
    return Math.max(
        Math.max((document.body && (document.body.scrollLeft || document.body.scrollTop)) ? D.body.scrollHeight : 0, D.documentElement.scrollHeight),
        Math.max((document.body && (document.body.scrollLeft || document.body.scrollTop)) ? D.body.offsetHeight : 0, D.documentElement.offsetHeight),
        Math.max((document.body && (document.body.scrollLeft || document.body.scrollTop)) ? D.body.clientHeight : 0, D.documentElement.clientHeight)
    );
}


function GetViewportWidth() {
    var viewportwidth;
    var viewportheight;

    // the more standards compliant browsers (mozilla/netscape/opera/IE7) use window.innerWidth and window.innerHeight

    if (typeof window.innerWidth != 'undefined') {
        viewportwidth = window.innerWidth,
          viewportheight = window.innerHeight
    }

    // IE6 in standards compliant mode (i.e. with a valid doctype as the first line in the document)

    else if (typeof document.documentElement != 'undefined'
         && typeof document.documentElement.clientWidth !=
         'undefined' && document.documentElement.clientWidth != 0) {
        viewportwidth = document.documentElement.clientWidth,
           viewportheight = document.documentElement.clientHeight
    }

    // older versions of IE

    else {
        viewportwidth = document.getElementsByTagName('body')[0].clientWidth,
           viewportheight = document.getElementsByTagName('body')[0].clientHeight
    }
    if (getDocHeight() > GetViewportHeight() && (document.body && (document.body.scrollLeft || document.body.scrollTop))) {
        viewportwidth = viewportwidth - 17;
    }

    return viewportwidth;
}

function GetViewportHeight() {
    var viewportwidth;
    var viewportheight;

    // the more standards compliant browsers (mozilla/netscape/opera/IE7) use window.innerWidth and window.innerHeight

    if (typeof window.innerWidth != 'undefined') {
        viewportwidth = window.innerWidth,
          viewportheight = window.innerHeight
    }

    // IE6 in standards compliant mode (i.e. with a valid doctype as the first line in the document)

    else if (typeof document.documentElement != 'undefined'
         && typeof document.documentElement.clientWidth !=
         'undefined' && document.documentElement.clientWidth != 0) {
        viewportwidth = document.documentElement.clientWidth,
           viewportheight = document.documentElement.clientHeight
    }

    // older versions of IE

    else {
        viewportwidth = document.getElementsByTagName('body')[0].clientWidth,
           viewportheight = document.getElementsByTagName('body')[0].clientHeight
    }
    return viewportheight;
}

function Vimeo(l, i) {
    if (document.getElementById(i).value == "") {
        document.getElementById(l).style.display = "none";
    }
}

function Vimeo2(l, i) {
    if (document.getElementById(i).value == "") {
        document.getElementById(l).style.display = "block";
    }
}

function Tooltip(e, s) {
    var sc = ScrollerTop();

    if (e.clientX < (document.body.clientWidth - 90)) {
        document.getElementById("tooltip").style.left = e.clientX + "px";
    }
    if ((e.clientY + sc - 34) > (sc)) {
        document.getElementById("tooltip").style.top = (e.clientY + sc - 34) + "px";
    }

    document.getElementById("tooltip").style.display = "block";
    document.getElementById("tooltip").innerHTML = s + "<b></b>";
}
function TooltipOff() {
    document.getElementById("tooltip").style.display = "none";
}

function InitFields() {
    for (i = 0; i < document.getElementsByTagName("input").length; i++) {
        if (document.getElementById("vm" + document.getElementsByTagName("input")[i].id) != null) {
            document.getElementById("vm" + document.getElementsByTagName("input")[i].id).style.display = "none";
            Vimeo2("vm" + document.getElementsByTagName("input")[i].id, document.getElementsByTagName("input")[i].id);
        }
    }
    for (i = 0; i < document.getElementsByTagName("textarea").length; i++) {
        if (document.getElementById("vm" + document.getElementsByTagName("textarea")[i].id) != null) {
            document.getElementById("vm" + document.getElementsByTagName("textarea")[i].id).style.display = "none";
            Vimeo2("vm" + document.getElementsByTagName("textarea")[i].id, document.getElementsByTagName("textarea")[i].id);
        }
    }
}

function alertx(s) {
    //document.getElementById("x").innerHTML = s + "<br>";
}

function SetVisible(o) {
    if (o.toString().indexOf("HTMLTableRowElement") > 0) {
        o.style.display = "table-row";
    }
    else {
        o.style.display = "block";
    }
}
function SetInVisible(o) {
    o.style.display = "none";
}
function SetEnabled(o) {
    o.disabled = false;
}
function SetDisabled(o) {
    o.disabled = true;
}