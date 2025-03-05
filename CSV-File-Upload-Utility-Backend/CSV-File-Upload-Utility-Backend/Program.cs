using CSV_File_Upload_Utility_Backend.Infrastructure;
using CSV_File_Upload_Utility_Backend.Interfaces;
using CSV_File_Upload_Utility_Backend.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    builder.Services.AddSingleton(sp => {
        var cosmosEndpoint = builder.Configuration["CosmosDb:Endpoint"];
        var cosmosKey = builder.Configuration["CosmosDb:Key"];
        return new CosmosClient(cosmosEndpoint, cosmosKey);
    });
    

    builder.Services.AddScoped<IFileParseService, FileParserService>();
    builder.Services.AddScoped<ISalesOrderService>(sp => {
        var cosmosClient = sp.GetRequiredService<CosmosClient>();
        var databaseName = builder.Configuration["CosmosDb:DatabaseName"];
        var containerName = builder.Configuration["CosmosDb:ContainerName"];
        return new CosmosDbService(cosmosClient, databaseName, containerName);
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            policy =>
            {
                policy.WithOrigins("http://localhost:8081", "https://localhost:8080")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseHttpsRedirection();
    app.UseCors("CorsPolicy");
    app.UseAuthorization();
    app.MapControllers();


    app.Run();