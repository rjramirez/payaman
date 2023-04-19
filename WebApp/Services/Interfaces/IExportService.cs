namespace WebApp.Services.Interfaces
{
    public interface IExportService
    {
        Task<byte[]> ConvertToExcelByte<TSource>(IEnumerable<TSource> reportResults);
    }
}
