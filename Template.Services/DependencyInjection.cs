using Microsoft.Extensions.DependencyInjection;
using Template.Services.Implementations;
using Template.Services.Interfaces;

namespace Template.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IItemService, ItemService>();
            return services;
        }
    }
}
