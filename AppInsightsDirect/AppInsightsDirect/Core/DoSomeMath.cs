﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AppInsightsDirect.Core
{
    public interface IDoSomeMath
    {
        decimal DivideTenByInput(int input);
    }

    public class DoSomeMath : IDoSomeMath
    {
        public decimal DivideTenByInput(int input)
        {
            return 10.0m / input;
        }
    }
}
