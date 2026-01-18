using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PharmacyApp.ViewModels
{
    public class MedicineFormViewModel : BaseViewModel
    {
        private readonly MedicineRepository _repo = new MedicineRepository();
        private readonly TypeMedicineRepository _typeRepo = new TypeMedicineRepository();
        private readonly ManufacturerRepository _manufacturerRepo = new ManufacturerRepository();

        public event Action<MedicineModel> OnSaved;

        public MedicineModel Medicine { get; set; }

        public ObservableCollection<TypeMedicineModel> Types { get; set; } = new ObservableCollection<TypeMedicineModel>();
        public ObservableCollection<ManufacturerModel> Manufacturers { get; set; } = new ObservableCollection<ManufacturerModel>();

        private TypeMedicineModel _selectedType;
        public TypeMedicineModel SelectedType
        {
            get => _selectedType;
            set
            {
                if (SetProperty(ref _selectedType, value))
                {
                    Medicine.Tid = value?.Id;
                }
            }
        }

        private ManufacturerModel _selectedManufacturer;
        public ManufacturerModel SelectedManufacturer
        {
            get => _selectedManufacturer;
            set
            {
                if (SetProperty(ref _selectedManufacturer, value))
                {
                    Medicine.Mid = value?.Id;
                }
            }
        }

        // ===== Commands =====
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand UploadImageCommand { get; }

        // ===== Constructor =====
        public MedicineFormViewModel(MedicineModel medicine = null, int? defaultImportPrice = null)
        {
            // ====== EDIT MODE ======
            if (medicine != null && !string.IsNullOrWhiteSpace(medicine.Id))
            {
                Medicine = _repo.GetById(medicine.Id) ?? throw new Exception("Không tìm thấy thuốc cần sửa.");

                if (!Medicine.InputTime.HasValue)
                    Medicine.InputTime = DateTime.Today;
            }
            // ====== ADD MODE ======
            else
            {
                Medicine = new MedicineModel
                {
                    Id = _repo.GenerateNewId(),
                    ImportPrice = defaultImportPrice ?? 0,
                    SellingPrice = 0,
                    ProductionDate = DateTime.Today,
                    ExpirationDate = DateTime.Today.AddYears(1),
                    InputTime = DateTime.Today,
                    ImagePath = "pack://application:,,,/Resources/default_image.png" // ảnh mặc định
                };
            }

            LoadTypes();
            LoadManufacturers();

            SelectedType = Types.FirstOrDefault(t => t.Id == Medicine.Tid);
            SelectedManufacturer = Manufacturers.FirstOrDefault(m => m.Id == Medicine.Mid);

            // ===== Commands =====
            SaveCommand = new RelayCommand(o => Save(o as Window));
            CancelCommand = new RelayCommand(o => Cancel(o as Window));
            UploadImageCommand = new RelayCommand(_ => UploadImage());
        }

        // ===== METHODS =====
        private void Save(Window window)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(Medicine.Name))
                {
                    MessageBox.Show("Tên thuốc không được để trống.");
                    return;
                }

                if (Medicine.ImportPrice < 0 || Medicine.SellingPrice < 0)
                {
                    MessageBox.Show("Giá nhập và giá bán phải >= 0.");
                    return;
                }

                Medicine.Tid = SelectedType?.Id;
                Medicine.Mid = SelectedManufacturer?.Id;

                bool exists = _repo.GetAll().Any(m => m.Id == Medicine.Id);
                if (exists)
                    _repo.Update(Medicine);
                else
                {
                    _repo.Add(Medicine);
                    OnSaved?.Invoke(Medicine);
                }

                MessageBox.Show("Lưu thành công.");
                window?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lưu thất bại: " + ex.Message);
            }
        }

        private void Cancel(Window window)
        {
            window?.Close();
        }

        private void LoadTypes()
        {
            Types.Clear();
            try
            {
                foreach (var t in _typeRepo.GetAll())
                    Types.Add(t);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lấy danh sách loại thuốc thất bại: " + ex.Message);
            }
        }

        private void LoadManufacturers()
        {
            Manufacturers.Clear();
            try
            {
                foreach (var m in _manufacturerRepo.GetAll())
                    Manufacturers.Add(m);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lấy danh sách hãng sản xuất thất bại: " + ex.Message);
            }
        }

        // ===== Upload Image =====
        private void UploadImage()
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Chọn ảnh thuốc"
            };

            if (dlg.ShowDialog() == true)
            {
                string selectedPath = dlg.FileName;

                // Bạn có thể lưu copy vào thư mục riêng trong project, ví dụ: "Images/Medicine"
                string destDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Medicine");
                Directory.CreateDirectory(destDir);

                string fileName = Path.GetFileName(selectedPath);
                string destPath = Path.Combine(destDir, fileName);

                // Copy file (overwrite nếu trùng)
                File.Copy(selectedPath, destPath, true);

                // Cập nhật đường dẫn vào model
                Medicine.ImagePath = destPath;
                OnPropertyChanged(nameof(Medicine));
            }
        }
    }
}
