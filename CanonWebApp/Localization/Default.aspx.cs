using System;
using System.IO;
using System.Data;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Collections.Generic;
using CanonWebApp.Code;
using CanonWebApp.Enums;
using Westwind.Globalization;
using Westwind.Web.Controls;
using Westwind.Globalization.Tools;

public partial class LocalizeAdmin_Default : BasePage
{
    #region Manager
    /// <summary>
    /// We talk directly to the Db resource manager (bus object) here rather than
    /// through the provider or resource manager, as we don't have the flexibility
    /// with the core resource managers.
    /// </summary>
    protected wwDbResourceDataManager Manager = new wwDbResourceDataManager(); 
    #endregion

    #region ResourceSet
    public string ResourceSet
    {
        get { return _ResourceSet; }
        set { _ResourceSet = value; }
    }
    private string _ResourceSet = ""; 
    #endregion

    #region Page Initialization and Data Binding routines

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        //Check if has rights to edit
        //switch (SessionManager.LoggedUser.Role.InternalID)
        //{
        //    case (int)RoleTypes.SystemAdministrator:
        //        break;
        //    default:
        //        Response.Redirect("~/Default.aspx");
        //        break;
        //}

        // *** On callbacks we don't need to populate any data since they are
        // *** raw method calls. Callback routes to parser from here
        if (Callback.IsCallback)
            return;

        Response.Expires = 0;

        if (!Manager.IsLocalizationTable(null))
        {
            ErrorDisplay.ShowError(Res("ResourceTableDoesntExist"));
            btnCreateTable.Visible = true;
            return;
        }

        GetResourceSet();

        DataTable dt = Manager.GetAllResourceIds(ResourceSet);
        if (dt == null)
        {
            ErrorDisplay.ShowError("Couldn't load resources: " + Manager.ErrorMessage);
            return;
        }

        lstResourceIds.DataSource = dt;
        lstResourceIds.DataValueField = "ResourceId";
        lstResourceIds.DataBind();

        if (lstResourceIds.Items.Count > 0)
            lstResourceIds.SelectedIndex = 0;

        dt = Manager.GetAllLocaleIds(ResourceSet);
        if (dt == null)
        {
            ErrorDisplay.ShowError("Couldn't load resources: " + Manager.ErrorMessage);
            return;
        }

        foreach (DataRow row in dt.Rows)
        {
            string Code = row["LocaleId"] as string;
            CultureInfo ci = CultureInfo.GetCultureInfo(Code.Trim());

            if (Code != "")
                row["Language"] = ci.DisplayName + " (" + ci.Name + ")";
            else
                row["Language"] = "Invariant";
        }

        lstLanguages.DataSource = dt;
        lstLanguages.DataValueField = "LocaleId";
        lstLanguages.DataTextField = "Language";
        lstLanguages.DataBind();

        if (lstLanguages.Items.Count > 0)
            lstLanguages.SelectedIndex = 0;
        else
            lstLanguages.Items.Add(new ListItem("Invariant", ""));
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // *** On callbacks we don't need to populate any data since they are
        // *** raw method calls. Callback routes to parser from here
        if (Callback.IsCallback)
            return;

        SetControlId();

        imgExportResources.Src = Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_EXTRACTRESOURCES);
        imgRefresh.Src = Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_REFRESH);
        imgRecycleApp.Src = Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_RECYCLE);

        if (btnCreateTable.Visible)
        {
            imgCreateTable.Src = Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_CREATETABLE);
            imgCreateTable.Visible = true;
        }

        imgDeleteResourceSet.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_DELETE);
        imgRenameResourceSet.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_RENAME);

        imgImport.Src = Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_IMPORT);
        imgBackup.Src = Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_BACKUP);


        // *** Check if resources are properly active if not we have a 
        //     problem and need to let user know
        if (Res("BackupComplete") == "BackupComplete")
        {
           // *** Not localized so it's always visible!!!
            ErrorDisplay.DisplayTimeout = 0;
            ErrorDisplay.ShowError("Resources are not available for this site. Most likely this means you have enabled the wwDbResourceProvider without first importing resources or that your database connection is not properly configured.<p/>" +
                                        "Please make sure you run this form without the wwDbResourceProvider enabled and ensure you have created the resource table and imported Resx resources of the site. <p />" +
                                        "For more information please check the configuration at: <p />" +
                                        "<a href='http://www.west-wind.com/tools/wwDbResourceProvider/docs'>www.west-wind.com/tools/wwDbResourceProvider/docs</a>");
        }

    }

    private void GetResourceSet()
    {
        ResourceSet = Request.Form[lstResourceSet.UniqueID];
        if (ResourceSet == null)
            ResourceSet = Request.QueryString["ResourceSet"];
        if (ResourceSet == null)
            ResourceSet = ViewState["ResourceSet"] as string;

        if (ResourceSet == null)
            ResourceSet = "";

        ResourceSet = ResourceSet.ToLower();

        if (!string.IsNullOrEmpty(ResourceSet))
            ViewState["ResourceSet"] = ResourceSet;

        // *** Clear selections
        lstResourceIds.SelectedValue = null;
        lstResourceSet.SelectedValue = null;

        DataTable dt = Manager.GetAllResourceSets(ResourceListingTypes.AllResources);
        lstResourceSet.DataSource = dt;
        lstResourceSet.DataValueField = "ResourceSet";
        lstResourceSet.DataBind();

        if (!String.IsNullOrEmpty(ResourceSet))
        {
            lstResourceSet.SelectedValue = ResourceSet;
        }

        if (!String.IsNullOrEmpty(lstResourceSet.SelectedValue))
        {
            ResourceSet = lstResourceSet.SelectedValue;
        }
    }

    private void SetControlId()
    {
        string CtlId = null;
        if (IsPostBack)
            CtlId = Request.Form[lstResourceIds.UniqueID];

        if (CtlId == null)
            CtlId = Request.QueryString["CtlId"];

        if (string.IsNullOrEmpty(CtlId))
            return;

        string Id = CtlId;

        // *** Search for .Text first
        string[] Tokens = Id.Split('.');
        if (Tokens.Length == 2)
            Id = Tokens[0] + ".Text";

        for (int x = 0; x < 2; x++)
        {
            if (x == 1)
                // *** No match for .text - find passed property
                Id = CtlId;

            foreach (ListItem li in lstResourceIds.Items)
            {
                if (li.Value.ToLower() == Id.ToLower())
                {
                    lstResourceIds.SelectedValue = Id;
                    return;
                }
            }
        }

    }
    #endregion

    #region Page Event Handlers - only a few of them - most calls are Ajax Callbacks
  

    protected void btnFileUpload_Click(object sender, EventArgs e)
    {
        if (!FileUpload.HasFile)
            return;

        //FileInfo fi = new FileInfo(FileUpload.FileName);
        string Extension = Path.GetExtension(FileUpload.FileName).TrimStart('.');  // fi.Extension.TrimStart('.');

        string Filter = ",bmp,ico,gif,jpg,png,css,js,txt,wav,mp3,";
        if (Filter.IndexOf("," + Extension + ",") == -1)
        {
            ErrorDisplay.ShowError(Res("InvalidFileUploaded"));
            return;
        }

        string FilePath = Server.MapPath(FileUpload.FileName);

        File.WriteAllBytes(FilePath, FileUpload.FileBytes);

        string ResourceId = txtNewResourceId.Text;

        // *** Try to add the file
        wwDbResourceDataManager Data = new wwDbResourceDataManager();
        if (Data.UpdateOrAdd(ResourceId, FilePath, txtNewLanguage.Text, ResourceSet, true) == -1)
            ErrorDisplay.ShowError(Res("ResourceUpdateFailed") + "<br/>" + Data.ErrorMessage);
        else
            ErrorDisplay.ShowMessage(Res("ResourceUpdated"));

        File.Delete(FilePath);

        lstResourceIds.Items.Add(ResourceId);
        lstResourceIds.SelectedValue = ResourceId;
    }


    protected void btnCreateTable_Click(object sender, EventArgs e)
    {
        if (!Manager.CreateLocalizationTable(null))
            ErrorDisplay.ShowError(Res("LocalizationTableNotCreated") + "<br />" + Manager.ErrorMessage);
        else
            ErrorDisplay.ShowMessage(Res("LocalizationTableCreated"));
    }


    protected void btnExportResources_Click(object sender, EventArgs e)
    {
        wwDbResXConverter Exporter = new wwDbResXConverter(Context.Request.PhysicalApplicationPath);

        if (!Exporter.GenerateLocalResourceResXFiles())
        {
            ErrorDisplay.ShowError(Res("ResourceGenerationFailed"));
            return;
        }
        if (!Exporter.GenerateGlobalResourceResXFiles())
        {
            ErrorDisplay.ShowError(Res("ResourceGenerationFailed"));
            return;
        }

        ErrorDisplay.ShowMessage(Res("ResourceGenerationComplete"));
    }

    protected void btnImport_Click(object sender, EventArgs e)
    {
        wwDbResXConverter Converter = new wwDbResXConverter(Context.Request.PhysicalApplicationPath);
        Converter.ImportWebResources();
        ErrorDisplay.ShowMessage(Res("ResourceImportComplete"));

        lstResourceIds.SelectedValue = null;
        GetResourceSet();
    }

    protected void btnRenameResourceSet_Click(object sender, EventArgs e)
    {   
        if (!Manager.RenameResourceSet(txtOldResourceSet.Text, txtRenamedResourceSet.Text))
            ErrorDisplay.ShowError(Manager.ErrorMessage);
        else
        {
            // *** Force the selected value to be set
            lstResourceSet.Items.Add(new ListItem("", txtRenamedResourceSet.Text.ToLower()));
            lstResourceSet.SelectedValue = txtRenamedResourceSet.Text.ToLower();

            //lstResourceSet.SelectedValue = string.Empty;   // null; 

            // *** Refresh and reset the resource list
            GetResourceSet();

            ErrorDisplay.ShowMessage(Res("ResourceSetRenamed"));
        }
    }
    #endregion

    #region Ajax Callback Methods
    [CallbackMethod]
    public DataTable GetResourceList(string ResourceSet)
    {
        DataTable dt = Manager.GetAllResourceIds(ResourceSet);
        if (Manager == null)
            throw new ApplicationException(Res("ResourceSetLoadingFailed") + ":" + Manager.ErrorMessage);

        return dt;
    }

    [CallbackMethod]
    public string GetResourceString(string ResourceId, string ResourceSet, string CultureName)
    {
        string Value = Manager.GetResourceString(ResourceId, ResourceSet, CultureName);

        if (Value == null && !string.IsNullOrEmpty(Manager.ErrorMessage))
            throw new ArgumentException(Manager.ErrorMessage);

        return Value;
    }

    [CallbackMethod]
    public Dictionary<string, string> GetResourceStrings(string ResourceId, string ResourceSet)
    {
        Dictionary<string, string> Resources = Manager.GetResourceStrings(ResourceId, ResourceSet);

        if (Resources == null)
            throw new ApplicationException(Manager.ErrorMessage);

        return Resources;
    }


    [CallbackMethod]
    public bool UpdateResourceString(string Value, string ResourceId, string ResourceSet, string LocaleId)
    {
        if (Manager.UpdateOrAdd(ResourceId, Value, LocaleId, ResourceSet) == -1)
            return false;

        wwDbResourceConfiguration.ClearResourceCache();
        return true;
    }

    [CallbackMethod]
    public bool DeleteResource(string ResourceId, string ResourceSet, string LocaleId)
    {
        if (!Manager.DeleteResource(ResourceId, LocaleId, ResourceSet))
            throw new ApplicationException(Res("ResourceUpdateFailed") + ": " + Manager.ErrorMessage);

        return true;
    }

    [CallbackMethod]
    public bool RenameResource(string ResourceId, string NewResourceId, string ResourceSet)
    {
        if (!Manager.RenameResource(ResourceId, NewResourceId, ResourceSet))
            throw new ApplicationException(Res("InvalidResourceId"));

        return true;
    }

    /// <summary>
    /// Renames all resource keys that match a property (ie. lblName.Text, lblName.ToolTip)
    /// at once. This is useful if you decide to rename a meta:resourcekey in the ASP.NET
    /// markup.
    /// </summary>
    /// <param name="Property">Original property prefix</param>
    /// <param name="NewProperty">New Property prefix</param>
    /// <param name="ResourceSet">The resourceset it applies to</param>
    /// <returns></returns>
    [CallbackMethod]
    public bool RenameResourceProperty(string Property, string NewProperty, string ResourceSet)
    {
        if (!Manager.RenameResourceProperty(Property, NewProperty, ResourceSet))
            throw new ApplicationException(Res("InvalidResourceId"));

        return true;

    }

    [CallbackMethod]
    public string Translate(string Text, string From, string To, string Service)
    {
        Service = Service.ToLower();

        TranslationServices Translate = new TranslationServices();
        Translate.TimeoutSeconds = 10;

        string Result = null;
        if (Service == "google")
            Result = Translate.TranslateGoogle(Text, From, To);
        else if (Service == "babelfish")
            Result = Translate.TranslateBabelFish(Text, From, To);

        if (Result == null)
            Result = Translate.ErrorMessage;

        return Result;
    }

    [CallbackMethod]
    public bool DeleteResourceSet(string ResourceSet)
    {
        return Manager.DeleteResourceSet(ResourceSet);
    }

    [CallbackMethod]
    public bool RenameResourceSet(string OldResourceSet, string NewResourceSet)
    {
        return Manager.RenameResourceSet(OldResourceSet, NewResourceSet);
    }

    [CallbackMethod]
    public void ReloadResources()
    {
        //Westwind.Globalization.Tools.wwWebUtils.RestartWebApplication();
        wwDbResourceConfiguration.ClearResourceCache();
    }

    [CallbackMethod]
    public bool Backup()
    {
        return Manager.CreateBackupTable(null);
    }

    #endregion

    #region Localization Helper Functions

    /// <summary>
    /// Local Resource Help Function for easier syntax
    /// </summary>
    /// <param name="ResourceKey"></param>
    /// <param name="ResourceSet"></param>
    /// <returns></returns>
    public string Res(string ResourceKey)
    {
        string Value = GetLocalResourceObject(ResourceKey) as string;
        if (Value == null)
            return ResourceKey;

        return Value;
    }

    /// <summary>
    /// Returns a global Resource Key
    /// </summary>
    /// <param name="ResourceKey"></param>
    /// <returns></returns>   
    public string GRes(string ResourceKey, string ResourceSet)
    {
        string Value = GetGlobalResourceObject(ResourceSet, ResourceKey) as string;
        if (Value == null)
            return ResourceKey;

        return Value;
    }

    /// <summary>
    /// Creates a client side compatible string including the quote characters
    /// from a local resource key.
    /// 
    /// This simplifies adding values to client script with code like this:
    /// 
    /// &lt;%= ResC("ResourceID") %&gt;
    /// </summary>
    /// <param name="ResourceKey"></param>
    /// <returns></returns>
    public string ResC(string ResourceKey)
    {
        string Value = Res(ResourceKey);
        return wwWebUtils.EncodeJsString(Value);
    }

    /// <summary>
    /// returns a properly encoded JavaScript string for a global resource
    /// </summary>
    /// <param name="ResourceKey"></param>
    /// <param name="ResourceKey"></param>
    /// <returns></returns>
    public string GResC(string ResourceKey, string ResourceSet)
    {
        string Value = GRes(ResourceKey, ResourceSet);
        return wwWebUtils.EncodeJsString(Value);
    }

    #endregion
}