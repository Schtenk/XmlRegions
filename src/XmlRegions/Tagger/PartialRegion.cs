namespace XmlRegions
{
    /// <summary>
    /// Implements a partial region, i.e. a region without and end line.
    /// </summary>
    internal class PartialRegion
    {
        #region Properties

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the parent region.
        /// </summary>
        /// <value>
        /// The partial parent.
        /// </value>
        public PartialRegion PartialParent { get; set; }

        /// <summary>
        /// Gets or sets the start line.
        /// </summary>
        /// <value>
        /// The start line.
        /// </value>
        public int StartLine { get; set; }

        /// <summary>
        /// Gets or sets the start offset.
        /// </summary>
        /// <value>
        /// The start offset.
        /// </value>
        public int StartOffset { get; set; }

        #endregion Properties
    }
}