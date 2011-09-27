using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenerateHighLightContent
{
    public interface IGenerateHighLight
    {
        /// <summary> 產生HighLight Code </summary>
        /// <returns> 產出的檔案路徑 </returns>
         string GenerateHighLightCode();

        /// <summary> 是否顯示行號 </summary>
         bool ShowLineNumber { get; set; }
    }
}
