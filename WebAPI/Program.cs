using ApiConfiguration;
using Common.DataTransferObjects.AppSettings;

using Common.Constants;
using Common.DataTransferObjects.ErrorLog;
using Common.DataTransferObjects.Version;
using DataAccess.DBContexts.PayamanDB;
using DataAccess.Services;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebAPI.Services;
using WebAPI.Services.Interfaces;
using WebApi.Helpers;
using DataAccess.UnitOfWorks.PayamanDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Entities;


/*SERVICES CONTAINER*/
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


/* Identity Server */
IdentityServerApiDefinition identityServerApiDefinition = new();
builder.Configuration.Bind("IdentityServerApiDefinition", identityServerApiDefinition);
builder.Services.AddSingleton(identityServerApiDefinition);

ApiServices.ConfigureServices(builder.Services, identityServerApiDefinition);

//Api Policy Authorization
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("SystemLog", builder =>
//    {
//        builder.RequireScope("PayamanApi.SystemLog");
//    });
//});



//DBContext Registration
builder.Services.AddDbContextPool<PayamanDBContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PayamanDB")));

//UoW Registration
builder.Services.AddScoped<IPayamanDBUnitOfWork, PayamanDBUnitOfWork>();

//Internal Service Registration
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
builder.Services.AddScoped<IDbContextChangeTrackingService, DbContextChangeTrackingService>();


// configure automapper with all automapper profiles from this assembly
builder.Services.AddAutoMapper(typeof(Program));



// 2. Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<PayamanDBContext>()
    .AddDefaultTokenProviders();

// 3. Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// 4. Adding Jwt Bearer
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
        };
    });

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// 5. Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowPayamanClient",
        b =>
        {
            b
                .WithOrigins("http://localhost:7020")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});




/*HTTP REQUEST PIPELINE*/
var app = builder.Build();


app.UseExceptionHandler(errorLogger =>
{
    errorLogger.Run(async context =>
    {
        var scoped = app.Services.CreateScope();
        IErrorLogService errorLogService = scoped.ServiceProvider.GetRequiredService<IErrorLogService>();
        context.Response.StatusCode = 500;
        ErrorMessage unhandledErrorDetail = await errorLogService.LogApiError(context);
        await context.Response.WriteAsync(JsonConvert.SerializeObject(unhandledErrorDetail));
    });
});

app.UseSwagger();
IApiVersionDescriptionProvider apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwaggerUI(options =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
        options.RoutePrefix = "docs";
    }
});

//7. Use CORS
app.UseCors("AllowPayamanClient");

app.UseHttpsRedirection();
app.UseRouting();

// 8. Authentication
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGet("/", async context =>
    {
        context.Response.ContentType = ApiHomePageConstant.ContentType;
        await context.Response.WriteAsync(
            string.Format(ApiHomePageConstant.ContentFormat,
            identityServerApiDefinition.ApiName,
            app.Environment.EnvironmentName,
            context.Request.Scheme,
            context.Request.Host.Value,
            VersionDetail.DisplayVersion()));
    });
});


app.Run();

