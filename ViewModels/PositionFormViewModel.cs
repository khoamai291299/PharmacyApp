using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System.Collections.ObjectModel;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class PositionFormViewModel : BaseViewModel
    {
        private readonly PositionRepository _repo;
        private readonly PositionModel _original;
        private readonly ObservableCollection<PositionModel> _items;

        public PositionModel Model { get; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public PositionFormViewModel(
            PositionModel model,
            ObservableCollection<PositionModel> items,
            PositionRepository repo,
            PositionModel original = null)
        {
            Model = model;
            _items = items;
            _repo = repo;
            _original = original;

            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(w => (w as Window)?.Close());
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Model.Id)
                || string.IsNullOrWhiteSpace(Model.Name))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
                return;
            }

            if (_original == null)
                _repo.Insert(Model);
            else
                _repo.Update(Model);

            (Application.Current.Windows[^1] as Window)?.Close();
        }
    }
}
