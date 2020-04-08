using System;
using System.Collections.Generic;
using System.Text;

namespace FlexProTransactionLogParser
{
    internal interface IRecord
    {
        string Column24 { get; set; }
        string Column13 { get; set; }
        string Raw { get; set; }
    }
}
