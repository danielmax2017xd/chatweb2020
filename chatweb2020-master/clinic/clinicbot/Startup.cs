// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.9.2

using AdaptiveExpressions;
using clinicbot.Data;
using clinicbot.Dialogs;
using clinicbot.infrastructura.Luis;
using clinicbot.infrastructura.QnAMakerAI;
using clinicbot.infrastructura.SendGridEmail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace clinicbot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)


        {
            var storage = new AzureBlobStorage(
                Configuration.GetSection("StorageConnectionString").Value,
                 Configuration.GetSection("StorageContainer").Value
                );

            var userState = new UserState(storage);
            services.AddSingleton(userState);

            var conversationSate = new ConversationState(storage);
            services.AddSingleton(conversationSate);


            services.AddControllers().AddNewtonsoftJson();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddDbContext<DataBaseService>(options => {
                options.UseCosmos(

                    Configuration["CosmosEndPoint"],
                        Configuration["CosmosKey"],
                            Configuration["CosmosDatabase"]
                    );
            });
            services.AddScoped<IDataBaseService, DataBaseService>();


            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
           
      

            services.AddSingleton<ISendGridEmailService, SendGridEmailService>();
            services.AddSingleton<ILuisService, LuisService>();

            services.AddSingleton< IQnAMakerAIService , QnAMakerAIService>();
            services.AddSingleton<IStorage, MemoryStorage>();
            services.AddSingleton<ConversationState>();
            services.AddTransient<RootDialog>();
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, ClinicBot<RootDialog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
