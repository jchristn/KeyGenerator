using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FindClosestString;

namespace Generator
{
    public class KeyValidator
    {
        #region Public-Members

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        public KeyValidator()
        {

        }

        #endregion

        #region Public-Methods

        public bool Validate(
            string key,
            List<string> keys,
            int minimumEditDistance)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (keys == null || keys.Count < 1) return true;

            (string, int) closest = ClosestString.UsingLevenshtein(key, keys);
            if (!String.IsNullOrEmpty(closest.Item1))
            {
                if (closest.Item2 > minimumEditDistance) return true;
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Private-Methods

        #endregion
    }
}
