using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Web.ASPxGridView.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Web.ASPxClasses.Localization;
using DevExpress.Web.ASPxEditors.Localization;

namespace CanonWebApp.Controls
{
    public class MyGridLocalizer : DevExpress.Web.ASPxGridView.Localization.ASPxGridViewResLocalizer
    {
        public static void Activate()
        {
            MyGridLocalizer localizer = new MyGridLocalizer();
            DefaultActiveLocalizerProvider<ASPxGridViewStringId> provider = new DefaultActiveLocalizerProvider<ASPxGridViewStringId>(localizer);
            MyGridLocalizer.SetActiveLocalizerProvider(provider);
        }

        public override string GetLocalizedString(ASPxGridViewStringId id)
        {
            switch (id)
            {
                case ASPxGridViewStringId.EmptyDataRow:
                    return "Žádná data k zobrazení";
                case ASPxGridViewStringId.CommandCancel:
                    return "Zrušit";
                case ASPxGridViewStringId.CommandEdit:
                    return "Editovat";
                case ASPxGridViewStringId.CommandDelete:
                    return "Smazat";
                case ASPxGridViewStringId.CommandClearFilter:
                    return "Zrušit";
                default:
                    return base.GetLocalizedString(id);
            }
        }
    }

    public class MyEditorsLocalizer : DevExpress.Web.ASPxEditors.Localization.ASPxEditorsResLocalizer
    {
        public static void Activate()
        {
            MyEditorsLocalizer localizer = new MyEditorsLocalizer();
            DefaultActiveLocalizerProvider<ASPxEditorsStringId> provider = new DefaultActiveLocalizerProvider<ASPxEditorsStringId>(localizer);
            MyEditorsLocalizer.SetActiveLocalizerProvider(provider);

        }
        public override string GetLocalizedString(ASPxEditorsStringId id)
        {
            if (id == ASPxEditorsStringId.FilterControl_Cancel)
                return "Zrušit";

            if (id == ASPxEditorsStringId.FilterControl_ClearFilter)
                return "Zrušit";

            return base.GetLocalizedString(id);
        }
    }


    public class MyWebLocalizer : ASPxperienceResLocalizer 
    {
        public static void Activate()
        {
            MyWebLocalizer localizer = new MyWebLocalizer();
            DefaultActiveLocalizerProvider<ASPxperienceStringId> provider = new DefaultActiveLocalizerProvider<ASPxperienceStringId>(localizer);
            MyWebLocalizer.SetActiveLocalizerProvider(provider);
        }

        public override string GetLocalizedString(ASPxperienceStringId id)
        {
            if (id == ASPxperienceStringId.UploadControl_AddButton)
                return "Přidat další soubor";

            if (id == ASPxperienceStringId.UploadControl_RemoveButton)
                return "Odstranit";

            return base.GetLocalizedString(id);
        }
    }
}
