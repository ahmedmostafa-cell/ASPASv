using API.Data;
using API.Entities;
using System;
using Newtonsoft.Json.Linq;
using EmailService;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;
namespace API.Services
{
    public class ApiDataService : BackgroundService
    {
        private readonly ILogger<ApiDataService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public ApiDataService(
            ILogger<ApiDataService> logger,
            IHttpClientFactory httpClientFactory,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await FetchApiDataAndNotifyClients();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        public async Task<List<EmailService.StockResult>> StockResultsToSendEmailAsync(JToken jsonResponse)
        {
            var resultsToSendEmail = new List<EmailService.StockResult>();
            foreach (var result in jsonResponse)
            {
                var stockResult = new EmailService.StockResult
                {
                    T = result["T"].ToString(),
                    V = Convert.ToInt32(result["v"]),
                    Vw = Convert.ToDecimal(result["vw"]),
                    O = Convert.ToDecimal(result["o"]),
                    C = Convert.ToDecimal(result["c"]),
                    H = Convert.ToDecimal(result["h"]),
                    L = Convert.ToDecimal(result["l"]),
                    N = Convert.ToInt32(result["n"]),
                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(result["t"])).UtcDateTime // Convert Unix timestamp to DateTime
                };
                resultsToSendEmail.Add(stockResult);
            }

            return resultsToSendEmail;
        }

        public async Task StockResultsToSaveDatabaseAsync(JToken jsonResponse)
        {
            using (var scope = _scopeFactory.CreateScope()) 
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                foreach (var result in jsonResponse)
                {
                    API.Entities.StockResult stockResult = new API.Entities.StockResult
                    {
                        T = result["T"].ToString(),
                        V = Convert.ToInt32(result["v"]),
                        Vw = Convert.ToDecimal(result["vw"]),
                        O = Convert.ToDecimal(result["o"]),
                        C = Convert.ToDecimal(result["c"]),
                        H = Convert.ToDecimal(result["h"]),
                        L = Convert.ToDecimal(result["l"]),
                        N = Convert.ToInt32(result["n"]),
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(result["t"])).UtcDateTime // Convert Unix timestamp to DateTime
                    };

                    dbContext.StockResults.Add(stockResult);
                }
                await dbContext.SaveChangesAsync();
            }
        }

        private async Task FetchApiDataAndNotifyClients()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = _configuration["AppSettings:ApiUrl"];
                var apiKey = _configuration["AppSettings:ApiKey"];
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("Authorization", $"Bearer {apiKey}");
                var response = await httpClient.SendAsync(request);
                var apiData = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(apiData);
                var results = jsonResponse["results"];
                var resultsToSendEmail = await StockResultsToSendEmailAsync(results);
                await StockResultsToSaveDatabaseAsync(results);
                var clients = dbContext.Clients.ToList();
                foreach (var client in clients)
                {
                    await SendEmailToClient(client, resultsToSendEmail);
                }
            }
        }

        private Task SendEmailToClient(Client client, List<EmailService.StockResult> results)
        {
            _logger.LogInformation($"Sending email to {client.EmailAddress}");
            IFormFileCollection files = null;
            var messages = new Message(new string[] { client.EmailAddress }, "Email From ASAP Systems " , "This is the content from our async email. i am happy", files);
             _emailSender.SendEmailAsync(messages, results , client.FirstName);
      
            return Task.CompletedTask;
        }
    }

}
