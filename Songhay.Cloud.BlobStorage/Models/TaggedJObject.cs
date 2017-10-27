using Newtonsoft.Json.Linq;

namespace Songhay.Cloud.BlobStorage.Models
{
    /// <summary>
    /// Extends <see cref="JObject"/> with a tag
    /// for repository persistence, etc.
    /// </summary>
    public class TaggedJObject : JObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaggedJObject"/> class.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="tagPropertyName">Name of the tag property.</param>
        public TaggedJObject(string json, string tagPropertyName = "Tag") : base(JObject.Parse(json))
        {
            this.Tag = (string)this[tagPropertyName];
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public string Tag { get; set; }
    }
}
