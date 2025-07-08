using System.Text.Json;

namespace Servicio_DW_Pagos.Servicios
{

    public class ServicioTipoCambio
    {
        private readonly HttpClient _httpClient;
        public ServicioTipoCambio(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("TipoCambioService");
        }
        public async Task<decimal?> ObtenerTipoCambioPorCodigo(string codigoMoneda)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>("tiposCambioActual");

                if (response.TryGetProperty("tiposCambio", out var tiposCambio))
                {
                    foreach (var moneda in tiposCambio.EnumerateArray())
                    {
                        if (moneda.GetProperty("codigo").GetString().Equals(codigoMoneda, StringComparison.OrdinalIgnoreCase))
                        {
                            return moneda.GetProperty("tipo_Cambio").GetDecimal();
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
