using GigaChat.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GigaChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();
            builder.Services.AddSession();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(x=>
            {
                x.AccessDeniedPath = "/Register/Home";
                x.LoginPath = "/Register/Home";
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.MapHub<ChatHub>("/chat");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name:"room",
                pattern: "room/{room}",
                defaults: new
                {
                    Action = "Index",
                    Controller = "Home",
                    room = ""
                }
            );
            app.MapControllerRoute(
                name: "default",
                pattern: "{action=Register}/{controller=Home}/{room?}");

            app.Run();
        }
    }
}
