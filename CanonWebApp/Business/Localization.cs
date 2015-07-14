using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Westwind.Globalization;
using Canon.Data;

namespace CanonWebApp.Business
{
    /// <summary>
    /// Localization business object class.
    /// </summary>
    public class Localization
    {
        #region GetUntranslatedTextsList
        /// <summary>
        /// Get untranslated texts list.
        /// </summary>
        public static IQueryable GetUntranslatedTextsList()
        {
            CanonDataContext db = Cdb.Instance;

            var resourceIdCounts = (from l in db.Localizations
                                    select new
                                    {
                                        l.ResourceSet,
                                        l.ResourceId,
                                        Count = (from lall in db.Localizations
                                                 where lall.ResourceId == l.ResourceId &&
                                                 lall.ResourceSet == l.ResourceSet
                                                 select lall).Count()
                                    }).Distinct();

            var untranslatedTexts = from rc in resourceIdCounts
                                    where rc.Count != Language.SupportedLanguages.Count
                                    select rc;

            return untranslatedTexts;
        }
        #endregion

        #region GetAllLocaleResources
        /// <summary>
        /// Get all locale resources.
        /// </summary>
        public static List<Canon.Data.Localization> GetAllLocaleResources(string locale)
        {
            CanonDataContext db = Cdb.Instance;

            var resources = from l in db.Localizations
                            where l.LocaleId == locale
                            select l;

            return resources.ToList();
        }
        #endregion


        #region FillUntranslatedResources
        /// <summary>
        /// Fill all untranslated resources for set locales with default values.
        /// </summary>
        /// <param name="locales"></param>
        public static void FillUntranslatedResources(params string[] locales)
        {
            List<Canon.Data.Localization> defaultResources = GetAllLocaleResources("");

            foreach (string locale in locales)
            {
                List<Canon.Data.Localization> localeResources = GetAllLocaleResources(locale);

                foreach (Canon.Data.Localization defaultResource in defaultResources)
                {
                    FillResource(defaultResource, localeResources, locale);
                }
            }

            wwDbResourceConfiguration.ClearResourceCache();
        }
        #endregion

        #region FillResource
        /// <summary>
        /// Fill untranslated resource.
        /// </summary>
        /// <param name="defaultResource"></param>
        /// <param name="localeResources"></param>
        /// <param name="locale"></param>
        protected static void FillResource(Canon.Data.Localization defaultResource, List<Canon.Data.Localization> localeResources, string locale)
        {
            var localeResource = from lr in localeResources
                                 where lr.ResourceSet == defaultResource.ResourceSet &&
                                 lr.ResourceId == defaultResource.ResourceId
                                 select lr;

            if (localeResource.Count() == 0)
            {
                Canon.Data.Localization localization = new Canon.Data.Localization();
                localization.ResourceId = defaultResource.ResourceId;
                localization.Value = defaultResource.Value;
                localization.LocaleId = locale;
                localization.ResourceSet = defaultResource.ResourceSet;
                localization.Type = defaultResource.Type;
                localization.BinFile = defaultResource.BinFile;
                localization.TextFile = defaultResource.TextFile;
                localization.Filename = defaultResource.Filename;

                InsertLocalization(localization);
            }
            else
            {
                Canon.Data.Localization localization = localeResource.First();

                if (localization.Value != String.Empty)
                {
                    return;
                }

                localization.Value = defaultResource.Value;

                UpdateLocalization(localization);
            }
        }
        #endregion

        
        #region InsertLocalization
        /// <summary>
        /// Insert new localization.
        /// </summary>
        /// <param name="localization"></param>
        protected static void InsertLocalization(Canon.Data.Localization localization)
        {
            CanonDataContext db = Cdb.Instance;

            db.Localizations.InsertOnSubmit(localization);
            db.SubmitChanges();
        } 
        #endregion

        #region UpdateLocalization
        /// <summary>
        /// Update localization.
        /// </summary>
        /// <param name="localization"></param>
        protected static void UpdateLocalization(Canon.Data.Localization localization)
        {
            CanonDataContext db = Cdb.Instance;

            if (db.Localizations.GetOriginalEntityState(localization) == null)
            {
                db.Localizations.Attach(localization, false);
                db.Refresh(RefreshMode.KeepCurrentValues, localization);
            }

            db.SubmitChanges();
        } 
        #endregion
    }
}