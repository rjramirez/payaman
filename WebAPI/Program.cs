using ApiConfiguration;
using WebAPI.Authorization;
using WebAPI.Helpers;
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


/*SERVICES CONTAINER*/
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

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
//        builder.RequireScope("RITSApi.SystemLog");
//    });
//});

//DBContext Registration
builder.Services.AddDbContextPool<RITSDBContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("RITSDB")));

//UoW Registration
builder.Services.AddScoped<IRITSDBUnitOfWork, RITSDBUnitOfWork>();

//Internal Service Registration
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
builder.Services.AddScoped<IDbContextChangeTrackingService, DbContextChangeTrackingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddHttpContextAccessor();

// configure automapper with all automapper profiles from this assembly
builder.Services.AddAutoMapper(typeof(Program));


// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


/*HTTP REQUEST PIPELINE*/
var app = builder.Build();


// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    //app.MapControllers();
}

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

