using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RedisExampleApi;
using RedisExampleApi.Repository;
using RedisExampleApi.Services;
using RedisExampleCache;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository>(sp =>
{
    var appdbcontext=sp.GetRequiredService<AppDbContext>();
    var productrepository=new ProductRepository(appdbcontext);
    var redisservice=sp.GetRequiredService<RedisService>();
    return new ProductRepositoryWithCache(productrepository, redisservice);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseInMemoryDatabase("myDatabase");
});

builder.Services.AddSingleton<RedisService>(sp =>
{
    return new RedisService(builder.Configuration["CacheOptions:Url"]);
});
builder.Services.AddSingleton<IDatabase>(sp =>
{
    var redisservice = sp.GetRequiredService<RedisService>();
    return redisservice.GetDb(0);
});

var app = builder.Build();
using (var scope=app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

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
