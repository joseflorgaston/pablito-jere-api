using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;



namespace PablitoJere.Services
{
    public class InitDatabaseService
    {
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public InitDatabaseService()
        {

        }

        public async Task InitDatabase(IServiceProvider services)
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                _userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await AddUserRolesIfNotExist(_roleManager);
                await AddUsersIfNotExistAsync(_userManager);

            }
        }

        public async Task AddUserRolesIfNotExist(RoleManager<IdentityRole> roleManager)
        {
            List<Task<IdentityResult>> tasks = new List<Task<IdentityResult>>();

            if (!(await roleManager.RoleExistsAsync("ADMIN")))
            {
                tasks.Append(CreateRole("ADMIN"));
            }
            if(!(await roleManager.RoleExistsAsync("USER")))
            {
                tasks.Append(CreateRole("USER"));
            }
            await Task.WhenAll(tasks);
        }
        public async Task AddUsersIfNotExistAsync(UserManager<IdentityUser> userManager)
        {
            var user = await userManager.FindByNameAsync("pablitojere");
            if (user == null)
            {
                user = new IdentityUser() { 
                    UserName = "pablitojere", AccessFailedCount = 0,
                    Email = "comisionpablitojere@gmail.com"
                };
                await userManager.CreateAsync(user, "aA123456!");
                var currentUser = await userManager.FindByNameAsync(user.UserName);
                await userManager.AddToRoleAsync(currentUser, "ADMIN");
            }
        }
        private Task<IdentityResult> CreateRole(string roleName)
        {
            var role = new IdentityRole();
            role.Name = roleName;
            return _roleManager.CreateAsync(role);
        }
    }
}
