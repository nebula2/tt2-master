using SkiaSharp;
using System.Security;

namespace TT2Master
{
    /// <summary>
    /// Resource getter
    /// </summary>
    [SecuritySafeCritical]
    public interface IGetBitmapResources
    {
        SKBitmap GetDecodedResource(string resourceId);
    }
}