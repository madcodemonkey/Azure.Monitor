using System;

namespace LogAnalyticsDirect
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        // public Age Age { get; set; }
        public Address Address { get; set; }
        public DateTime TimeCreated { get; set; } 
    }

    public class Age
    {
        public Age(int value)
        {
            Value = value;
            IsReallyOld = value > 80;
        }

        public int Value { get; set; }
        public bool IsReallyOld { get; set; }
    }
}