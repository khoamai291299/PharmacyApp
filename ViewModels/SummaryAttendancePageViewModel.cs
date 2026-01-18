using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PharmacyApp.ViewModels
{
    public class SummaryAttendancePageViewModel : BaseViewModel
    {
        private readonly SummaryAttendanceRepository _repo;

        public SummaryAttendancePageViewModel()
        {
            _repo = new SummaryAttendanceRepository();

            SummaryList = new ObservableCollection<SummaryAttendanceModel>();

            LoadCommand = new RelayCommand(_ => LoadData());
            AddCommand = new RelayCommand(_ => Add(), _ => CanAdd());
            UpdateCommand = new RelayCommand(_ => Update(), _ => SelectedItem != null);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedItem != null);
            ClearCommand = new RelayCommand(_ => ClearForm());

            LoadData();
        }

        // ───────────────────────────────────────────────────────────────
        // PROPERTIES
        // ───────────────────────────────────────────────────────────────

        private ObservableCollection<SummaryAttendanceModel> _summaryList;
        public ObservableCollection<SummaryAttendanceModel> SummaryList
        {
            get => _summaryList;
            set { _summaryList = value; OnPropertyChanged(); }
        }

        private SummaryAttendanceModel _selectedItem;
        public SummaryAttendanceModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();

                if (value != null)
                {
                    Rid = value.Rid;
                    Eid = value.Eid;
                    NumOfWorkDay = value.NumOfworkDay;
                    NumOfDayOff = value.NumOfdayOff;
                    NetSalary = value.NetSalary;
                }
            }
        }

        // FORM INPUTS
        public int Rid { get; set; }
        public string Eid { get; set; }
        public int NumOfWorkDay { get; set; }
        public int NumOfDayOff { get; set; }
        public int NetSalary { get; set; }

        // ───────────────────────────────────────────────────────────────
        // COMMANDS
        // ───────────────────────────────────────────────────────────────

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

        // ───────────────────────────────────────────────────────────────
        // CRUD FUNCTIONS
        // ───────────────────────────────────────────────────────────────

        private void LoadData()
        {
            SummaryList.Clear();
            foreach (var item in _repo.GetAll())
            {
                SummaryList.Add(item);
            }
        }

        private bool CanAdd()
        {
            return !string.IsNullOrEmpty(Eid) && Rid > 0;
        }

        private void Add()
        {
            var model = new SummaryAttendanceModel
            {
                Rid = Rid,
                Eid = Eid,
                NumOfworkDay = NumOfWorkDay,
                NumOfdayOff = NumOfDayOff,
                NetSalary = NetSalary
            };

            if (_repo.Insert(model))
            {
                MessageBox.Show("Thêm mới thành công!");
                LoadData();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Thêm thất bại!");
            }
        }

        private void Update()
        {
            if (SelectedItem == null) return;

            var model = new SummaryAttendanceModel
            {
                Rid = Rid,
                Eid = Eid,
                NumOfworkDay = NumOfWorkDay,
                NumOfdayOff = NumOfDayOff,
                NetSalary = NetSalary
            };

            if (_repo.Update(model))
            {
                MessageBox.Show("Cập nhật thành công!");
                LoadData();
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại!");
            }
        }

        private void Delete()
        {
            if (SelectedItem == null) return;

            if (_repo.Delete(SelectedItem.Rid, SelectedItem.Eid))
            {
                MessageBox.Show("Xóa thành công!");
                LoadData();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Xóa thất bại!");
            }
        }

        private void ClearForm()
        {
            Rid = 0;
            Eid = "";
            NumOfWorkDay = 0;
            NumOfDayOff = 0;
            NetSalary = 0;
            OnPropertyChanged(nameof(Rid));
            OnPropertyChanged(nameof(Eid));
            OnPropertyChanged(nameof(NumOfWorkDay));
            OnPropertyChanged(nameof(NumOfDayOff));
            OnPropertyChanged(nameof(NetSalary));
        }
    }
}
