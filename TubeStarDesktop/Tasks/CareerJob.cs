using System;
using System.Windows.Media;

namespace TubeStar
{
    public class CareerJob : Task
    {
        private string _name;

        public override TaskType TaskType
        {
            get { return TaskType.Job; }
        }

        public override string Name
        {
            get { return _name; }
        }

        public override Color Color
        {
            get { return Colors.Crimson; }
        }

        public override int HoursToComplete
        {
            get { return 8; }
        }

        public CareerJob(string name)
        {
            _name = name;
        }
    }
}
