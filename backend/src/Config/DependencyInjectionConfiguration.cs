using Microsoft.Extensions.DependencyInjection;
using MyApp.Repositories;
using MyApp.Services;

namespace MyApp.Config
{
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection AddAplicacao(this IServiceCollection services)
        {
            // Registra os repositórios e serviços
            services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            services.AddScoped<IFornecedorService, FornecedorService>();
            
            return services;
        }
    }
}
