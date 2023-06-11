using DocuTest.Data.Main.DAL.Generators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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