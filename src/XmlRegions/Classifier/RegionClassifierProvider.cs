using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace XmlRegions
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("XML")]
    [ContentType("AXAML")]
    [Order(After = "Default")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class RegionClassifierProvider : IClassifierProvider
    {
        #region Properties

        /// <summary>
        /// Gets or sets the registry service.
        /// </summary>
        /// <value>
        /// The registry.
        /// </value>
        [Import]
        public IClassificationTypeRegistryService Registry { get; set; }

        #endregion Properties

        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets a classifier for the given text buffer.
        /// </summary>
        /// <param name="textBuffer">The <see cref="Microsoft.VisualStudio.Text.ITextBuffer" /> to classify.</param>
        /// <returns>
        /// A classifier for the text buffer, or null if the provider cannot do so in its current state.
        /// </returns>
        public IClassifier GetClassifier(ITextBuffer textBuffer) => textBuffer.Properties.GetOrCreateSingletonProperty(() => new RegionClassifier(Registry));

        #endregion Public Methods

        #endregion Methods
    }
}