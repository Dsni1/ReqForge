using System;

namespace ReqForge.Models;

public class RequestHistoryItem
{
    public DateTime Timestamp { get; set; }
    public Method Method { get; set; }
    public string Url { get; set; } = "";
    public int StatusCode { get; set; }
    public double ElapsedTime { get; set; }
    public string RequestBody { get; set; } = "";
    public string RequestHeader { get; set; } = "";
    public string ResponseBody { get; set; } = "";

    public string Title => $"{Method} {Url}";
}