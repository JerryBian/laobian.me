using Laobian.Lib;
using Laobian.Lib.Cache;
using Laobian.Lib.Command;
using Laobian.Lib.Converter;
using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Option;
using Laobian.Lib.Repository;
using Laobian.Lib.Service;
using Laobian.Lib.Store;
using Laobian.Lib.Worker;
using Laobian.Web.HostedServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

// testing
//var f1 = "C:\\temp2\\asset\\blog\\post";
//var f2 = "C:\\temp2\\asset\\blog\\post1";
//Directory.CreateDirectory(f2);
//foreach(var f in Directory.EnumerateFiles(f1, "*", SearchOption.AllDirectories))
//{
//    var c = File.ReadAllText(f);
//    var p = JsonHelper.Deserialize<BlogPost>(c);
//    p.Id = StringHelper.Random();
//    var path = Path.Combine(f2, $"{p.Id}.json");
//    File.WriteAllText(path, JsonHelper.Serialize(p, true));
//}

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureAppConfiguration((hostContext, config) => { _ = config.AddEnvironmentVariables("ENV_"); });

// Add services to the container.
builder.Services.Configure<LaobianOption>(o => { o.FetchFromEnv(builder.Configuration); });

builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));
builder.Services.AddSingleton<IReadRepository, ReadRepository>();
builder.Services.AddSingleton<IReadService, ReadService>();
builder.Services.AddSingleton<IBlogRepository, BlogRepository>();
builder.Services.AddSingleton<IBlogService, BlogService>();
builder.Services.AddSingleton<ICacheManager, MemoryCacheManager>();
builder.Services.AddSingleton<ICommandClient, CommandClient>();
builder.Services.AddSingleton<IBlogPostAccessWorker, BlogPostAccessWorker>();

builder.Services.AddMemoryCache();
builder.Services.AddHostedService<GitFileHostedService>();
builder.Services.AddHostedService<BlogPostHostedService>();
builder.Services.AddControllersWithViews().AddJsonOptions(config =>
{
    config.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    var converter = new IsoDateTimeConverter();
    config.JsonSerializerOptions.Converters.Add(converter);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = $".LAOBIAN.AUTH.{builder.Environment.EnvironmentName}";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.ReturnUrlParameter = "returnUrl";
                options.LoginPath = new PathString("/login");
                options.LogoutPath = new PathString("/logout");
                options.Cookie.Domain = builder.Environment.IsDevelopment() ? "localhost" : ".laobian.me";
            });

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapAreaControllerRoute(Constants.AreaAdmin, Constants.AreaAdmin, "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapAreaControllerRoute(Constants.AreaRead, Constants.AreaRead, "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapAreaControllerRoute(Constants.AreaBlog, Constants.AreaBlog, "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();