using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpManual.Helpers
{
    public class ApplicationConfigurationHandler : AuthorizationHandler<ApplicationConfiguration>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApplicationConfiguration requirement)
        {
            var userName = context.User.Identity.Name.ToLowerInvariant();

            if (requirement.AdminNames.ConvertAll(d => d.ToLower()).Contains(userName))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask; // if it does not compile use Task.FromResult(0)
        }
    }

    public class ApplicationConfiguration : IAuthorizationRequirement
    {
        public List<string> AdminNames { get; set; }
        public ApplicationConfiguration(List<string> userNames)
        {
            AdminNames = userNames;
        }
    }
}
