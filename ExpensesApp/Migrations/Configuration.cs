namespace ExpensesApp.Migrations
{
    using ExpensesApp.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ExpensesApp.Context.ExpensesAppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ExpensesApp.Context.ExpensesAppContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            context.Users.AddOrUpdate(new User[]
            {
                new User()
                {
                    Id =  1,
                    Name = "Test User",
                    Email = "test@gmail.com",
                    Password = "12345678"
                }
            });

            context.Expenses.AddOrUpdate(new Expense[] {
                new Expense() {
                    Id = 1,
                    Description = "Water Bill",
                    Value = 10.20m,
                    type = 1,
                    UserId = 1
                }
            });
        }
    }
}
