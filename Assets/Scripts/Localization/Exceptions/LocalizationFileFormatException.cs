using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Exceptions
{
    public class LocalizationFileFormatException : Exception
    {
        public LocalizationFileFormatException()
        {

        }

        public LocalizationFileFormatException(string message)
            :base (string.Format(message))
        { 

        }
    }
}
