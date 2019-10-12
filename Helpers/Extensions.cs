using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ApiTest.Helpers
{
    public static class Extensions
    {
        public static void AddPagination(this HttpResponse response, int pageNumber, int pageSize, int totalPages, int totalCount)
        {
            var paginationHeader = new { pageNumber, pageSize, totalPages, totalCount };

            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination"); // expone la cabecera
        }
    }
}