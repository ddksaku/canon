var myFunctionRef = function myFunction(x, y, z, e) {
    var o = document.getElementById(z);
    var r = o.parentNode.parentNode;
    if (o.checked) {
        eval("arr_" + y + " = " + "arr_" + y + " + '" + x + "|';");
        r.className = "sel";
    }
    else {
        r.className = "";
        eval("arr_" + y + " = " + "arr_" + y + ".replace('|" + x + "|', '|');");
    }
    eval("document.getElementById('" + y.replace('_canvas', '') + "_hf').value = " + "arr_" + y + ";");
}

function AddResultsRow(id, s, v, sv, ch) {
    var t = GetTable(id);
    var rc = t.rows.length;
    rc = 0;
    var r = t.tBodies[0].insertRow(rc);

    var th = document.createElement('th');
    r.appendChild(th);
    var e1 = document.createElement("input");
    e1.type = "checkbox";
    e1.name = id;
    e1.ajax = "no";
    e1.id = id + "__" + v;

    var xx;
    eval("xx = arr_" + id + ";");
    if (xx.indexOf("|" + v + "|") > -1) {
        e1.setAttribute('value', 1);
        e1.setAttribute("checked", true);
        e1.checked = "checked";
        e1.defaultChecked = true;
        r.className = "sel";
    }
    XBrowserAddHandler(e1, 'click', function()
    { myFunctionRef(v, id, e1.id) });

    th.appendChild(e1);
    var c2 = document.createElement('td');
    var lt = document.createTextNode(s);
    c2.appendChild(lt);
    r.appendChild(c2);
}

function GetTable(id) {
    return getElementsByClassName(document.getElementById(id), "table", "dropdown")[0];
}

function ClearResultsRows(id) {
    var t = GetTable(id).tBodies[0];
    while (t.hasChildNodes()) {
        t.removeChild(t.firstChild);
    }
}

function SearchArray(a, a2, v, id, sv) {
    if (v.value.length < 3) {
        //GetTable(id).style.display = "none";
        //ClearResultsRows(id);
        return;
    }
    ClearResultsRows(id);
    for (i = a.length - 1; i >= 0; i--) {
        if (a[i].toString().toLowerCase().search(v.value.toLowerCase()) > -1) {
            AddResultsRow(id, a[i].toString(), a2[i].toString(), sv);
        }
    }
    GetTable(id).style.display = "table";
}

function InitSearchBox(a, a2, v, id, sv) {
    var limit = 50;
    var max = a.length;
    if (max > limit) {
        max = limit;
        }
    for (i = max-1; i >= 0; i--) {
        AddResultsRow(id, a[i].toString(), a2[i].toString(), sv);
    }
}