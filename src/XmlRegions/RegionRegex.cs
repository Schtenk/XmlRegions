using System.Text.RegularExpressions;

namespace XmlRegions
{
    /// <summary>
    /// Provides regular expressions to match XML regions.
    /// </summary>
    public static class RegionRegex
    {
        #region Properties

        /// <summary>
        /// Gets the regular expression to match the start or end of a XML region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        public static Regex Region { get; } = new Regex(@"^(\s*)(<!-- ?#(end)?region)(.*?)-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Gets the regular expression to match the end of a XML region.
        /// </summary>
        /// <value>
        /// The region end.
        /// </value>
        public static Regex RegionEnd { get; } = new Regex(@"^(\s*)<!-- ?#endregion(.*?)-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Gets the regular expression to match the start of a XML region.
        /// </summary>
        /// <value>
        /// The region start.
        /// </value>
        public static Regex RegionStart { get; } = new Regex(@"^(\s*)<!-- ?#region(.*?)-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion Properties
    }
}