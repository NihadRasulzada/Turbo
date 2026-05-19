using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Turbo.Shared.Application.Context;

namespace Turbo.Shared.Application.Extensions;

public static class DbContextFkExtensions
{
    public static async Task<bool> HasForeignKeyReferencesAsync<TEntity>(
        this AppDbContext context,
        Guid id
    )
        where TEntity : class
    {
        IEntityType? entityType = context.Model.FindEntityType(typeof(TEntity));

        if (entityType == null)
            return false;

        foreach (IForeignKey fk in entityType.GetReferencingForeignKeys())
        {
            Type dependentType = fk.DeclaringEntityType.ClrType;
            string fkProperty = fk.Properties.First().Name;

            // ✅ DbContext.Set<DependentType>()
            MethodInfo setMethod = typeof(AppDbContext)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .First(m => m.Name == "Set" && m.IsGenericMethod && m.GetParameters().Length == 0);

            object dbSet = setMethod.MakeGenericMethod(dependentType).Invoke(context, null)!;

            // x =>
            ParameterExpression parameter = Expression.Parameter(dependentType, "x");

            // x.FkProperty
            MemberExpression property = Expression.Property(parameter, fkProperty);

            // == id
            ConstantExpression constant = Expression.Constant(id);
            BinaryExpression equal = Expression.Equal(property, constant);

            LambdaExpression lambda = Expression.Lambda(equal, parameter);

            // Queryable.Any<DependentType>(source, predicate)
            MethodInfo anyMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                .MakeGenericMethod(dependentType);

            IQueryable queryable = ((IQueryable)dbSet);

            bool result = (bool)anyMethod.Invoke(null, new object[] { queryable, lambda })!;

            if (result)
                return true;
        }

        return false;
    }
}
