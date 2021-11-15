using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlRegions
{
    internal sealed class RegionTagger : ITagger<IOutliningRegionTag>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionTagger"/> class.
        /// </summary>
        /// <param name="buffer">The text buffer.</param>
        public RegionTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _snapshot = buffer.CurrentSnapshot;

            _buffer.ChangedLowPriority += OnChangedLowPriority;

            Parse();
        }

        #endregion Constructors

        #region Fields

        private readonly ITextBuffer _buffer;
        private readonly List<Region> _regions = new List<Region>();

        private ITextSnapshot _snapshot;

        #endregion Fields

        #region Events

        /// <summary>
        /// Occurs when tags are added to or removed from the provider.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        #endregion Events

        #region Methods

        #region Private Static Methods

        /// <summary>
        /// Gets a new <see cref="SnapshotSpan"/> for the specified <paramref name="region"/> and <paramref name="snapshot"/>.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="snapshot">The snapshot.</param>
        /// <returns></returns>
        private static SnapshotSpan AsSnapshotSpan(Region region, ITextSnapshot snapshot)
        {
            var startLine = snapshot.GetLineFromLineNumber(region.StartLine);
            var endLine = region.StartLine == region.EndLine
                ? startLine
                : snapshot.GetLineFromLineNumber(region.EndLine);

            return new SnapshotSpan(startLine.Start, endLine.End);
        }

        /// <summary>
        /// Parses the specified line, searching for the end of a region.
        /// </summary>
        /// <param name="regions">The regions.</param>
        /// <param name="line">The line.</param>
        /// <param name="text">The text.</param>
        /// <param name="currentRegion">The current region.</param>
        /// <param name="regionStart">The region start.</param>
        private static void ParseRegionEnd(List<Region> regions, ITextSnapshotLine line, string text, ref PartialRegion currentRegion, ref int regionStart)
        {
            var regionEndMatch = RegionRegex.RegionEnd.Match(text);

            if (!regionEndMatch.Success)
                return;

            regionStart = regionEndMatch.Groups[1].Success
                ? regionEndMatch.Groups[1].Length
                : 0;

            if (currentRegion == null)
                return;

            regions.Add(new Region
            {
                Level = currentRegion.Level,
                StartLine = currentRegion.StartLine,
                StartOffset = currentRegion.StartOffset,
                EndLine = line.LineNumber
            });

            currentRegion = currentRegion.PartialParent;
        }

        /// <summary>
        /// Parses the specified line, searching for the start of a region.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="text">The text.</param>
        /// <param name="currentRegion">The current region.</param>
        /// <param name="regionStart">The region start.</param>
        /// <returns></returns>
        private static bool ParseRegionStart(ITextSnapshotLine line, string text, ref PartialRegion currentRegion, ref int regionStart)
        {
            var regionStartMatch = RegionRegex.RegionStart.Match(text);

            if (!regionStartMatch.Success)
                return false;

            regionStart = regionStartMatch.Groups[1].Success
                ? regionStartMatch.Groups[1].Length
                : 0;

            var currentLevel = currentRegion != null
                ? currentRegion.Level
                : 1;

            currentRegion = new PartialRegion
            {
                Level = currentLevel + 1,
                StartLine = line.LineNumber,
                StartOffset = regionStart,
                PartialParent = currentRegion
            };

            return true;
        }

        #endregion Private Static Methods

        #region Public Methods

        /// <summary>
        /// Gets all the tags that intersect the <paramref name="spans" />.
        /// </summary>
        /// <param name="spans">The spans to visit.</param>
        /// <returns>
        /// A <see cref="Microsoft.VisualStudio.Text.Tagging.ITagSpan`1" /> for each tag.
        /// </returns>
        /// <remarks>
        /// <para>Taggers are not required to return their tags in any specific order.</para>
        /// <para>The recommended way to implement this method is by using generators ("yield return"),
        /// which allows lazy evaluation of the entire tagging stack.</para>
        /// </remarks>
        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            var currentRegions = _regions;
            var currentSnapshot = _snapshot;
            var fullSpan = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
            var startLineNumber = fullSpan.Start.GetContainingLine().LineNumber;
            var endLineNumber = fullSpan.End.GetContainingLine().LineNumber;

            foreach (var region in currentRegions)
            {
                if (region.StartLine > endLineNumber || region.EndLine < startLineNumber)
                    continue;

                var startLine = currentSnapshot.GetLineFromLineNumber(region.StartLine);
                var endLine = currentSnapshot.GetLineFromLineNumber(region.EndLine);
                var snapshot = new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End);
                var match = RegionRegex.RegionStart.Match(startLine.GetText());
                var collapsedForm = !string.IsNullOrWhiteSpace(match.Groups[2].Value)
                    ? match.Groups[2].Value.Trim()
                    : "...";
                var collapsedHintForm = snapshot.GetText();

                yield return new TagSpan<IOutliningRegionTag>(snapshot, new OutliningRegionTag(false, true, collapsedForm, collapsedHintForm));
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Called when the <see cref="ITextBuffer"/> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextContentChangedEventArgs"/> instance containing the event data.</param>
        private void OnChangedLowPriority(object sender, TextContentChangedEventArgs e)
        {
            if (e.After != _buffer.CurrentSnapshot)
                return;

            Parse();
        }

        /// <summary>
        /// Parses the current snapshot of <see cref="_buffer"/>.
        /// </summary>
        private void Parse()
        {
            var snapshot = _buffer.CurrentSnapshot;
            var regions = new List<Region>();

            PartialRegion currentRegion = null;

            foreach (var line in snapshot.Lines)
            {
                var regionStart = -1;
                var text = line.GetText();

                if (!ParseRegionStart(line, text, ref currentRegion, ref regionStart))
                    ParseRegionEnd(regions, line, text, ref currentRegion, ref regionStart);
            }

            var oldSpans = new List<Span>(_regions
                .Select(r => AsSnapshotSpan(r, _snapshot)
                .TranslateTo(snapshot, SpanTrackingMode.EdgeExclusive)
                .Span));

            var newSpans = new List<Span>(regions
                .Select(r => AsSnapshotSpan(r, snapshot).Span));

            var oldSpanCollection = new NormalizedSpanCollection(oldSpans);
            var newSpanCollection = new NormalizedSpanCollection(newSpans);

            var removed = NormalizedSpanCollection.Difference(oldSpanCollection, newSpanCollection);

            var changeStart = int.MaxValue;
            var changeEnd = -1;

            if (removed.Count > 0)
            {
                changeStart = removed[0].Start;
                changeEnd = removed[removed.Count - 1].End;
            }

            if (newSpans.Count > 0)
            {
                changeStart = Math.Min(changeStart, newSpans[0].Start);
                changeEnd = Math.Max(changeEnd, newSpans[newSpans.Count - 1].End);
            }

            _snapshot = snapshot;
            _regions.Clear();
            _regions.AddRange(regions);

            if (changeStart <= changeEnd)
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_snapshot, Span.FromBounds(changeStart, changeEnd))));
        }

        #endregion Private Methods

        #endregion Methods
    }
}