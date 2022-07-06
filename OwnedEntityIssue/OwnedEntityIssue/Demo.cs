using System;
using NUnit.Framework;

namespace OwnedEntityIssue;

public class Demo
{
    public static readonly string SQ_LITE_CONNECTION_STRING = "Filename=TestDatabase.db";
    private TestDbContext _dbContext = null!;

    [Test]
    public void WhenValue1ExistsAndUpdateValue2_ShouldDeleteValue1()
    {
        var entity = new Entity
        {
            Id = Guid.NewGuid(),
            Owned = new Owned(new ColumnValue("123"), null),
        };
        _dbContext.Entities.Add(entity);
        _dbContext.SaveChanges();

        var created = _dbContext.Entities.Find(entity.Id)!;
        created.Owned = new Owned(created.Owned.Value1, new ColumnValue("321"));
        _dbContext.SaveChanges();

        // Owned property null, even if it object referenced
        Assert.Null(created.Owned.Value1);
    }

    [Test]
    public void WhenBothExistsAndUpdateValue2_ShouldDeleteBoth()
    {
        var entity = new Entity
        {
            Id = Guid.NewGuid(),
            Owned = new Owned(new ColumnValue("123"), new ColumnValue("321")),
        };
        _dbContext.Entities.Add(entity);
        _dbContext.SaveChanges();

        var created = _dbContext.Entities.Find(entity.Id)!;
        created.Owned = new Owned(created.Owned.Value1, created.Owned.Value2);
        _dbContext.SaveChanges();

        // Owned properties null, even if it referenced
        Assert.Null(created.Owned.Value1);
        Assert.Null(created.Owned.Value2);
    }

    [Test]
    public void WhenValue1ExistsAndUpdateValue2WithRecreation_ShouldNotDeleteAnything()
    {
        var entity = new Entity
        {
            Id = Guid.NewGuid(),
            Owned = new Owned(new ColumnValue("123"), null),
        };
        _dbContext.Entities.Add(entity);
        _dbContext.SaveChanges();

        var created = _dbContext.Entities.Find(entity.Id)!;
        created.Owned = new Owned(new ColumnValue(created.Owned.Value1!.Value), new ColumnValue("321"));
        _dbContext.SaveChanges();

        // When references updated, properties not null
        Assert.NotNull(created.Owned.Value1);
        Assert.NotNull(created.Owned.Value2);
    }

    [SetUp]
    public void SetUp()
    {
        _dbContext = new TestDbContext();
        _dbContext.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Dispose();
    }
}