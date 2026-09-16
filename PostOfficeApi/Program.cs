using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using PostOfficeApi.Authentication;
using PostOfficeApi.Data;
using PostOfficeApi.Models.Dtos;
using PostOfficeApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Where(x => x.Value?.Errors.Count > 0).SelectMany(x => x.Value!.Errors.Select(e => new
                {
                    Field = x.Key,
                    Error = e.ErrorMessage
                }))
                .ToList();

            var firstError = errors.FirstOrDefault();

            var message = firstError != null? $"Invalid value for field '{firstError.Field}'." : "The submitted payload is invalid.";

            var response = new PostOfficeDataCreateResponse
            {
                Success = false,
                Message = message,
                TrackingNo = string.Empty,
                RecordId = 0,
                RecordedAt = DateTime.UtcNow.AddHours(5)
            };

            return new BadRequestObjectResult(response);
        };
    })
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    foreach (var header in new[]
             {
                 SignatureAuthenticationDefaults.AppNumberHeader,
                 SignatureAuthenticationDefaults.UserTokenHeader,
                 SignatureAuthenticationDefaults.CallDateTimeHeader,
                 SignatureAuthenticationDefaults.SignatureHeader
             })
    {
        options.AddSecurityDefinition(header, new OpenApiSecurityScheme
        {
            Name = header,
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Description = $"{header} request header."
        });

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            { new OpenApiSecuritySchemeReference(header, document), new List<string>() }
        });
    }
});

var connectionString = builder.Configuration.GetConnectionString("PostOfficeDb");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'PostOfficeDb' is not configured. Set it with user secrets " +
        "(dotnet user-secrets set \"ConnectionStrings:PostOfficeDb\" \"...\") or the " +
        "ConnectionStrings__PostOfficeDb environment variable.");
}

builder.Services.AddDbContext<PostOfficeDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IPostOfficeDataService, PostOfficeDataService>();

builder.Services.AddMemoryCache();
builder.Services.Configure<ApiClientOptions>(builder.Configuration.GetSection(ApiClientOptions.SectionName));

builder.Services
    .AddAuthentication(SignatureAuthenticationDefaults.AuthenticationScheme)
    .AddScheme<SignatureAuthenticationOptions, SignatureAuthenticationHandler>(
        SignatureAuthenticationDefaults.AuthenticationScheme, _ => { });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(SignatureAuthenticationDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
