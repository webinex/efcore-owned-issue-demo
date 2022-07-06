using System;
using Microsoft.EntityFrameworkCore;

namespace OwnedEntityIssue;

public class ColumnValue
{
    protected ColumnValue()
    {
    }

    public ColumnValue(string value)
    {
        Value = value;
    }

    public string Value { get; protected set; }
}

public class Owned
{
    protected Owned()
    {
    }

    public Owned(ColumnValue? value1, ColumnValue? value2)
    {
        Value1 = value1;
        Value2 = value2;
    }

    public ColumnValue? Value1 { get; protected set; }
    public ColumnValue? Value2 { get; protected set; }
}

public class Entity
{
    public Guid Id { get; set; }
    public Owned Owned { get; set; } = null!;
}

public class TestDbContext : DbContext
{
    public TestDbContext()
        : base(new DbContextOptionsBuilder<TestDbContext>().UseSqlite(Demo.SQ_LITE_CONNECTION_STRING).Options)
    {
    }

    public DbSet<Entity> Entities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Entity>(entity =>
        {
            entity.ToTable("entities", "test");
            entity.Property(x => x.Id).HasColumnName("id");
            entity.HasKey(x => x.Id);
            entity.OwnsOne(x => x.Owned,
                o =>
                {
                    o.OwnsOne(
                        x => x.Value1,
                        v => v.Property(x => x.Value).HasColumnName("value1"));
                    o.OwnsOne(
                        x => x.Value2,
                        v => v.Property(x => x.Value).HasColumnName("value2"));
                });
        });

        base.OnModelCreating(model);
    }
}