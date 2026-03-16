// Services/ApiService.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ReqForge.Models;

namespace ReqForge.Services;

public class ApiService
{
    private readonly HttpClient _httpClient = new();

    public async Task<ApiResponse> SendAsync(ApiRequest request)
    {
        var parsedHeaders = ParseHeaders(request.Headers);
        var hasContentTypeHeader = HasHeader(parsedHeaders, "Content-Type");

        var httpRequest = new HttpRequestMessage(
            new HttpMethod(request.Method.ToString()),
            request.URL
        );

        // Ha van body (POST/PUT/PATCH)
        if (!string.IsNullOrEmpty(request.Body))
        {
            httpRequest.Content = new StringContent(request.Body, Encoding.UTF8);

            // Reasonable default for API tests, but allow override via custom headers.
            if (!hasContentTypeHeader)
            {
                httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
        }

        ApplyHeaders(httpRequest, parsedHeaders);

        var stopwatch = Stopwatch.StartNew();
        var httpResponse = await _httpClient.SendAsync(httpRequest);
        stopwatch.Stop();

        var body = await httpResponse.Content.ReadAsStringAsync();
        var headers = httpResponse.Headers.ToString();

        return new ApiResponse
        {
            StatusCode = (int)httpResponse.StatusCode,
            ResponseBody = body,
            ResponseHeaders = headers,
            ElapsedTime = stopwatch.Elapsed.TotalMilliseconds
        };
    }

    private static List<KeyValuePair<string, string>> ParseHeaders(string headers)
    {
        var result = new List<KeyValuePair<string, string>>();

        if (string.IsNullOrWhiteSpace(headers))
            return result;

        var lines = headers.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var colonIndex = line.IndexOf(':');
            if (colonIndex <= 0)
            {
                throw new FormatException($"Invalid header format on line {i + 1}. Use 'Header-Name: value'.");
            }

            var name = line.Substring(0, colonIndex).Trim();
            var value = line.Substring(colonIndex + 1).Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new FormatException($"Invalid header name on line {i + 1}.");
            }

            result.Add(new KeyValuePair<string, string>(name, value));
        }

        return result;
    }

    private static bool HasHeader(List<KeyValuePair<string, string>> headers, string name)
    {
        foreach (var header in headers)
        {
            if (string.Equals(header.Key, name, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private static void ApplyHeaders(HttpRequestMessage request, List<KeyValuePair<string, string>> headers)
    {
        foreach (var header in headers)
        {
            if (request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                continue;

            if (request.Content is null)
            {
                throw new InvalidOperationException(
                    $"Header '{header.Key}' cannot be applied without request content.");
            }

            if (!request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value))
            {
                throw new InvalidOperationException($"Failed to apply header '{header.Key}'.");
            }
        }
    }
}
