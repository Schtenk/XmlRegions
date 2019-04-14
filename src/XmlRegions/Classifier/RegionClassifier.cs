using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;

namespace XmlRegions
{
    internal class RegionClassifier : IClassifier
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionClassifier"/> class.
        /// </summary>
        /// <param name="registry">The registry.</param>
        public RegionClassifier(IClassificationTypeRegistryService registry)
        {
            _classificationType = registry.GetClassificationType(PredefinedClassificationTypeNames.ExcludedCode);
        }

        #endregion Constructors

        #region Fields

        private readonly IClassificationType _classificationType;

        #endregion Fields

        #region Events

        /// <summary>
        /// Occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event does not need to be raised for newly-inserted text.
        /// However, it should be raised if any text other than that which was actually inserted has been reclassified.
        /// It should also be raised if the deletion of text causes the remaining
        /// text to be reclassified.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged
        {
            add { /* Intentionally left empty */ }
            remove { /* Intentionally left empty */ }
        }

        #endregion Events

        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets all the <see cref="Microsoft.VisualStudio.Text.Classification.ClassificationSpan" /> objects that intersect the given range of text.
        /// </summary>
        /// <param name="span">The snapshot span.</param>
        /// <returns>
        /// A list of <see cref="Microsoft.VisualStudio.Text.Classification.ClassificationSpan" /> objects that intersect with the given range.
        /// </returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>();

            if (span.IsEmpty)
                return result;

            var text = span.GetText();

            if (string.IsNullOrWhiteSpace(text))
                return result;

            if (RegionRegex.Region.IsMatch(text))
                result.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, span), _classificationType));

            return result;
        }

        #endregion Public Methods

        #endregion Methods
    }
}