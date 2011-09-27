using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using Helper;

namespace GenerateHighLightContent
{
    public class GenerateHighLight : IGenerateHighLight
    {
        #region -- Field and Property --

        public string Content { get; set; }

        public string CodeType { get; set; }

        public string HighLightStyle { get; set; }

        public bool ShowLineNumber { get; set; }

        public string FileName { get; set; }

        /// <summary> highlight.exe 參數 設定於 App.config 的 HighLightSection 區塊 </summary>
        private static HighLightSection _section = ConfigurationManager.GetSection("HighLightSection") as HighLightSection;
        #endregion

        #region -- Constructor --

        /// <summary> 建構子 </summary>
        /// <param name="fileName">檔案名稱</param>
        /// <param name="sourceContent">要HighLight的內容</param>
        /// <param name="codeType">語言類型</param>
        /// <param name="highLightStyle">HighLight的Style</param>
        public GenerateHighLight(string fileName, string sourceContent, string codeType, string highLightStyle)
        {
            FileName = fileName;
            Content = sourceContent;
            CodeType = codeType;
            HighLightStyle = highLightStyle;
        }

        #endregion

        #region -- IGenerageHighLight Member --

        public string GenerateHighLightCode()
        {
            string tempPath = Path.GetTempPath();
            string inputFileName = Path.Combine(tempPath, FileName);
            string outputFileName = Path.Combine(tempPath, FileName) + ".html";

            File.WriteAllText(inputFileName, Content, Encoding.UTF8);

            var workingDirectory = Path.Combine(ProcessHelper.GetAssemblyLocationDirectory, _section.FolderName);

            ProcessHelper helper = new ProcessHelper(workingDirectory, _section.ProcessName);
            helper.Arguments = GenerateArguments(inputFileName, outputFileName);
            helper.IsWaitForInputIdle = false;
            helper.WindowStyle = ProcessWindowStyle.Hidden;

            helper.ProcessStart();

            if (!File.Exists(outputFileName))
                throw new FileNotFoundException("找不到outputFile");

            File.Delete(inputFileName);
            return outputFileName;
        }

        private string GenerateArguments(string inputFileName, string outputFileName)
        {
            if (_section == null) throw new Exception("Config 內找不到 HighLightSection 區段");

            StringBuilder sb = new StringBuilder();

            ReadConfigCollection(sb, _section.GeneralArguments);
            ReadConfigCollection(sb, _section.OutputArguments);

            if (ShowLineNumber)
                sb.Append(" " + _section.OutputArguments["LineNumbers"].Key);

            string arguments = sb.ToString().TemplateSubstitute(new
            {
                inputFileName = String.Format("\"{0}\"",inputFileName), 
                outputFileName = String.Format("\"{0}\"",outputFileName),
                codeType = CodeType,
                highLightStyle = HighLightStyle
            });

            return arguments;
        }

        private void ReadConfigCollection(StringBuilder sb, ConfigurationElementCollection collection)
        {
            foreach (Argument item in collection)
            {
                if (item.Option)
                    continue;

                sb.Append(item.Key);
                if (!String.IsNullOrEmpty(item.Value))
                    sb.Append(" " + item.Value);
                sb.Append(" ");
            }
        }

        #endregion
    }
}
