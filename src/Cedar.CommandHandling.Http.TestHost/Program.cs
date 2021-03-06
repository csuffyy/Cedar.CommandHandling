﻿namespace Cedar.HttpCommandHandling.TestHost
{
    using System;
    using System.Diagnostics;
    using Cedar.CommandHandling;
    using Cedar.CommandHandling.Http;
    using Microsoft.Owin;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.Hosting;
    using Microsoft.Owin.StaticFiles;
    using Owin;

    internal class Program
    {
        private static void Main()
        {
            var resolver = new CommandHandlerResolver(new CommandModule());
            var settings = new CommandHandlingSettings(resolver);
            var commandHandlingMiddleware = CommandHandlingMiddleware.HandleCommands(settings);

            using(WebApp.Start("http://localhost:8080",
                app =>
                {
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        RequestPath = new PathString(""),
                        FileSystem = new PhysicalFileSystem(@"..\..\wwwroot")
                    });
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        RequestPath = new PathString("/cedarjs"),
                        FileSystem = new PhysicalFileSystem(@"..\..\..\Cedar.HttpCommandHandling.Js")
                    });
                    app.Map("/test/commands", commandsApp => commandsApp.Use(commandHandlingMiddleware));
                }))
            {
                Process.Start("http://localhost:8080/index.html");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}