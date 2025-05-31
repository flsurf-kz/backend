using Hangfire;
using Flsurf.Infrastructure;
using Flsurf.Presentation.Web;
using Flsurf.Domain;
using Flsurf.Application;
using Flsurf.Infrastructure.Data;
using Flsurf.Infrastructure.EventStore;
using Flsurf.Infrastructure.BackgroundJobs;
using Flsurf.Presentation.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
builder.Services.AddDomainServices();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration, builder.Environment, builder.Logging);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "api/swagger";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Flsurf API");
        options.DefaultModelsExpandDepth(-1);
    });
    await app.InitialiseDatabaseAsync();
    await app.InitialiseEventStoreDatabaseAsync(); 
}
app.UseEventDispatcher(); 
app.UseStaticFiles();

if (app.Environment.IsProduction())
    app.UseResponseCompression();

app.UseRouting();

app.UseCors("FLsurf");

app.UseAntiforgery();

// Õ≈ Ã≈Õﬂ“‹, ÀŒÀ
app.UseAuthentication();
app.UseAuthorization();

app.UseHealthChecks("/api/health");

if (app.Environment.IsProduction())
{
    app.UseHangfireDashboard("/api/hangfire", new DashboardOptions
    {
        Authorization = new[] { new CookieDashboardAuthorization() }
    });
} else
{
    app.UseHangfireDashboard("/api/hangfire"); 
}
BackgroundJobsRegister.RegisterInfrastructureBGJobs();

app.UseWebSockets(new WebSocketOptions() { 
    KeepAliveInterval = TimeSpan.FromMinutes(2), 
});

app.MapHub<GeneralHub>("/api/ws/general").RequireAuthorization();

app.MapHealthChecks("/api/health");
app.MapControllers();

app.Run();

// for testing! 
public partial class Program { }