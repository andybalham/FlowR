using System;
using System.Collections.Generic;
using System.Text;

namespace TwentyQuestions
{
    public interface IConsoleService
    {
        void WriteLine(string value);

        string ReadLine();
    }
}
