using Songhay.Models;
using System;
using System.Text;

namespace Songhay.Cloud.BlobStorage.Tests.Models
{
    /// <summary>
    /// Defines the conventional Blog entry.
    /// </summary>
    public class BlogEntry : ISortable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlogEntry"/> class.
        /// </summary>
        public BlogEntry()
        {
        }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the incept date.
        /// </summary>
        /// <value>
        /// The incept date.
        /// </value>
        public DateTime InceptDate { get; set; }

        /// <summary>
        /// Gets or sets the published state.
        /// </summary>
        /// <value>
        /// The published state.
        /// </value>
        public bool? IsPublished { get; set; }

        /// <summary>
        /// Gets or sets the item category.
        /// </summary>
        /// <value>
        /// The item category.
        /// </value>
        public string ItemCategory { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        /// <value>
        /// The modification date.
        /// </value>
        public DateTime ModificationDate { get; set; }
        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        /// <value>
        /// The slug.
        /// </value>
        public string Slug { get; set; }

        /// <summary>
        /// Gets or sets the sort ordinal.
        /// </summary>
        /// <value>
        /// The sort ordinal.
        /// </value>
        public byte SortOrdinal { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Converts this instance to memberwise clone.
        /// </summary>
        /// <returns></returns>
        public BlogEntry ToMemberwiseClone()
        {
            return this.MemberwiseClone() as BlogEntry;
        }

        /// <summary>
        /// Converts this instance into a string.
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (string.IsNullOrEmpty(this.Title))
            {
                sb.Append("Title: [!MISSING]");
            }
            else
            {
                sb.Append($"Title: {this.Title}");
            }

            sb.AppendFormat("Is Published?: {0}", (this.IsPublished.GetValueOrDefault()) ? "yes" : this.IsPublished.HasValue ? "no" : "unknown");
            sb.Append($", Sort Ordinal: {this.SortOrdinal}");
            sb.Append($", Incept Date: {this.InceptDate}");
            sb.Append($", Modification Date: {this.ModificationDate}");

            void append(string label, string value)
            {
                sb.Append($"{label}: {value}");
            }

            if (!string.IsNullOrEmpty(this.Author))
            {
                sb.AppendLine();
                append(nameof(this.Author), this.Author);
            }

            if (!string.IsNullOrEmpty(this.Slug))
            {
                sb.AppendLine();
                append(nameof(this.Slug), this.Slug);
            }

            if (!string.IsNullOrEmpty(this.ItemCategory))
            {
                sb.AppendLine();
                append(nameof(this.ItemCategory), this.ItemCategory);
            }

            if (!string.IsNullOrEmpty(this.Tag))
            {
                sb.AppendLine();
                append(nameof(this.Tag), this.Tag);
            }

            var limit = 255;
            if (!string.IsNullOrEmpty(this.Content))
            {
                sb.AppendLine();
                sb.AppendFormat("Content: {0}", (this.Content.Length > limit) ? $"{this.Content.Substring(0, limit - 1)}..." : this.Content);
            }

            sb.AppendLine();

            return (sb.Length > 0) ? sb.ToString() : base.ToString();
        }
    }
}
