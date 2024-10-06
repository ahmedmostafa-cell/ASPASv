using API.Data;
using API.Entities;
using System;
using Newtonsoft.Json.Linq;
using EmailService;
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

        public async Task<List<EmailService.StockResult>> SaveStockResultsAsync(JToken jsonResponse)
        {
            var resultsToSendEmail = new List<EmailService.StockResult>();

           

            // Loop through each result in the JSON array
            foreach (var result in jsonResponse)
            {
                // Create a new StockResult object
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

                // Add the stock result to the list
                resultsToSendEmail.Add(stockResult);
            }

            // Return the list of stock results
            return resultsToSendEmail;
        }

        private async Task FetchApiDataAndNotifyClients()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                var httpClient = _httpClientFactory.CreateClient();

                var apiUrl = _configuration["AppSettings:ApiUrl"];
                var apiKey = _configuration["AppSettings:ApiKey"];

                // Fetch data from the API
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("Authorization", $"Bearer {apiKey}");
                var response = await httpClient.SendAsync(request);
                var apiData = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(apiData);
                var resultsToSaveDatabase = jsonResponse["results"];
                var resultsToSendEmail = await SaveStockResultsAsync(resultsToSaveDatabase);
                var clients = dbContext.Clients.ToList();

                // Save the data to the database
                foreach (var result in resultsToSaveDatabase)
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

                // Get clients and send them emails
               
                foreach (var client in clients)
                {
                    // Logic to send email
                    await SendEmailToClient(client, resultsToSendEmail);
                   
                }
            }
        }

        private Task SendEmailToClient(Client client, List<EmailService.StockResult> results)
        {
            
            _logger.LogInformation($"Sending email to {client.EmailAddress}");
            IFormFileCollection files = null;
            MySearch mysearch = new MySearch();
            var messages = new Message(new string[] { client.EmailAddress }, "Email From ASAP Systems " , "This is the content from our async email. i am happy", files);
             _emailSender.SendEmailAsync(messages, results , client.FirstName);
            // Add your email sending logic here
            return Task.CompletedTask;
        }
    }

}
