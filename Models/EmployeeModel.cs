using System;
using System.ComponentModel;

namespace PharmacyApp.Models
{
    public class EmployeeModel : INotifyPropertyChanged
    {
        private string _id;
        private string _name;
        private string _phone;
        private string _address;
        private DateTime? _startday;
        private DateTime? _birthday;
        private bool? _gender;
        private int? _salary;
        private string _did;
        private string _pid;

        public string Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(nameof(Phone)); }
        }

        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(nameof(Address)); }
        }

        public DateTime? startday
        {
            get => _startday;
            set { _startday = value; OnPropertyChanged(nameof(startday)); }
        }

        public DateTime? birthday
        {
            get => _birthday;
            set { _birthday = value; OnPropertyChanged(nameof(birthday)); }
        }

        public bool? Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged(nameof(Gender)); }
        }

        public int? Salary
        {
            get => _salary;
            set { _salary = value; OnPropertyChanged(nameof(Salary)); }
        }

        public string Did
        {
            get => _did;
            set { _did = value; OnPropertyChanged(nameof(Did)); }
        }

        public string Pid
        {
            get => _pid;
            set { _pid = value; OnPropertyChanged(nameof(Pid)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
