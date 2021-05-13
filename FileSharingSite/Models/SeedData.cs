using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FileSharingSite.Data;
using System;
using System.Linq;

namespace FileSharingSite.Models
{
    public static class SeedData
    {
        public static void InitializeUser(IServiceProvider serviceProvider)
        {
            using (var context = new Data.ApplicationContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<Data.ApplicationContext>>()))
            {
                // Look for any Catalogs.
                if (context.User.Any())
                {
                    return;   // DB has been seeded
                }


                context.User.AddRange(

                new UserModel
                {
                   Login = "admin",
                   Password = "12345"
                }


                );

                context.SaveChanges();

            }
        }
        public static void InitializeCatalog(IServiceProvider serviceProvider)
        {
            using (var context = new Data.ApplicationContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<Data.ApplicationContext>>()))
            {
                // Look for any Catalogs.
                if (context.Catalog.Any())
                {
                    return;   // DB has been seeded
                }


                context.Catalog.AddRange(
                new CatalogModel
                {
                    Name = "Изображения"
                },

                new CatalogModel
                {
                    Name = "Видео"
                },

                new CatalogModel
                {
                    Name = "Музыка"
                },

                new CatalogModel
                {
                    Name = "Программы"
                },

                new CatalogModel
                {
                    Name = "Документы"
                }
                );
                context.SaveChanges();
                
            }
        }

        public static void InitializeFiles(IServiceProvider serviceProvider)
        {
            using (var context = new Data.ApplicationContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<Data.ApplicationContext>>()))
            {
                // Look for any Catalogs.
                if (context.File.Any())
                {
                    return;   // DB has been seeded
                }


                context.File.AddRange(

                new FileModel
                {
                    Name = "FPSMonitor.txt",
                    UserId = context.User.FirstOrDefault().Id,
                    CatalogId = context.Catalog.FirstOrDefault().Id,
                    UploadDate = DateTime.Now,
                    FilePath = "C:\\Users\\Федя\\Documents\\FPSMonitor.txt",
                    Annotation = "Test",
                    Size = "63 КБ"
                }

               
                );
                
                context.SaveChanges();
                
            }
        }
    }
}
