using System.Text.Json.Serialization;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// RabbitMQ Config
ConnectionFactory factory = new()
{
    UserName = builder.Configuration["Rabbit:UserName"]!,
    Password = builder.Configuration["Rabbit:Password"]!,
    VirtualHost = builder.Configuration["Rabbit:VirtualHost"]!,
    HostName = builder.Configuration["Rabbit:HostName"]!,
    Port = int.Parse(builder.Configuration["Rabbit:Port"]!)
};

IConnection conn = await factory.CreateConnectionAsync();
IChannel channel = await conn.CreateChannelAsync();

builder.Services.AddSingleton(channel);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePathBase("/weather");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();