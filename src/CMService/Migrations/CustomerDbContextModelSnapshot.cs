using CMService.Models;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using System;

namespace CMService.Migrations
{
    [ContextType(typeof(CustomerDbContext))]
    partial class CustomerDbContextModelSnapshot : ModelSnapshot
    {
        public override void BuildModel(ModelBuilder builder)
        {
            builder
                .Annotation("SqlServer:DefaultSequenceName", "DefaultSequence")
                .Annotation("SqlServer:Sequence:.DefaultSequence", "'DefaultSequence', '', '1', '10', '', '', 'Int64', 'False'")
                .Annotation("SqlServer:ValueGeneration", "Sequence");
            
            builder.Entity("Entities.Customer", b =>
                {
                    b.Property<int>("Id")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("AddressLine1");
                    
                    b.Property<string>("Category");
                    
                    b.Property<string>("Country");
                    
                    b.Property<DateTime>("DateOfBirth");
                    
                    b.Property<string>("Gender");
                    
                    b.Property<int>("HouseNumber");
                    
                    b.Property<string>("Name");
                    
                    b.Property<string>("State");
                    
                    b.Key("Id");
                });
            
            builder.Entity("Entities.CustomerUpdate", b =>
                {
                    b.Property<int>("Id")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<int>("CustomerId");
                    
                    b.Property<DateTime>("Timestamp");
                    
                    b.Property<string>("Type");
                    
                    b.Key("Id");
                });
            
            builder.Entity("Entities.CustomerUpdate", b =>
                {
                    b.Reference("Entities.Customer")
                        .InverseCollection()
                        .ForeignKey("CustomerId");
                });
        }
    }
}
