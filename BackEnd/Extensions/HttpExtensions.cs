using BackEnd.Helpers;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BackEnd.Extensions
{
    public static class HttpExtensions
    {
         public static void AddPaginationHeader(this HttpResponse response, int currentPage, int ItemsPerPage,
         int TotalItems,int TotalPages)

         {
            var paginationHeader = new PaginationHeader(currentPage,ItemsPerPage,TotalItems,TotalPages);

            var option = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            response.Headers.Add("pagination", JsonSerializer.Serialize(paginationHeader, option));
            response.Headers.Add("Access-Control-Expose-Headers", "pagination");
         }
    }
}
