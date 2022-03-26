using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SourceGeneratorConsole
{
    public partial class CarModel : INotifyPropertyChanged
    {
        private double SpeedKmPerHourBackingField;
        private int NumberOfDoorsBackingField;
        private string ModelBackingField = "";

        public void SpeedUp() => SpeedKmPerHour *= 1.1;
    }
}
