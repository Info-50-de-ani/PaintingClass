using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;

namespace PaintingClass.PaintTools
{
    /// <summary>
    /// Clasa din care toate uneltele se vor extinde toate uneltele
    /// TODO: NU ESTE TERMINATA
    /// </summary>
    public abstract class PaintTool
    {
        //prioritatea in toolbar, cu cat e mai mic cu atat o sa apara mai sus
        public abstract int priority { get; }

        //genereaza un control care va fi afisat in toolbar
        //poate returna un <Image> sau <Label> de exemplu
        public abstract Control GetControl();
    }

}
