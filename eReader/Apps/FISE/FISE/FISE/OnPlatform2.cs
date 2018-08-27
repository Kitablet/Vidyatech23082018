using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitablet
{
    public sealed class OnPlatform2<T>
    {
        public OnPlatform2()
        {
            Android = default(T);
            iOS = default(T);
            WinPhone = default(T);
            Windows = default(T);
            Win8X = default(T);
            Other = default(T);
        }
        public T Android { get; set; }
        public T iOS { get; set; }
        public T WinPhone { get; set; }        
        public T Windows { get; set; }        
        public T Win8X { get; set; }
        public T Other { get; set; }
        

        public static implicit operator T(Kitablet.OnPlatform2<T> onPlatform)
        {
            switch (Constant.CurrentPlateform)
            {
                case "Android":
                    return onPlatform.Android;

                case "iOS":
                    return onPlatform.iOS;

                case "UWP":
                    return onPlatform.Windows;
                case "Win8.1":
                    return onPlatform.Win8X;

                default:
                    return onPlatform.Other;
            }
        }
    }
}
