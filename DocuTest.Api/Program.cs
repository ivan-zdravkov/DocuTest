using DocuTest.Application.Contexts;
using DocuTest.Application.Interfaces;
using DocuTest.Application.Services;
using DocuTest.Application.Strategies;
using DocuTest.Data.Main.DAL.Factories;
using DocuTest.Data.Main.DAL.Generators;
using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Data.Main.DAL.Repositories;
using DocuTest.Shared.Enums;
using DocuTest.Shared.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Boolean.TryParse(builder.Configuration["GenerateData"], out bool generateData);

if (generateData)
{
    DataGenerator.Generate(connectionString).Wait();

    Console.WriteLine("Data generation completed.");
}
else
{
    Console.WriteLine("Data generation skipped.");
}

builder.Services.AddSingleton<IDbConnectionFactory>(new SqlConnectionFactory(connectionString));

builder.Services.AddScoped<DocuTest.Shared.Interfaces.IUserContext>(services => new UserContext(Guid.NewGuid(), "Test User", "me@example.con", Role.Admin));

builder.Services.AddScoped<IDocumentReadStrategy>(services => new DocumentRoleReadStrategy(services.GetService<IUserContext>()!));
builder.Services.AddScoped<IDocumentWriteStrategy>(services => new DocumentRoleWriteStrategy(services.GetService<IUserContext>()!));

builder.Services.AddScoped<IMetadataRepository, MetadataRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

builder.Services.AddScoped<IMetadataService, MetadataService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();