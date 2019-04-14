using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace XmlRegions
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("XML")]
    internal sealed class RegionTaggerProvider : ITaggerProvider
    {
        #region Methods

        #region Public Methods

        /// <summary>
        /// Creates a tag provider for the specified buffer.
        /// </summary>
        /// <typeparam name="T">The type of the tag.</typeparam>
        /// <param name="buffer">The <see cref="Microsoft.VisualStudio.Text.ITextBuffer" />.</param>
        /// <returns></returns>
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
            where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new RegionTagger(buffer)) as ITagger<T>;
        }

        #endregion Public Methods

        #endregion Methods
    }
}