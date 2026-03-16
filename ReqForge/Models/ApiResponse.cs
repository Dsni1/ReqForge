namespace ReqForge.Models;

public class ApiResponse
{
    public int StatusCode { get; set; } = 200;
    public string ResponseBody  { get; set; } = "";
    public string ResponseHeaders { get; set; } = "";
    public double ElapsedTime { get; set; } = 0;
}