using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using CanonWebApp.Controls;

/// <summary>
/// Victory Page Base class.
/// </summary>
public class VictoryPage : System.Web.UI.Page
{
    #region ViewState

    protected override void SavePageStateToPersistenceMedium(object state)
    {
        Session[this.UniqueID + "VS"] = state;
    }

    protected override object LoadPageStateFromPersistenceMedium()
    {
        return Session[this.UniqueID + "VS"];
    }

    #endregion

    public enum ControlActionType
    {
        Show = 1,
        Hide = 2,
        FadeIn = 3,
        FadeOut = 4
    }

    #region Ajax Validation

    // Validation states
    public enum ControlState
    {
        Valid,
        Invalid
    }

    /// <summary>
    /// Ajax Control Validation
    /// </summary>
    class ControlValidation
    {
        private Control _validatedControl;

        public Control ValidatedControl
        {
            get { return _validatedControl; }
            set { _validatedControl = value; }
        }

        private ControlState _state;

        public ControlState State
        {
            get { return _state; }
            set { _state = value; }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public ControlValidation(Control control, string message, ControlState state)
        {
            _validatedControl = control;
            _message = message;
            _state = state;
        }
    }

    // Controls collection
    Dictionary<string, ControlValidation> _controlValidations = new Dictionary<string, ControlValidation>();

    /// <summary>
    /// "Marks" control valid
    /// </summary>
    /// <param name="control"></param>
    /// <param name="message"></param>
    public void ValidControl(Control control, string message)
    {
        SetControlState(new ControlValidation(control, message, ControlState.Valid));
    }

    /// <summary>
    /// "Marks" control invalid
    /// </summary>
    /// <param name="control"></param>
    /// <param name="message"></param>
    public void InValidControl(Control control, string message)
    {
        SetControlState(new ControlValidation(control, message, ControlState.Invalid));
    }

    /// <summary>
    /// Sets control validation state
    /// </summary>
    /// <param name="cv"></param>
    void SetControlState(ControlValidation cv)
    {
        if (!_controlValidations.ContainsKey(cv.ValidatedControl.ClientID))
        {
            _controlValidations.Add(cv.ValidatedControl.ClientID, cv);
        }
        else
        {
            _controlValidations[cv.ValidatedControl.ClientID] = cv;
        }
    }

    /// <summary>
    /// Generates Ajax validation string
    /// </summary>
    /// <returns></returns>
    public string ValidationString()
    {
        StringBuilder errors = new StringBuilder();
        StringBuilder oks = new StringBuilder();
        int i = 0;
        foreach (ControlValidation cv in _controlValidations.Values)
        {
            i++;
            // Errors
            errors.Append(cv.ValidatedControl.ClientID);
            errors.Append("=");
            errors.Append(cv.Message);
            if (i < _controlValidations.Count)
            {
                errors.Append(";");
            }
            // Oks
            if (cv.State == ControlState.Valid)
            {
                oks.Append(cv.ValidatedControl.ClientID);
            }
            if (i < _controlValidations.Count)
            {
                oks.Append(";");
            }
        }
        return string.Concat(errors.ToString(), "|", oks.ToString());
    }

    /// <summary>
    /// Generates Ajax validation string
    /// </summary>
    /// <returns></returns>
    public string ValidationStringAlert()
    {
        StringBuilder errors = new StringBuilder();
        StringBuilder oks = new StringBuilder();
        int i = 0;
        foreach (ControlValidation cv in _controlValidations.Values)
        {
            i++;
            // Errors
            errors.Append(cv.ValidatedControl.ClientID);
            errors.Append("=");
            errors.Append(cv.Message);
            if (i < _controlValidations.Count)
            {
                errors.Append(";");
            }
            // Oks
            if (cv.State == ControlState.Valid)
            {
                oks.Append(cv.ValidatedControl.ClientID);
            }
            if (i < _controlValidations.Count)
            {
                oks.Append(";");
            }
        }
        return string.Concat(errors.ToString(), "|", oks.ToString());
    }

    /// <summary>
    /// Are controls valid?
    /// </summary>
    public bool IsAjaxValid
    {
        get
        {
            foreach (ControlValidation cv in _controlValidations.Values)
            {
                if (cv.State == ControlState.Invalid)
                {
                    return false;
                }
            }
            return true;
        }
    }

    #endregion

    #region Ajax Values

    // Controls collection
    Dictionary<string, ControlValue> _controlValues = new Dictionary<string, ControlValue>();

    /// <summary>
    /// Ajax Callback values
    /// </summary>
    class ControlValue
    {
        private Control _valueControl;

        public Control ValueControl
        {
            get { return _valueControl; }
            set { _valueControl = value; }
        }

        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public ControlValue(Control control, string value)
        {
            _valueControl = control;
            _value = value;
        }
    }

    /// <summary>
    /// Sets control value
    /// </summary>
    /// <param name="control"></param>
    /// <param name="message"></param>
    void SetControlValue(ControlValue cv)
    {
        if (!_controlValues.ContainsKey(cv.ValueControl.ClientID))
        {
            _controlValues.Add(cv.ValueControl.ClientID, cv);
        }
        else
        {
            _controlValues[cv.ValueControl.ClientID] = cv;
        }
    }

    /// <summary>
    /// Value to control
    /// </summary>
    /// <param name="control"></param>
    /// <param name="message"></param>
    public void ValueControl(Control control, string value)
    {
        SetControlValue(new ControlValue(control, value));
    }

    /// <summary>
    /// Value to control
    /// </summary>
    /// <param name="control"></param>
    /// <param name="message"></param>
    public void ValueControl(Control control, Control value)
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        HtmlTextWriter htw = new HtmlTextWriter(sw);
        value.RenderControl(htw);
        SetControlValue(new ControlValue(control, sb.ToString()));
    }

    /// <summary>
    /// Generates Ajax callback values string
    /// </summary>
    /// <returns></returns>
    public string CallbackValuesString()
    {
        StringBuilder errors = new StringBuilder();
        StringBuilder oks = new StringBuilder();
        int i = 0;
        foreach (ControlValidation cv in _controlValidations.Values)
        {
            i++;
            // Errors
            errors.Append(cv.ValidatedControl.ClientID);
            errors.Append("=");
            errors.Append(cv.Message);
            if (i < _controlValidations.Count)
            {
                errors.Append(";");
            }
            // Oks
            if (cv.State == ControlState.Valid)
            {
                oks.Append(cv.ValidatedControl.ClientID);
            }
            if (i < _controlValidations.Count)
            {
                oks.Append(";");
            }
        }
        return string.Concat(errors.ToString(), "|", oks.ToString());
    }

    /// <summary>
    /// Generates Ajax callback values string
    /// </summary>
    /// <returns></returns>
    public string ValueString()
    {
        StringBuilder ret = new StringBuilder();
        int i = 0;
        foreach (ControlValue cv in _controlValues.Values)
        {
            i++;
            // Errors
            ret.Append(cv.ValueControl.ClientID);
            ret.Append("###");
            ret.Append(cv.Value);
            if (i < _controlValues.Count)
            {
                ret.Append(";;;");
            }
        }
        return ret.ToString();
    }

    public string ValueString2()
    {
        StringBuilder ret = new StringBuilder();
        int i = 0;
        foreach (Control cv in _bindedControls)
        {
            i++;
            // Errors
//            if (cv is DropDownList || !(cv is ListBox))
            if (cv is DropDownList || cv is ListBox)
            {
                ret.Append(cv.ClientID);
            }
            else
            {
                ret.Append(cv.ID);
            }
            ret.Append("###");
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            if (cv is DropDownList)
            {
                ret.Append(CreateDropDownList((DropDownList)cv));
            }
            else if (cv is ListBox)
            {
                ret.Append(CreateListBox((ListBox)cv));
            }
            else
            {
                cv.RenderControl(htw);
            }
            ret.Append(sb.ToString());
            if (i < _bindedControls.Count)
            {
                ret.Append(";;;");
            }
        }
        if (_bindedControls.Count == 1 && _controlValues.Count > 0)
        {
            ret.Append(";;;");
        }
        ret.Append(ValueString());
        return ret.ToString();
    }

    #endregion

    #region AjaxActions

    // Controls collection
    Dictionary<string, ControlAction> _controlActions = new Dictionary<string, ControlAction>();

    /// <summary>
    /// Ajax Callback values
    /// </summary>
    class ControlAction
    {
        private string _actionControl;

        public string ActionControl
        {
            get { return _actionControl; }
            set { _actionControl = value; }
        }

        private ControlActionType _action;

        public ControlActionType Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public ControlAction(string control, ControlActionType action)
        {
            _actionControl = control;
            _action = action;
        }
    }

    /// <summary>
    /// Sets control value
    /// </summary>
    /// <param name="control"></param>
    /// <param name="message"></param>
    void SetControlAction(ControlAction ca)
    {
        if (!_controlActions.ContainsKey(ca.ActionControl))
        {
            _controlActions.Add(ca.ActionControl, ca);
        }
        else
        {
            _controlActions[ca.ActionControl] = ca;
        }
    }

    /// <summary>
    /// Value to control
    /// </summary>
    /// <param name="control"></param>
    /// <param name="message"></param>
    public void ActionControl(string control, ControlActionType action)
    {
        SetControlAction(new ControlAction(control, action));
    }

    /// <summary>
    /// Generates Ajax callback values string
    /// </summary>
    /// <returns></returns>
    public string ActionsString()
    {
        StringBuilder ret = new StringBuilder();
        int i = 0;
        foreach (ControlAction ca in _controlActions.Values)
        {
            i++;
            // Errors
            ret.Append(ca.ActionControl);
            ret.Append("=");
            ret.Append((int)ca.Action);
            if (i < _controlActions.Count)
            {
                ret.Append(";");
            }
        }
        return ret.ToString();
    }

    #endregion

    // Ajax Content control ID
    private string _ajaxContentID;

    /// <summary>
    /// Ajax Content control ID
    /// </summary>
    public string AjaxContentID
    {
        get { return _ajaxContentID; }
        set { _ajaxContentID = value; }
    }

    // Is in Ajax Mode?
    private bool _isAjax;
    /// <summary>
    /// Is in Ajax Mode?
    /// </summary>
    public bool IsAjax
    {
        get { return _isAjax; }
        set { _isAjax = value; }
    }

    /// <summary>
    /// Finds control in the full tree.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="c"></param>
    /// <returns>Found control. if not found then null.</returns>
    public Control FindControl(string id, Control c)
    {
        Control ret = c;
        if (!string.IsNullOrEmpty(c.ID))
        {
            if (c.ID == id)
            {
                return c;
            }
        }
        foreach (Control c2 in c.Controls)
        {
            if (ret.ID != id)
            {
                ret = FindControl(id, c2);
            }
        }
        return ret;
    }

    /// <summary>
    /// Init.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["ajax"]))
        {
            if (Request["ajax"].Equals("yes"))
            {
                IsAjax = true;
            }
        }
        base.OnPreRender(e);
    }

    #region Trace
    
    DateTime lastdate = DateTime.Now;
    public void TraceIt(string s)
    {
        //if (!IsAjax)
        //{
        //    Response.Write(s + " - " + DateTime.Now.Subtract(lastdate).TotalMilliseconds);
        //    Response.Write("<br>");
        //    lastdate = DateTime.Now;
        //}
    }
    
    #endregion

    /// <summary>
    /// PreRender.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        if (IsAjax && !(string.IsNullOrEmpty(_ajaxContentID)))
        {
            ControlVisibility("", this.Page.Form);
            this.Header.Visible = false;
            Control c = FindControl(_ajaxContentID, this.Master);
            this.Form.Controls.Clear();
            this.Form.Controls.Add(c);
        }

        TraceIt("Pre render end");

        base.OnPreRender(e);
    }

    List<string> _dropDownLists = new List<string>();

    public string DropDownLists()
    {
        string ret = "";
        foreach (string s in _dropDownLists)
        {
            ret += s + ";";
        }
        return ret;
    }

    string CreateDropDownList(DropDownList ddl)
    {
        string s = "";
        foreach (ListItem li in ddl.Items)
        {
            s += li.Text;
            s += "===";
            s += li.Value;
            s += "===";
            s += li.Selected.ToString();
            s += "===";
            s += li.Enabled.ToString();
            s += "$$$";
        }
        s = s.Length > 0 ? s.Substring(0, s.Length - 3) : "";
        return string.Format("GenerateSelect(\"{0}\", \"{1}\");", ddl.ClientID, s);
    }

    string CreateListBox(ListBox ddl)
    {
        string s = "";
        foreach (ListItem li in ddl.Items)
        {
            s += li.Text;
            s += "===";
            s += li.Value;
            s += "===";
            s += li.Selected.ToString();
            s += "===";
            s += li.Enabled.ToString();
            s += "$$$";
        }
        s = s.Length > 0 ? s.Substring(0, s.Length - 3) : "";
        return string.Format("GenerateSelect(\"{0}\", \"{1}\");", ddl.ClientID, s);
    }

    /// <summary>
    /// Rendering.
    /// </summary>
    /// <param name="writer"></param>
    protected override void Render(HtmlTextWriter writer)
    {
        TraceIt("Render start");
        if (IsAjax)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            base.Render(htw);
            Regex r = new Regex("<form.*>");
            Regex r2 = new Regex("<input type=\"hidden\" name=\"__VIEWSTATE\".*>");
            Regex r3 = new Regex("<!DOCTYPE.*script>");
            string sout = r.Replace(sb.ToString().Replace("</form>", ""), "");
            sout = sb.ToString();
            sout = sout.Replace("<html xmlns=\"http://www.w3.org/1999/xhtml\">", "");
            sout = sout.Replace("</html>", "");
            sout = sout.Replace("<body>", "");
            sout = sout.Replace("</body>", "");
            sout = r2.Replace(sout, "");
            sout = r3.Replace(sout.Replace("\n", ""), "");
            writer.Write(sout);
        }
        else
        {
            TraceIt("Else start");

            ControlTree("", this);
            //Response.End();
            int i = 0;
            foreach (string s in _rl)
            {
                Control c = FindControl(s, this);
                Control cin = FindControl(_rid[i], this);

                if (!(c is MultiView) && !(cin is DropDownList) && !(cin is ListBox) & !(cin is SearchBox))
                {
                    c.Controls.AddAt(_ri[i], new LiteralControl("<div id=\"" + _rid[i] + "\"></div>"));
                }
                if (cin is DropDownList)
                {
                    _dropDownLists.Add(CreateDropDownList((DropDownList)cin));
                    ((DropDownList)cin).Items.Clear();
                }
                if (cin is ListBox)
                {
                    _dropDownLists.Add(CreateListBox((ListBox)cin));
                    ((ListBox)cin).Items.Clear();
                }

                Control ci = FindControl(_rid[i], this);
                if (ci is MultiView)
                {
                    MultiView mv = (MultiView)ci;
                    int k = mv.ActiveViewIndex;
                    int j = 0;
                    foreach (View v in mv.Views)
                    {
                        mv.ActiveViewIndex = j;
                        StringBuilder sb = new StringBuilder();
                        StringWriter sw = new StringWriter(sb);
                        HtmlTextWriter htw = new HtmlTextWriter(sw);
                        v.RenderControl(htw);
                        string disp = "style=\"display:none;\" ";
                        if (j == k)
                        {
                            disp = "";
                        }
                        string inners = "";
                        int x = 0;
                        foreach (string s2 in _rl)
                        {
                            Control c2 = FindControl(s2, this);
                            if (c2 is View)
                            {
                                if (c2.ID == v.ID)
                                {
                                    inners += "<div id=\"" + _rid[x] + "\"></div>";
                                }
                            }
                            x++;
                        }
                        c.Controls.AddAt(_ri[i], new LiteralControl("<div " + disp + "id=\"" + v.ID + "\">" + sb.ToString() + inners + "</div>"));
                        j++;
                    }
                    mv.ActiveViewIndex = -1;
                }
                i++;
            }

            StringBuilder js = new StringBuilder();
            js.AppendLine("<script language=\"javascript\">");
            js.AppendLine(DropDownLists());
            js.AppendLine("</script>");
            Form.Controls.AddAt(Form.Controls.Count - 1, new LiteralControl(js.ToString()));

            Application["JBR"] = "1";

            TraceIt("Render end");

            base.Render(writer);
        }
        //TraceIt();
    }

    int c_index = 0;
    List<string> _rl = new List<string>();
    List<string> _rid = new List<string>();
    List<int> _ri = new List<int>();

    public Control ControlTree(string id, Control c)
    {
        Control ret = c;
        c_index++;
        if (!string.IsNullOrEmpty(c.ID))
        {
            if (c is Repeater || c is BulletedList || c is GridView || c is MultiView || c is DropDownList || c is ListBox || c is SearchBox)
            {
                _rl.Add(c.Parent.ID);
                _rid.Add(c.ID);
                _ri.Add(c_index);
            }
            if (c is DropDownList)
            {

            }
        }
        if (c.Controls.Count > 0)
        {
            c_index = 0;
        }
        foreach (Control c2 in c.Controls)
        {
            if (ret.ID != id)
            {
                ret = ControlTree(id, c2);
            }
        }
        return ret;
    }

    /// <summary>
    /// Sets control visibility
    /// </summary>
    /// <param name="id">Control ID</param>
    /// <param name="c">Control</param>
    /// <returns></returns>
    public Control ControlVisibility(string id, Control c)
    {
        Control ret = c;
        if (!string.IsNullOrEmpty(c.ID))
        {
            if (c is Panel || c is HtmlGenericControl || c is TextBox || c is LinkButton || c is ListBox)
            {
                if (c.Visible)
                {
                    ActionControl(c.ClientID, ControlActionType.Show);
                    if (c is DropDownList)
                    {
                        ((DropDownList)c).Attributes.Add("style", "display: block;");
                    }
                }
                else
                {
                    ActionControl(c.ClientID, ControlActionType.Hide);
                    if (c is DropDownList)
                    {
                        ((DropDownList)c).Attributes.Add("style", "display: none;");
                    }
                }
            }
            else if (c is Repeater)
            {
                if (c.Visible)
                {
                    ActionControl(c.ID, ControlActionType.Show);
                }
                else
                {
                    ActionControl(c.ID, ControlActionType.Hide);
                }
            }
        }
        foreach (Control c2 in c.Controls)
        {
            if (ret.ID != id)
            {
                ret = ControlVisibility(id, c2);
            }
        }
        return ret;
    }

    /// <summary>
    /// Sets data source to the control
    /// </summary>
    /// <param name="c">Control</param>
    /// <param name="ds">DataSource</param>
    public void DataSource(Control c, object ds)
    {
        if (c is Repeater)
        {
            ((Repeater)c).DataSource = ds;
        }
        if (c is BulletedList)
        {
            ((BulletedList)c).DataSource = ds;
        }
        if (c is GridView)
        {
            ((GridView)c).DataSource = ds;
        }
        if (c is DropDownList)
        {
            ((DropDownList)c).DataSource = ds;
        }
        if (c is ListBox)
        {
            ((ListBox)c).DataSource = ds;
        }
    }

    List<Control> _bindedControls = new List<Control>();

    /// <summary>
    /// Binds all data to the control
    /// </summary>
    /// <param name="c">Control</param>
    public void DataBind(Control c)
    {
        if (c is MultiView)
        {
            MultiView mv = (MultiView)c;
            foreach (View v in mv.Views)
            {
                ActionControl(v.ID, ControlActionType.Hide);
            }
            ActionControl(mv.Views[mv.ActiveViewIndex].ID, ControlActionType.Show);
        }
        else
        {
            _bindedControls.Add(c);
            c.DataBind();
        }
    }

    /// <summary>
    /// Sets text of this controls: TextBox, Label
    /// </summary>
    /// <param name="c">Control</param>
    /// <param name="s">Text to display</param>
    public void Text(Control c, string s)
    {
        if (c is TextBox)
        {
            ((TextBox)c).Text = s;
        }
        if (c is Label)
        {
            ((Label)c).Text = s;
        }
        ValueControl(c, new LiteralControl(s));
    }

    /// <summary>
    /// Very important
    /// </summary>
    /// <param name="control"></param>
    public override void VerifyRenderingInServerForm(Control control)
    {
        return;
    }
}
