using ApiConfiguration;
using Common.DataTransferObjects.AppSettings;

using Common.Constants;
using Common.DataTransferObjects.ErrorLog;
using Common.DataTransferObjects.Version;
using DataAccess.DBContexts.RITSDB;
using DataAccess.Services;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebAPI.Services;
using WebAPI.Services.Interfaces;
using WebApi.Helpers;
using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Authorization;
using DataAccess.DbContexts.RITSDB.Models;
using Microsoft.AspNetCore.Identity;



/*SERVICES CONTAINER*/
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;



/* Identity Server */
IdentityServerApiDefinition identityServerApiDefinition = new();
builder.Configuration.Bind("IdentityServerApiDefinition", identityServerApiDefinition);
builder.Services.AddSingleton(identityServerApiDefinition);



//Api Policy Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SystemLog", builder =>
    {
        builder.RequireScope("RITSApi.SystemLog");
    });
    options.AddPolicy("SystemData", builder =>
    {
        builder.RequireScope("RITSApi.SystemData");
    });
});

//DBContext Registration
builder.Services.AddDbContextPool<RITSDBContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("RITSDB")));
builder.Services.AddDbContext<AppDBContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("RITSDB")));

//UoW Registration
builder.Services.AddScoped<IRITSDBUnitOfWork, RITSDBUnitOfWork>();

//Internal Service Registration
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
builder.Services.AddScoped<IDbContextChangeTrackingService, DbContextChangeTrackingService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();


// configure automapper with all automapper profiles from this assembly
builder.Services.AddAutoMapper(typeof(Program));


// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<RITSDBContext>()
    .AddDefaultTokenProviders();


// Configure IdentityServer
builder.Services.AddIdentityServer()
    .AddAspNetIdentity<ApplicationUser>()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(builder.Configuration.GetConnectionString("RITSDB"));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(builder.Configuration.GetConnectionString("RITSDB"));
    }).AddDeveloperSigningCredential();

ApiServices.ConfigureServices(builder.Services, identityServerApiDefinition);



// configure automapper with all automapper profiles from this assembly
builder.Services.AddAutoMapper(typeof(Program));


/*HTTP REQUEST PIPELINE*/
var app = builder.Build();

// Seed data
//SeedData.Initialize(app.Services);

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


app.UseHttpsRedirection();
app.UseRouting();

// Enable IdentityServer4
app.UseIdentityServer();

// Enable authentication
app.UseAuthentication();

// Enable authorization
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

