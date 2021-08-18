namespace Models
{
    class ValInfo
    {
        public string IdCode { get; set; }
        public string Name { get; set; }
        public string EngName { get; set; }
        public int Nominal { get; set; }
        public string ParentCode { get; set; }
        public int ISONumCode { get; set; }
        public string ISOCharCode { get; set; }

        public ValInfo()
        {
        }
    }
}
