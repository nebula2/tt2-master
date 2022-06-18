using System;
using System.Collections.Generic;
using System.Text;
using TT2Master.Shared.Models;

namespace TT2Master.ViewModels.Assets
{
    public class AssetTypeViewModel : AssetType
    {
        public string AssetStateTranslated { get; set; }

        public void TranslateAssetState() => AssetStateTranslated = Enum.GetName(typeof(AssetDownloadResult), AssetState).TranslatedString();
    }
}