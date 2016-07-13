namespace Ci.Template.Library.Enums
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public enum LanguageType
    {
        [Description("繁體中文")]
        [Display(Name = "zh-TW", ShortName = ("中"))]
        Chinese = 0,

        [Description("English")]
        [Display(Name = "en-US", ShortName = ("英"))]
        English = 1,

        [Description("日文語")]
        [Display(Name = "ja", ShortName = ("日"))]
        Japanese = 2
    }

    public enum LanguageStatus
    {
        NoData = 0,
        DataShow = 1,
        DataHide = 2
    }
}
