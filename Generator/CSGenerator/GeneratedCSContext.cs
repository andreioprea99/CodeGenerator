

using CodeGenerator.Models;
using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator.Generator
{
    public class GeneratedCSContext : GeneratedCSClass
    {
        List<string> Instructions { get; set; } = new List<string>();
        public GeneratedCSContext(MainRequestDTO specs, string repositoryName)
        {
            //Set name
            Name = $"{repositoryName}DBContext";
            Namespace = "External";
            //Set dependencies
            ImportDependencies.Add("Microsoft.EntityFrameworkCore");
            ImportDependencies.Add("Npgsql");
            ImportDependencies.Add("System.Linq");

            var repository = specs.Repositories.Where(repository => repository.Name == repositoryName).First();
            var dtos = specs.DTOs.Where(dto => repository.DTOs.Contains(dto.Name));
            HashSet<EntityModel> entities = new HashSet<EntityModel>();
            foreach (var dto in dtos)
            {
                var entityNames = dto.Fields.Select(dtoField => dtoField.Projecting.EntityName);
                foreach (var entityName in entityNames)
                {
                    entities.Add(specs.Entities.Where(entity => entity.Name == entityName).First());
                }
            }
            //Compute instructions
            Instructions.Add($"public {Name}(DbContextOptions <{Name}> options) : base(options)");
            Instructions.Add("{");
            Instructions.Add("}");
            Instructions.Add($"protected override void OnModelCreating(ModelBuilder modelBuilder)");
            Instructions.Add("{");
            //Compute context for a entity
            foreach (var entity in entities)
            {
                Fields.Add(new GeneratedCSField($"DbSet<{entity.Name}>", $"{entity.Name}s"));
                //Table definition
                Instructions.Add($"modelBuilder.Entitiy<{entity.Name}>().ToTable(\"{entity.Name.ToLower()}s\");");
                foreach (var field in entity.Fields)
                {
                    string columnType = "";
                    switch (field.Type)
                    {
                        case EntityFieldType.Int:
                            columnType = "integer";
                            break;
                        case EntityFieldType.String:
                            columnType = $"varchar({field.MaxSize})";
                            break;
                        case EntityFieldType.Long:
                            columnType = "bignit";
                            break;
                        case EntityFieldType.DateTime:
                            columnType = "date";
                            break;
                        case EntityFieldType.Boolean:
                            columnType = "boolean";
                            break;
                    }
                    Instructions.Add($"modelBuilder.Entity<{entity.Name}>().Property(e => e.{field.Name})" +
                        $".HasColumnType(\"{columnType}\")" +
                        $".HasColumnName(\"{field.Name}\")" +
                        $"{ (field.PrimaryKey ? ".ValueGeneratedOnAdd()" : "") }" +
                        $"{ (field.NotNull || field.PrimaryKey ? ".IsRequired(true)" : "") };");
                    if (field.PrimaryKey)
                    {
                        Instructions.Add($"modelBuilder.Entity<{entity.Name}>().HasKey(e => e.{field.Name}).HasName(\"{field.Name.ToLower()}_primary\");");
                    }

                    foreach (var reference in field.References)
                    {
                        string sourceRelathionsip = "";
                        string targetRelathionsip = "";
                        switch (reference.Type)
                        {
                            case EntityFieldReferenceType.OneToOne:
                                targetRelathionsip = "WithOne";
                                sourceRelathionsip = "HasOne";
                                break;

                            case EntityFieldReferenceType.ManyToOne:
                                targetRelathionsip = "WithMany";
                                sourceRelathionsip = "HasOne";
                                break;
                            case EntityFieldReferenceType.ManyToMany:
                                targetRelathionsip = "WithMany";
                                sourceRelathionsip = "HasMany";
                                break;
                        }
                        Instructions.Add($"modelBuilder.Entity<{entity.Name}>()" +
                            $".{sourceRelathionsip}(e => e.{reference.ReferencedEntity}Navigation)" +
                            $".{targetRelathionsip}(e => e.{entity.Name}Navigation)" +
                            $".HasForeignKey(e => e.{field.Name})" +
                            $".OnDelete(DeleteBehaviour.Cascade);");
                    }

                }

            }
            Instructions.Add("}");
        }

        public override string ToString()
        {
            var dependencies = string.Join("\n", ImportDependencies.Select(dependency => $"using {dependency};"));
            var fields = string.Join("\n", Fields) + "\n";
            var content = string.Join("\n", Instructions) + "\n";
            return dependencies + "\n"
                + "namespace " + Namespace + "\n"
                + "{\n"
                + "public class " + Name + $"{(ImplementsInterface == null ? "" : $" : {ImplementsInterface}")}" + "\n"
                + "{\n"
                + fields
                + content
                + "}\n"
                + "}\n"
                ;
        }
    }
}
