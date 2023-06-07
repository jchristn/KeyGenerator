using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public class RecordManager
    {
        #region Public-Members

        public List<string> VendorKeys
        {
            get
            {
                return _VendorKeys;
            }
            set
            {
                if (value == null) _VendorKeys = new List<string>();
                else _VendorKeys = value;
            }
        }

        public List<string> CodecKeys
        {
            get
            {
                return _CodecKeys;
            }
            set
            {
                if (value == null) _VendorKeys = new List<string>();
                else _VendorKeys = value;
            }
        }

        #endregion

        #region Private-Members

        private List<string> _VendorKeys = new List<string>();
        private List<string> _CodecKeys = new List<string>();

        #endregion

        #region Constructors-and-Factories

        public RecordManager()
        {

        }

        #endregion

        #region Public-Methods

        public void RetrieveVendorKeys(Func<List<string>> retrieveVendorKeys)
        {
            if (retrieveVendorKeys == null) throw new ArgumentNullException(nameof(retrieveVendorKeys));

            _VendorKeys = retrieveVendorKeys.Invoke();
        }

        public void RetrieveCodecKeys(Func<List<string>> retrieveCodecKeys)
        {
            if (retrieveCodecKeys == null) throw new ArgumentNullException(nameof(retrieveCodecKeys));

            _CodecKeys = retrieveCodecKeys.Invoke();
        }

        #endregion

        #region Private-Methods

        #endregion
    }
}
