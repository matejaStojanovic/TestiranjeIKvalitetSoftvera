using TransactionNow.Application.Interfaces;
using TransactionNow.Application.Services;
using TransactionNow.Infrastructure;
using TransactionNow.Infrastructure.Repositories;
using TransactionNow.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<MongoContext>();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IInvoiceRepository, MongoInvoiceRepository>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<InvoiceService>();




builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReact");

app.UseSession();


app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();




if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var mongoContext = scope.ServiceProvider.GetRequiredService<MongoContext>();
    DatabaseSeeder.Seed(mongoContext);
}

app.Run();
