namespace Sortiously
{
    public class MergePurgeParam
    {
        public string[] MasterFields { get; set; }
        public string[] DetailFields { get; set; }
        public bool KeyFound { get; set; }
        public MergePurgeAction DataAction { get; set; }
    }

}
