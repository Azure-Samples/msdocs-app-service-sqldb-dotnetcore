using System.Linq;
using System;
namespace DotNetCoreSqlDb.Models
{
    public static class DbInitializer
    {
        public static void Initializer(MyDatabaseContext context)
        {
            context.Database.EnsureCreated();
            if(context.Todo.Any())
            {
                return;
            }

            var todo = new Todo[]
            {
                new Todo{Description="Description 1",CreatedDate= DateTime.Parse("2012-09-01")}
            };
            foreach (Todo item in todo)
            {
                context.Todo.Add(item);
            }
            context.SaveChanges();
        }
    }
}