namespace Sortiously
{
    public class SortProgress
    {
        public bool Reading { get; set; }

        public bool Writing { get; set; }

        public int Counter { get; set; }

        internal void InitReading()
        {
            this.ResetCounter();
            this.Reading = true;
            this.Writing = false;
        }

        internal void InitWriting()
        {
            this.ResetCounter();
            this.Reading = false;
            this.Writing = true;
        }

        private void ResetCounter()
        {
            this.Counter = 0;
        }
    }
}
