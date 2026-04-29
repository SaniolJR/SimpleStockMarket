using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Tests;

internal static class EfMockHelpers
{
    public static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> sourceList) where T : class
    {
        var queryable = sourceList.AsQueryable();

        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
        mockSet.As<IEnumerable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        mockSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(sourceList.Add);

        // Async support for FirstOrDefaultAsync with predicate
        mockSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .Returns((Expression<Func<T, bool>> predicate, CancellationToken ct) =>
            {
                var compiled = predicate.Compile();
                T result = sourceList.FirstOrDefault(compiled);
                return Task.FromResult(result);
            });

        // Async support for FirstOrDefaultAsync without predicate
        mockSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<CancellationToken>()))
            .Returns((CancellationToken ct) => Task.FromResult(sourceList.FirstOrDefault()));

        return mockSet;
    }
}