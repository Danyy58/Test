using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Task.Run(() =>
{
    Thread.Sleep(1000);
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "http://localhost:5000/swagger",
            UseShellExecute = true
        });
    }
    catch { }
});

app.Run("http://localhost:5000");
