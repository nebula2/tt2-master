using System.Security;
using System.Threading.Tasks;

namespace TT2Master.Interfaces
{
    [SecuritySafeCritical]
    public interface IPhotoLibrary
    {
        //Task<Stream> PickPhotoAsync();

        Task<(bool, string)> SavePhotoAsync(byte[] data, string folder, string filename);
    }
}