using StyleSyndicatePrjBE.Services;
using StyleSyndicatePrjBE.Agents;
using StyleSyndicatePrjBE.Data;
using StyleSyndicatePrjBE.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure In-Memory Database
builder.Services.AddDbContext<StyleSyndicateDbContext>(options =>
    options.UseInMemoryDatabase("StyleSyndicateDb"));

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Style Syndicate API - AutoGen Multi-Agent System",
        Version = "v2.0",
        Description = "Multi-Agent Fashion Orchestrator API powered by Microsoft AutoGen - AI-driven style consulting with collaborative agent workflows",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Style Syndicate Team"
        }
    });
});

// Configure AutoGen services
var autoGenConfig = new AutoGenConfig
{
    ApiKey = builder.Configuration["AzureOpenAI:ApiKey"],
    Endpoint = builder.Configuration["AzureOpenAI:Endpoint"],
    ModelName = builder.Configuration["AzureOpenAI:ModelName"] ?? "gpt-4",
    MaxTokens = int.Parse(builder.Configuration["AzureOpenAI:MaxTokens"] ?? "2000"),
    Temperature = float.Parse(builder.Configuration["AzureOpenAI:Temperature"] ?? "0.7")
};
builder.Services.AddSingleton(autoGenConfig);
builder.Services.AddSingleton<MockLLMProvider>();

// Register Services (using EF-based implementations)
builder.Services.AddScoped<IUserDataService, EFUserDataService>();
builder.Services.AddScoped<ITrendService, MockTrendService>();
builder.Services.AddScoped<IInventoryService, EFInventoryService>();

// Register AutoGen Agents
builder.Services.AddScoped<ConciergeAgent>();
builder.Services.AddScoped<HistorianAgent>();
builder.Services.AddScoped<TrendAnalystAgent>();
builder.Services.AddScoped<InventoryScoutAgent>();
builder.Services.AddScoped<CriticAgent>();

// Register AutoGen GroupChat Manager (new implementation)
builder.Services.AddScoped<IAutoGenGroupChatManager, AutoGenGroupChatManager>();
builder.Services.AddScoped<IGroupChatManager>(sp => 
    new GroupChatManager(sp.GetRequiredService<IAutoGenGroupChatManager>()));

// Add CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "https://localhost:4200",
            "http://localhost:3000",
            "https://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Style Syndicate API v2 - AutoGen Multi-Agent");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();
