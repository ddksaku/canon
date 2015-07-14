using System;
using System.Collections.Generic;
using System.Linq;
using CanonWebApp.Code;
using CanonWebApp.Enums;

namespace CanonWebApp.Business
{
    /// <summary>
    /// Language business object class.
    /// </summary>
    public class Language
    {
        #region GetLanguagesList
        /// <summary>
        /// Get languages list.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetLanguagesList()
        {
            Dictionary<int, string> languages = new Dictionary<int, string>();

            foreach (Languages language in System.Enum.GetValues(typeof(Languages)).Cast<Languages>())
            {
                languages.Add((int)language, Utilities.GetResourceString("Common", String.Concat("Language", language.ToString())));
            }

            return languages;
        }
        #endregion

        #region SupportedLanguages
        /// <summary>
        /// Supported languages.
        /// </summary>
        public static Dictionary<Languages, string> SupportedLanguages
        {
            get
            {
                Dictionary<Languages, string> supportedLanguages = new Dictionary<Languages, string>();

                supportedLanguages.Add(Languages.English, "en-GB");
                supportedLanguages.Add(Languages.Czech, "cs-CZ");

                return supportedLanguages;
            }
        }
        #endregion
    }
}