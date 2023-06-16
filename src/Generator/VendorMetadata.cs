using System;
using System.Text.Json.Serialization;
using Watson.ORM.Core;

namespace Generator
{
    [Table("vendors")]
    public class VendorMetadata
    {
        #region Public-Members

        [JsonIgnore]
        [Column("id", true, DataTypes.Int, false)]
        public int Id { get; set; } = 0;

        [Column("guid", false, DataTypes.Nvarchar, 64, false)]
        public string GUID { get; set; } = Guid.NewGuid().ToString();

        [Column("key", false, DataTypes.Nvarchar, 32, false)]
        public string Key { get; set; } = string.Empty;

        [Column("name", false, DataTypes.Nvarchar, 64, false)]
        public string Name { get; set; } = string.Empty;

        [Column("contactinfo", false, DataTypes.Nvarchar, 256, false)]
        public string ContactInformation { get; set; } = string.Empty;

        [Column("createdutc", false, DataTypes.DateTime, false)]
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        [Column("lastmodifiedutc", false, DataTypes.DateTime, 32, false)]
        public DateTime LastModifiedUtc { get; set; } = DateTime.UtcNow;

        public int? EditDistance { get; set; } = null;

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        public VendorMetadata()
        {

        }

        public VendorMetadata(string key, string name, string contactInformation)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (String.IsNullOrEmpty(contactInformation)) throw new ArgumentNullException(nameof(contactInformation));

            Key = key;
            Name = name;
            ContactInformation = contactInformation;
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}