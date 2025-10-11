using Microsoft.AspNetCore.Identity;

namespace AuthService.Api.Data;

public static class Seed
{
    public static async Task RunAsync(IServiceProvider sp)
    {
        var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = sp.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = new[] { "Admin", "Scorer", "Viewer" };
        foreach (var r in roles)
            if (!await roleMgr.RoleExistsAsync(r))
                await roleMgr.CreateAsync(new IdentityRole(r));

        async Task<IdentityUser> EnsureUser(string email, string role)
        {
            var u = await userMgr.FindByEmailAsync(email);
            if (u == null)
            {
                u = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                await userMgr.CreateAsync(u, "ChangeMe!123");
            }
            if (!await userMgr.IsInRoleAsync(u, role))
                await userMgr.AddToRoleAsync(u, role);
            return u;
        }

        await EnsureUser("admin@local",  "Admin");
        await EnsureUser("scorer@local", "Scorer");
        await EnsureUser("viewer@local", "Viewer");
    }
}
