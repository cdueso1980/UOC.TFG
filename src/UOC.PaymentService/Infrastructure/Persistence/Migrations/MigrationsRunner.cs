using System;
using Npgsql;
using Microsoft.Extensions.Logging;
using UOC.SharedContracts;

namespace UOC.PaymentService.Infrastructure.Persistence
{
    public sealed class MigrationsRunner : IMigrationsRunner
    {
        private readonly ILogger logger;
        private readonly PostgresqlConfigOptions postgresqlConfigOptions;
        
        public MigrationsRunner(ILogger<MigrationsRunner> logger, PostgresqlConfigOptions postgresqlConfigOptions)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.postgresqlConfigOptions = postgresqlConfigOptions ?? throw new ArgumentNullException(nameof(postgresqlConfigOptions));
        }

        public void Migrate()
        {
            using (var dbConnection = new NpgsqlConnection(postgresqlConfigOptions.Connection)) 
            {
                var evolve = new Evolve.Evolve(dbConnection, message => this.logger.LogInformation(message));

                evolve.IsEraseDisabled = false;
                evolve.EmbeddedResourceAssemblies = new[] { this.GetType().Assembly };
                evolve.MetadataTableSchema = "payment";
                evolve.MetadataTableName = "migrations";
                evolve.Schemas = new [] { "payment" };

                evolve.Erase();
                evolve.Migrate();
                evolve.Info();
            }
        }
    }
}