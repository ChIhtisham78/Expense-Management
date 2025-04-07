namespace ExpenseManagment.Data.Common
{
    public class AppConfiguration
    {
        public static TConfig Configure<TConfig>(IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)

                throw new ArgumentNullException(nameof(configuration));
            var config = new TConfig();

            var classType = typeof(TConfig);
            configuration.Bind(classType.Name, config);
            services.AddSingleton(config);
            return config;
        }
    }
}


