using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.ActionsResult;

namespace Data.Transactions.API.Helpers
{
    public static class HttpHelper
    {
        /// <summary>
        /// Baixa uma string assincronamente.
        /// </summary>
        /// <param name="url">A url de onde a string será baixada.</param>
        /// <returns>Um objeto do tipo <see cref="DownloadStringResponse"/> com as informações da Request.</returns>
        public static async Task<Result<string>> DownloadStringAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        return Result<string>.Ok(await response.Content.ReadAsStringAsync());
                    }

                    return Result<string>.Fail(null,
                        $"Could not download string at url: {url}. Status code: {response.StatusCode}");
                }
                catch (HttpRequestException ex)
                {
                    return Result<string>.Fail(null, $"Exception on download string at url: {url}. Message: {ex.Message}");
                }
            }
        }
    }
}