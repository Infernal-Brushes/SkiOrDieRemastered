using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Exceptions
{
    public class LocalizationException : Exception
    {
        public LocalizationException()
        {

        }
        public LocalizationException(string message) : base(string.Format(message))
        {
            
        }
    }
}
