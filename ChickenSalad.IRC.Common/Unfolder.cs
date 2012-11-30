using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChickenSalad.IRC.Common
{
    public class Unfolder<T>
    {
        public bool Success { get; set; }
        public T NextValue { get; set; }
    }

    public class Unfolder
    {
        public static Unfolder<T> Continue<T>(T value)
        {
            return new Unfolder<T>
            {
                Success = true,
                NextValue = value,
            };
        }

        public static Unfolder<T> Stop<T>()
        {
            return new Unfolder<T>
            {
                Success = false,
                NextValue = default(T),
            };
        }

        public static Unfolder<T> From<T>(bool success, T value)
        {
            return new Unfolder<T>
            {
                Success = success,
                NextValue = success ? value : default(T),
            };
        }
    }
}
