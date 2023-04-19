using Common.DataTransferObjects.Image;
using Common.DataTransferObjects.MsGraph;

namespace WebApp.Services.Interfaces
{
    public interface IMicrosoftGraphService
    {
        Task<IEnumerable<MsGraphUserDetail>> FilterUser(string searchString);
        Task<MsGraphUserDetail> GetUserById(string msGraphId);
        Task<ImageDetail> GetUserPhotoById(string msGraphId, string size);
    }
}
