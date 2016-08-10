namespace Samples.Complete
{
    class StringTools
    {
        public string AlignLeft(object input, int width)
        {
            if (input == null)
            {
                return null;            
            }

            return string.Format("{0,-"+ width +"}", input.ToString());
        }

        public string AlignRight(object input, int width)
        {
            if (input == null)
            {
                return null;
            }

            return string.Format("{0," + width + "}", input.ToString());
        }
    }
}
