using System.Collections.Generic;

namespace TT2Master
{
    /// <summary>
    /// Supported Languages
    /// </summary>
    public static class SupportedLanguages
    {
        /// <summary>
        /// List of supported languages
        /// </summary>
        public static List<Language> Languages => new List<Language>
        {
            new Language {DisplayName = "German - Deutsch (de-DE)"      , ShortName = "de-DE"},
            new Language {DisplayName = "English"                       , ShortName = "en"},
            //new Language {DisplayName = "Portuguese - Português (pt-PT)", ShortName = "pt-PT"},
            new Language {DisplayName = "Portuguese - Português (pt-BR)", ShortName = "pt-BR"},
            new Language {DisplayName = "French - Français (fr-FR)"     , ShortName = "fr-FR"},
            new Language {DisplayName = "Korean - 한국의 (ko-KR)"        , ShortName = "ko-KR"},
            new Language {DisplayName = "Spanish - Español (es-ES)"     , ShortName = "es-ES"},
            new Language {DisplayName = "Dutch - Nederlands (nl-NL)"    , ShortName = "nl-NL"},
            new Language {DisplayName = "Russian - Pусский (ru-RU)"     , ShortName = "ru-RU"},
            new Language {DisplayName = "Turkish - Türk (tr-TR)"        , ShortName = "tr-TR"},
            new Language {DisplayName = "Chinese - 中国 (zh-Hans)"      , ShortName = "zh-Hans"},
            new Language {DisplayName = "Czech - český (cs-CZ)"         , ShortName = "cs-CZ"},
            new Language {DisplayName = "Polish - polski (pl-PL)"       , ShortName = "pl-PL"},
        };
    }
}