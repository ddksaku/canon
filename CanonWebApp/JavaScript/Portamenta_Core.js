/*************************/
/*         BASE          */
/*************************/

// ajax request (if not overdriven in page)
var ajax_url = "";
// all validations after first send
var revalidateOn = false;

// OnLoad action
// window.onload = PageLoad;

function InitGraphSize(w, h) {
    document.getElementById(w).value = GetViewportWidth();
    document.getElementById(h).value = GetViewportHeight();
}


/*************************/
/*       AJAX CORE       */
/*************************/

// Multithreading
var threadIndex = -1;
var actions = [];
var urls = [];
var http_request = false;
var values = "";
var controlactions = "";

// Ajax callback
function alertContents() {
    if (http_request.readyState == 4) {
        if (http_request.status == 200) {
            if (actions[threadIndex] == "ValidateFormAndSend" || actions[threadIndex] == "ValidateForm") {
                s = http_request.responseText;
                errors = s.split('|')[0];
                oks = s.split('|')[1];
                values = s.split('|')[2];
                controlactions = s.split('|')[3];
                if (actions[threadIndex] == "ValidateFormAndSend") {
                    revalidateOn = true;
                }

                ShowFormErrors(oks, errors);
                ShowValues(values);
                ExecuteActions(controlactions);
                //document.getElementById("x").innerHTML += "end ";
            }

            // Multithread
            if (actions[threadIndex + 1] != "" && actions[threadIndex + 1] != undefined) {
                threadIndex++;
                makeRequest(urls[threadIndex + 1]);
            }
            else {
                actions = [];
                urls = [];
                threadIndex = -1;
            }
        }
        // End Process
    }
    else {
        // Ajax Error
    }
}

// Ajax request
function makeRequest(adresa, parameters, action) {
    threadIndex = threadIndex + 1;
    actions[threadIndex] = action;
    urls[threadIndex] = adresa;

    if (window.XMLHttpRequest) {
        // Mozilla, Safari,...
        http_request = new XMLHttpRequest();
    }
    else {
        if (window.ActiveXObject) {
            // IE
            try {
                http_request = new ActiveXObject("Msxml2.XMLHTTP");
            }
            catch (e) {
                try {
                    http_request = new ActiveXObject("Microsoft.XMLHTTP");
                }
                catch (e) {
                }
            }
        }
    }
    if (!http_request) {
        // Ajax not supported
        return false;
    }
    params = parameters;
    params += "&ajax=yes&" + Math.random();
    http_request.open('POST', adresa, true);
    http_request.onreadystatechange = alertContents;
    http_request.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    http_request.setRequestHeader("Content-length", params.length);
    http_request.setRequestHeader("Connection", "close");
    //document.getElementById("x").innerHTML = params;
    //document.getElementById("x").innerHTML += "start ";
    http_request.send(params);
}

/*************************/
/*         FORMS         */
/*************************/
var formID = "";
var nextResendID = "";
var formIsOk = false;
var inputObjectID;

var ddlLines = "";
function Lines(s, id) {
    if (id != null)
    {
        ddlLines = id;
    }
    if (s == "0") {
        SetDisabled(document.getElementById(ddlLines));
    }
    else {
        SetEnabled(document.getElementById(ddlLines));
    }
}
 

function ShowValues(values) {
    if (values != "") {
        arrValues = values.split(';;;');
        for (i = 0; i < arrValues.length; i++) {
            if (document.getElementById(arrValues[i].split('###')[0]).value != undefined) {
                if (document.getElementById(arrValues[i].split('###')[0]).options != undefined) {
                    eval(arrValues[i].split('###')[1]);
                    Lines(1);
                }
                else
                {
                    document.getElementById(arrValues[i].split('###')[0]).value = arrValues[i].split('###')[1];
                }
            }
            else {
                alert(document.getElementById(arrValues[i].split('###')[0])).toString();
                document.getElementById(arrValues[i].split('###')[0]).innerHTML = arrValues[i].split('###')[1];
            }
        }
    }
}

function ExecuteActions(actions) {
    if (actions != "") {
        arrActions = actions.split(';');
        for (i = 0; i < arrActions.length; i++) {
            //alert(arrActions[i].split('=')[0]);
            if (arrActions[i].split('=')[1].toString() == "1") {
                SetVisible(document.getElementById(arrActions[i].split('=')[0]));
            }
            if (arrActions[i].split('=')[1].toString() == "2") {
                SetInVisible(document.getElementById(arrActions[i].split('=')[0]));
            }
        }
    }
}

// Validate
function ShowFormErrors(oks, errors) {
    // Internal variables
    var ok = true;
    arrErrors = errors.split(';');
    arrOks = oks.split(';');

    // Test, if form is ok
    for (i = 0; i < arrErrors.length; i++) {
        if (arrErrors[i].split('=')[0] != arrOks[i].split('=')[0]) {
            ok = false;
            formIsOk = false;
            break;
        }
    }
    // Validate all forms
    if (ok && action == "ValidateFormAndSend") {
        if (!formIsOk) {
            formIsOk = true;
            formIsOk = false;
        }
    }
}

// Send
function SendForm(ID, a, sender, input, preloadImage) {
    if (sender != undefined) {
        nextResendID = sender.id;
    }

    inputObjectID = "";
    if (input != undefined) {
        inputObjectID = input.id;
    }

    var queryString = "";
    // Form fields to scan

    for (i = 0; i < document.getElementById(ID).getElementsByTagName("input").length; i++) {
        if (document.getElementById(ID).getElementsByTagName("input")[i].ajax != "no") {
            if (document.getElementById(ID).getElementsByTagName("input")[i].type == "checkbox") {
                if (document.getElementById(ID).getElementsByTagName("input")[i].checked) {
                    queryString += document.getElementById(ID).getElementsByTagName("input")[i].id.replace(/_/g, '$') + "=" + "on&";
                }
                else {
                    queryString += document.getElementById(ID).getElementsByTagName("input")[i].id.replace(/_/g, '$') + "=" + "&";
                }
            }
            else {
                queryString += document.getElementById(ID).getElementsByTagName("input")[i].id.replace(/_/g, '$') + "=" + encodeURI(document.getElementById(ID).getElementsByTagName("input")[i].value) + "&";
            }
        }
    }
    for (i = 0; i < document.getElementById(ID).getElementsByTagName("select").length; i++) {
        queryString += document.getElementById(ID).getElementsByTagName("select")[i].id.replace(/_/g, '$') + "=" + encodeURI(document.getElementById(ID).getElementsByTagName("select")[i].value) + "&";
    }
    for (i = 0; i < document.getElementById(ID).getElementsByTagName("textarea").length; i++) {
        queryString += document.getElementById(ID).getElementsByTagName("textarea")[i].id.replace(/_/g, '$') + "=" + encodeURI(document.getElementById(ID).getElementsByTagName("textarea")[i].value) + "&";
    }

    action = a;
    formID = ID;

    makeRequest(ajax_url, 'action=' + action + '&formID=' + formID + '&' + queryString + '__EVENTTARGET=' + sender.id.replace(/_/g, '$') + '&' + Math.random(), a);
    return false;
}