using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenerateHighLightContent
{
    public interface IGenerateHighLight
    {
         void GenerateHighLightCode();

         bool ShowLineNumber { get; set; }
    }
}
