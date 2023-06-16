using System;
using System.Text.Json.Serialization;
using Watson.ORM.Core;

namespace Generator
{
    [Table("codecs")]
    public class CodecMetadata
    {
        #region Public-Members

        [JsonIgnore]
        [Column("id", true, DataTypes.Int, false)]
        public int Id { get; set; } = 0;

        [Column("guid", false, DataTypes.Nvarchar, 64, false)]
        public string GUID { get; set; } = Guid.NewGuid().ToString();

        [Column("vendorguid", false, DataTypes.Nvarchar, 64, false)]
        public string VendorGUID { get; set; } = Guid.NewGuid().ToString();

        [Column("key", false, DataTypes.Nvarchar, 32, false)]
        public string Key { get; set; } = string.Empty;

        [Column("name", false, DataTypes.Nvarchar, 64, false)]
        public string Name { get; set; } = string.Empty;

        [Column("version", false, DataTypes.Nvarchar, 32, false)]
        public string Version { get; set; } = string.Empty;

        [Column("uri", false, DataTypes.Nvarchar, 256, false)]
        public string Uri { get; set; } = string.Empty;

        [Column("createdutc", false, DataTypes.DateTime, false)]
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        [Column("lastmodifiedutc", false, DataTypes.DateTime, 32, false)]
        public DateTime LastModifiedUtc { get; set; } = DateTime.UtcNow;

        public int? EditDistance { get; set; } = null;

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        public CodecMetadata()
        {

        }

        public CodecMetadata(string key, string name, string version, string uri)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (String.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
            if (String.IsNullOrEmpty(uri)) throw new ArgumentNullException(nameof(uri));

            Key = key;
            Name = name;
            Version = version;
            Uri = uri;
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}