using System.Data.Entity.Core.Objects;

namespace HtmlHelpers.BeginCollectionItem
{
    public static class ViewFunctions
    {
        public static string GetPartialViewName<T>(this T entity) where T : class
        {
            string entityTypeName = ObjectContext.GetObjectType(entity.GetType()).Name;

            return $"_{entityTypeName.Substring(0, 1).ToLower()}{entityTypeName.Remove(0, 1)}";
        }
    }
}