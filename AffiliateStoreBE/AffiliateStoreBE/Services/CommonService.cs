using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Reflection;
using System.Net.NetworkInformation;

namespace AffiliateStoreBE.Services
{
    public class CommonService
    {
        protected IQueryable<T> DoSearch<T>(IQueryable<T> query, FilterModel filter)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "a");
            if (!string.IsNullOrEmpty(filter.SearchText) && !string.IsNullOrEmpty(filter.SearchFields))
            {
                Expression<Func<T, bool>> lamada = a => false;
                var fields = filter.SearchFields.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                Expression whereExpr = null;
                foreach (var field in fields)
                {
                    MemberExpression member = GetMemberExpression(parameter, field);
                    Expression expRes;
                    if (filter.IsSearchIgnoreCase && !(string.Equals(filter.SearchText, filter.SearchText.ToLower(), StringComparison.Ordinal) &&
                        string.Equals(filter.SearchText, filter.SearchText.ToUpper(), StringComparison.Ordinal)))
                    {
                        var stringContains = typeof(string).GetMethods().FirstOrDefault(a => a.Name == nameof(string.Contains) && a.GetParameters().Length == 2);
                        expRes = Expression.Call(member, stringContains, Expression.Constant(filter.SearchText.Trim()), Expression.Constant(StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        //var stringContains = typeof(string).GetMethods().FirstOrDefault(a => a.Name == nameof(string.Contains) && a.GetParameters().Length == 1);
                        //expRes = Expression.Call(member, stringContains, Expression.Constant(filter.SearchText.Trim()));

                        expRes = query.InternalLikeCaseInsensitive(parameter, new List<string>() { field }, filter.SearchText.Trim());
                    }
                    if (whereExpr == null)
                    {
                        whereExpr = expRes;
                    }
                    else
                    {
                        whereExpr = Expression.Or(whereExpr, expRes);
                    }
                }
                lamada = Expression.Lambda<Func<T, bool>>(whereExpr, parameter);
                query = query.Where(lamada);
            }
            return query;
        }

        private MemberExpression GetMemberExpression(ParameterExpression parameter, string field)
        {
            if (field.Contains("."))
            {
                var items = field.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                MemberExpression result = null;
                foreach (var item in items)
                {
                    if (result == null)
                    {
                        result = Expression.PropertyOrField(parameter, item);
                    }
                    else
                    {
                        result = Expression.PropertyOrField(result, item);
                    }
                }
                return result;
            }
            else
            {
                return Expression.PropertyOrField(parameter, field);
            }
        }

        public static Expression InternalLikeCaseInsensitive<TEntity>(this IQueryable<TEntity> source, ParameterExpression parameter, List<string> propertyNames, string searchKeyword, SearchType searchType = SearchType.All)
        {
            var dbContext = GetDbContext(source);
            Expression body = default(MethodCallExpression);
            {
                if (dbContext.Database.IsSqlServer())
                {
                    body = propertyNames.Select(propertyName => Expression.Call(typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like),
                                                                                                            new[]
                                                                                                            {
                                                                                                        typeof(DbFunctions), typeof(string), typeof(string)
                                                                                                            }),
                                                                    Expression.Constant(EF.Functions),
                                                                    GetMemberExpression(parameter, propertyName),
                                                                    Expression.Constant(GetSearchKeyword(searchKeyword, searchType))))
                .Aggregate<MethodCallExpression, Expression>(null, (current, call) => current != null ? Expression.OrElse(current, call) : (Expression)call);

                    //body = Expression.Call(typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like),
                    //                                                                                        new[]
                    //                                                                                        {
                    //                                                                                    typeof(DbFunctions), typeof(string), typeof(string)
                    //                                                                                        }),
                    //                                                Expression.Constant(EF.Functions),
                    //                                                GetMemberExpression(parameter, propertyName),
                    //                                                Expression.Constant(GetSearchKeyword(searchKeyword, searchType)));
                }
                else
                {
                    //body = Expression.Call(typeof(NpgsqlDbFunctionsExtensions).GetMethod(nameof(NpgsqlDbFunctionsExtensions.ILike),
                    //                                                                                        new[]
                    //                                                                                        {
                    //                                                                                    typeof(DbFunctions), typeof(string), typeof(string)
                    //                                                                                        }),
                    //                                                Expression.Constant(EF.Functions),
                    //                                                GetMemberExpression(parameter, propertyName),
                    //                                                Expression.Constant(GetSearchKeyword(searchKeyword, searchType)));

                    string escapeCharacter = "";
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        if (searchKeyword.Contains("%") || searchKeyword.Contains("_") || searchKeyword.Contains("\\"))
                        {
                            escapeCharacter = @"\";
                        }
                        searchKeyword = searchKeyword.Replace("\\", "\\\\");
                        searchKeyword = searchKeyword.Replace("_", @"\_");
                        searchKeyword = searchKeyword.Replace("%", @"\%");
                    }

                    body = propertyNames.Select(propertyName => Expression.Call(typeof(NpgsqlDbFunctionsExtensions).GetMethod(nameof(NpgsqlDbFunctionsExtensions.ILike),
                                                                                                            new[]
                                                                                                            {
                                                                                                        typeof(DbFunctions), typeof(string), typeof(string), typeof(string)
                                                                                                            }),
                                                                    Expression.Constant(EF.Functions),
                                                                    GetMemberExpression(parameter, propertyName),
                                                                    Expression.Constant(GetSearchKeyword(searchKeyword, searchType)),
                                                                    Expression.Constant(escapeCharacter)
                                                                    ))
                .Aggregate<MethodCallExpression, Expression>(null, (current, call) => current != null ? Expression.OrElse(current, call) : (Expression)call);
                }
            }
            return body;
        }

        private static DbContext GetDbContext(IQueryable query)
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var queryCompiler = typeof(EntityQueryProvider).GetField("_queryCompiler", bindingFlags).GetValue(query.Provider);
            var queryContextFactory = queryCompiler.GetType().GetField("_queryContextFactory", bindingFlags).GetValue(queryCompiler);

            var dependencies = typeof(RelationalQueryContextFactory).GetProperty("Dependencies", bindingFlags).GetValue(queryContextFactory);
            var queryContextDependencies = typeof(DbContext).Assembly.GetType(typeof(QueryContextDependencies).FullName);
            var stateManagerProperty = queryContextDependencies.GetProperty("StateManager", bindingFlags | BindingFlags.Public).GetValue(dependencies);
            var stateManager = (IStateManager)stateManagerProperty;

            return stateManager.Context;
        }

        private static string GetSearchKeyword(string searchKeyword, SearchType searchType)
        {
            switch (searchType)
            {
                default:
                case SearchType.All:
                    searchKeyword = $"%{searchKeyword.Trim()}%";
                    break;

                case SearchType.Left:
                    searchKeyword = $"%{searchKeyword.Trim()}";
                    break;

                case SearchType.Right:
                    searchKeyword = $"{searchKeyword.Trim()}%";
                    break;
            }
            return searchKeyword;
        }
    }
    public class FilterModel
    {
        //key:fieldName, value:filter list
        public Dictionary<string, List<FilterItem>> Filters { get; set; }

        //用于获取后台缓存的Filter cache， 如果需要刷新filter 缓存的话请置空该值
        public string FilterKey { get; set; }

        //Paging
        public virtual int Offset { get; set; } = 1;

        public virtual int Limit { get; set; } = 10;

        //Sort: SortBy需要是查询出来数据的字段名
        public string SortBy { get; set; }

        public bool IsASC { get; set; }

        //Search
        public string SearchText { get; set; }

        //如果想支持search多个字段, 请使用;分隔, 目前只支持string类型字段的search
        public virtual string SearchFields { get; set; }

        public virtual bool IsSearchIgnoreCase { get; set; } = false;
    }

    public enum SearchType
    {
        /// <summary>
        /// %____%
        /// </summary>
        All,

        /// <summary>
        /// %____
        /// </summary>
        Left,

        /// <summary>
        /// ____%
        /// </summary>
        Right
    }
}
