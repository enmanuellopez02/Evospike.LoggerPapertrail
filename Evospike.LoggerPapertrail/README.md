# LoggerPapertrail
Simple .NET registry with fully structured events using Papertrail

### `appsettings.json` configuration

The file path and other settings can be read from JSON configuration if desired.

In `appsettings.json` add a `"PapertrailSetting"` properties:

```json
{
   "PapertrailSetting": {
    "Host": "logs.papertrailapp.com",
    "Port": "23526"
  }
}
```

And then pass the configuration section to the next methods:

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    loggerFactory.AddLoggerPapertrail(Configuration.GetSection(nameof(PapertrailSetting)).Get<PapertrailSetting>());
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        endpoints.MapHealthChecksUI();
        endpoints.MapControllers();
    });
}
```

Example of a controller using dependency injection services

```csharp
[Route("[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private readonly ILogger<ItemsController> _logger;
    private readonly IRepository<Item> _itemsRepository;

    public ItemsController(ILogger<ItemsController> logger, IRepository<Item> itemsRepository)
    {
        _logger = logger;
        _itemsRepository = itemsRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetAsync()
    {
        try
        {
            var items = (await _itemsRepository.GetAllAsync()).Select(item => item.AsDto());
        }
        catch(Exception e)
        {
            _logger.LogError("Error in method {0}", GetAsync);
        }
        return items;
    }
}
```