
namespace LeaveManagement.Data
{
    public static class DbInitializer
    {
        // Seed initial data
        public static void seed(AppDBContext context)
        {
            // vérification de l'existance des employees  pour que éviter l'insertion des employee à chaque  démmarage 
            if(context.Employees.Any()) return;
            var employees = new List<Employee>
            {
                new Employee {FullName="Firas Ben Haj Yedder", Departement="RH", JoiningDate= DateTime.Now.AddYears(-2)},
                new Employee {FullName="Fadoua Ben Haj Yedder", Departement="IT", JoiningDate= DateTime.Now.AddYears(-1)}
            };
            context.Employees.AddRange(employees);
            context.SaveChanges();

            context.LeaveRequests.Add(new LeaveRequest
            {
                EmployeeId = employees[0].Id,
                LeaveType = LeaveType.Annual,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(15),
                status = LeaveStatus.Pending, 
                Reason = "Vacances d'été",
                CreatedAt = DateTime.Now
            });
            context.SaveChanges();
        }
    }
}