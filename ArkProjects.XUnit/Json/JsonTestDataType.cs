namespace ArkProjects.XUnit.Json
{
    public enum JsonTestDataType : byte
    {
        /// <summary>
        /// Auto detection mode
        /// </summary>
        Auto,

        /// <summary>
        /// Force test cases array
        /// </summary>
        SingleParam,

        /// <summary>
        /// Force dictionary (paramName &lt;=&gt; data) or array of positioned parameters
        /// </summary>
        MultiParams
    }
}