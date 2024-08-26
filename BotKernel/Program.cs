using BotKernel.Data;
using BotKernel.Middlewares;
using BotKernel.Repositories;
using BotKernel.SwaggerConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<HeaderFilter>();
});

builder.Services.AddSingleton<OracleDbContext>();
builder.Services.AddScoped<EmployeeContactsRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseApiAuth();

app.UseAuthorization();

app.MapControllers();

app.Run();
