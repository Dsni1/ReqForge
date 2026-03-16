namespace ReqForge.Models;

public class ApiRequest
{
    public string URL  { get; set; } = "";
    public Method Method { get; set; } = Method.GET;
    public string Headers { get; set; } = "";
    public string Body { get; set; } = "";
}