// ViewModels/MainViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReqForge.Models;
using ReqForge.Services;

namespace ReqForge.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ApiService _apiService = new();

    // History
    public ObservableCollection<RequestHistoryItem> History { get; } = new();
    [ObservableProperty] private RequestHistoryItem? _selectedHistoryItem;

    partial void OnSelectedHistoryItemChanged(RequestHistoryItem? value)
    {
        LoadFromHistory(value);
    }

    // Request mezők
    [ObservableProperty] private string _url = "https://";
    [ObservableProperty] private Method _selectedMethod = Method.GET;
    [ObservableProperty] private string _requestHeader = "";
    [ObservableProperty] private string _requestBody = "";

    // Response mezők
    [ObservableProperty] private string _responseBody = "";
    [ObservableProperty] private string _responseHeaders = "";
    [ObservableProperty] private int _statusCode;
    [ObservableProperty] private double _elapsedTime;

    // UI állapot
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = "";

    // A ComboBoxhoz — minden enum értéket felsorol
    public IEnumerable<Method> Methods => Enum.GetValues<Method>();

    [RelayCommand]
    private async Task SendRequestAsync()
    {
        ErrorMessage = "";
        IsLoading = true;
        try
        {
            var request = new ApiRequest
            {
                URL = Url,
                Method = SelectedMethod,
                Headers = RequestHeader,
                Body = RequestBody
            };

            var response = await _apiService.SendAsync(request);

            StatusCode = response.StatusCode;
            ResponseBody = TryFormatJson(response.ResponseBody);
            ResponseHeaders = response.ResponseHeaders;
            ElapsedTime = response.ElapsedTime;
            
            History.Insert(0, new RequestHistoryItem
            {
                Timestamp = DateTime.Now,
                Method = request.Method,
                Url = request.URL,
                StatusCode = response.StatusCode,
                ElapsedTime = response.ElapsedTime,
                RequestBody = request.Body,
                RequestHeader = request.Headers,
                ResponseBody = response.ResponseBody
            });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void LoadFromHistory(RequestHistoryItem? item)
    {
        if (item is null) return;

        Url = item.Url;
        SelectedMethod = item.Method;
        RequestBody = item.RequestBody;
        RequestHeader = item.RequestHeader;
    }
    
    private static string TryFormatJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        try
        {
            using var doc = JsonDocument.Parse(input);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (JsonException)
        {
            return input;
        }
    }
}