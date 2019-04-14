namespace XmlRegions
{
    /// <summary>
    /// Implements a region.
    /// </summary>
    /// <seealso cref="XmlRegions.PartialRegion" />
    internal class Region : PartialRegion
    {
        #region Properties

        /// <summary>
        /// Gets or sets the end line.
        /// </summary>
        /// <value>
        /// The end line.
        /// </value>
        public int EndLine { get; set; }

        #endregion Properties
    }
}