using System;

namespace TT2Master.Model.Navigation
{
    /// <summary>
    /// Creates navigation path strings
    /// </summary>
    public class NavigationConstants
    {
        /// <summary>
        /// The default path using master detail
        /// </summary>
        public const string DefaultPath = "/MyMasterDetailPage/DefaultNavigationPage/";

        /// <summary>
        /// Returns a path string where parent and child are nested
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static string ChildNavigationPath<TParent, TChild>()
            where TParent : class
            where TChild : class 
            => $"{DefaultPath}{typeof(TParent).Name}/{typeof(TChild).Name}";

        /// <summary>
        /// Returns a path to an element. 
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public static string NavigationPath<TChild>() where TChild : class => DefaultPath + typeof(TChild).Name;
    }
}
