using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Enterprise.Domain.Entities;
using Enterprise.Domain.Enums;

namespace Enterprise.Infrastructure;

public static class DbSeeder
{
    public static async Task SeedAsync(EnterpriseDbContext context)
    {
        // 1. Force ensure migrations are applied
        await context.Database.MigrateAsync();

        // 2. If we already have 100+ employees, skip seeding
        if (await context.Employees.CountAsync() >= 100)
        {
            return;
        }

        // 3. Clear manager references first to break circular foreign key dependency cycle
        var existingDepts = await context.Departments.ToListAsync();
        foreach (var dept in existingDepts)
        {
            dept.AssignManager(null);
        }
        await context.SaveChangesAsync();

        // 4. Safe to clear the tables now
        var existingEmployees = await context.Employees.ToListAsync();
        context.Employees.RemoveRange(existingEmployees);
        context.Departments.RemoveRange(existingDepts);
        await context.SaveChangesAsync();

        // 5. Seed 5 Departments
        var depts = new List<Department>
        {
            new Department("Engineering", "ENG", null),
            new Department("Human Resources", "HR", null),
            new Department("Sales & Marketing", "MKT", null),
            new Department("Finance & Accounts", "FIN", null),
            new Department("Product Management", "PM", null)
        };
        await context.Departments.AddRangeAsync(depts);
        await context.SaveChangesAsync();

        // 6. Seed 112 Employees programmatically
        var firstNames = new[] { 
            "Aarav", "Aanya", "Arjun", "Ananya", "Vihaan", "Aditi", "Krishna", "Sai", "Pranav", "Nisha", 
            "Rohan", "Pooja", "Vikram", "Kavya", "Siddharth", "Neha", "Rahul", "Shreya", "Varun", "Riya",
            "Dulaj", "Ruwan", "Nirmal", "Sithara", "Harini", "Chathura", "Ishara", "Nuwan", "Amara", "Piyumi"
        };
        var lastNames = new[] { 
            "Perera", "Silva", "Fernando", "Jayawardena", "Amarasinghe", "Gunawardena", "De Alwis", "Dias", "Herath", "Karunaratne", 
            "Mendis", "Peiris", "Ranasinghe", "Samarasinghe", "Wijesinghe", "Senanayake", "Pathirana", "Liyanage", "Cooray", "Rodrigo",
            "Hettiarachchi", "Ekanayake", "Jayasinghe", "Rajapakse", "Bandara", "Rathnayake", "Dissanayake", "Dharshana", "Premarathne", "Goonetilleke"
        };

        var districts = new[] { "Colombo", "Gampaha", "Kalutara", "Kandy", "Galle" };
        var districtCities = new Dictionary<string, (string City, string Code)[]>
        {
            ["Colombo"] = new[] { ("Colombo 03", "00300"), ("Dehiwala", "10350"), ("Moratuwa", "10400") },
            ["Gampaha"] = new[] { ("Negombo", "11500"), ("Gampaha", "11000"), ("Kelaniya", "11600") },
            ["Kalutara"] = new[] { ("Kalutara", "12000"), ("Panadura", "12500"), ("Horana", "12400") },
            ["Kandy"] = new[] { ("Kandy", "20000"), ("Peradeniya", "20400"), ("Katugastota", "20800") },
            ["Galle"] = new[] { ("Galle", "80000"), ("Hikkaduwa", "80240"), ("Ambalangoda", "80300") }
        };

        var jobTitles = new[] { 
            "Software Engineer", "Senior Software Engineer", "Tech Lead", "QA Engineer", "Product Manager", 
            "HR Generalist", "HR Manager", "Sales Executive", "Marketing Specialist", "Financial Analyst", "Accountant" 
        };

        var random = new Random(42);
        var employees = new List<Employee>();

        for (int i = 1; i <= 112; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var email = $"{firstName.ToLower()}.{lastName.ToLower()}.{i}@ewms.com";
            var phone = $"+9477{random.Next(1000000, 9999999)}";

            var jobTitle = jobTitles[random.Next(jobTitles.Length)];
            
            // Link job titles to specific seeded departments
            Department dept;
            if (jobTitle.Contains("Software") || jobTitle.Contains("Lead") || jobTitle.Contains("QA"))
                dept = depts[0]; // ENG
            else if (jobTitle.Contains("HR"))
                dept = depts[1]; // HR
            else if (jobTitle.Contains("Sales") || jobTitle.Contains("Marketing"))
                dept = depts[2]; // MKT
            else if (jobTitle.Contains("Financial") || jobTitle.Contains("Accountant"))
                dept = depts[3]; // FIN
            else
                dept = depts[4]; // PM

            // Ensure random hire date spread across the past 3 years
            var joinDate = DateTime.UtcNow.Date.AddDays(-random.Next(15, 1100));
            
            // Direct distributions: Active, Probation
            var status = (EmployeeStatus)random.Next(0, 2);

            // Create valid Sri Lankan NIC
            var isFemale = i % 2 == 0;
            var year = 1975 + (i % 25);
            var dayOfYear = isFemale ? 501 + random.Next(1, 365) : random.Next(1, 365);

            var nicYearPart = (year % 100).ToString("D2");
            var nicDayPart = dayOfYear.ToString("D3");
            var nicSeqPart = (1000 + i).ToString("D4");
            var nic = $"{nicYearPart}{nicDayPart}{nicSeqPart}V";

            var dob = new DateTime(year, 1, 1).AddDays(dayOfYear - 1);
            var gender = isFemale ? Gender.Female : Gender.Male;
            var maritalStatus = (MaritalStatus)random.Next(0, 4);

            var addressLine1 = $"No. {random.Next(1, 250)}, Galle Road";
            var district = districts[random.Next(districts.Length)];
            var cities = districtCities[district];
            var (city, postalCode) = cities[random.Next(cities.Length)];

            var employee = new Employee(
                firstName,
                lastName,
                email,
                phone,
                jobTitle,
                joinDate,
                dept.Id,
                nic,
                dob,
                gender,
                maritalStatus,
                addressLine1,
                district,
                city,
                postalCode,
                status
            );

            employees.Add(employee);
        }

        await context.Employees.AddRangeAsync(employees);
        await context.SaveChangesAsync();

        // 7. Assign managers to seeded departments
        var engManager = employees.FirstOrDefault(e => e.DepartmentId == depts[0].Id && e.JobTitle == "Tech Lead");
        if (engManager != null) depts[0].AssignManager(engManager.Id);

        var hrManager = employees.FirstOrDefault(e => e.DepartmentId == depts[1].Id && e.JobTitle == "HR Manager");
        if (hrManager != null) depts[1].AssignManager(hrManager.Id);

        var mktManager = employees.FirstOrDefault(e => e.DepartmentId == depts[2].Id && e.JobTitle == "Marketing Specialist");
        if (mktManager != null) depts[2].AssignManager(mktManager.Id);

        var finManager = employees.FirstOrDefault(e => e.DepartmentId == depts[3].Id && e.JobTitle == "Accountant");
        if (finManager != null) depts[3].AssignManager(finManager.Id);

        var pmManager = employees.FirstOrDefault(e => e.DepartmentId == depts[4].Id && e.JobTitle == "Product Manager");
        if (pmManager != null) depts[4].AssignManager(pmManager.Id);

        await context.SaveChangesAsync();
    }
}
