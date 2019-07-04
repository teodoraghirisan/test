using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamenNet.Models
{
    public class ExamDbSeeder
    {
        public static void Initialize(ExamDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any expense.
            //    if (context.Expenses.Any())
            //    {
            //        return;   // DB has been seeded
            //    }

            //    context.Expenses.AddRange(
            //        new Expense
            //        {
            //            Description = "Descrip",
            //            Type = Type.Electronics,
            //            Location = "Oradea",
            //            Date = Convert.ToDateTime("2019-05-05T11:11:11"),
            //            Currency = "USD",
            //            Sum = 454.77
            //        },
            //        new Expense
            //        {
            //            Description = "Alta",
            //            Type = Type.Other,
            //            Location = "Covasna",
            //            Date = Convert.ToDateTime("2019-05-07T10:10:10"),
            //            Currency = "EURO",
            //            Sum = 617.55
            //        }
            //    );
            //    context.SaveChanges();

        }
    }
}

