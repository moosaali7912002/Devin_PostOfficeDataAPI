using Microsoft.EntityFrameworkCore;
using PostOfficeApi.Data;
using PostOfficeApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostOfficeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PostOfficeDb")));

builder.Services.AddScoped<IPostOfficeDataService, PostOfficeDataService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseSwaggerUI(options =>
    //{
    //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PostOfficeApi v1");
    //});
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
