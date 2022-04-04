// See https://aka.ms/new-console-template for more information
using Migrator;

new DbMigrator(typeof(Program).Assembly).Migrate();
