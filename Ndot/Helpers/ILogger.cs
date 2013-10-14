using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ndot.Helpers
{
    public interface ILogger
    {
        void Write(string msg);
    }

    public class Logger : ILogger
    {
        public void Write(string msg)
        {
            throw new NotImplementedException();
        }
    }
}