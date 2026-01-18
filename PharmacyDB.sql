CREATE DATABASE PharmacyDB;
GO

USE PharmacyDB;
GO

-- Table 1: Bộ phận
CREATE TABLE Department(
	id NVARCHAR(10) NOT NULL PRIMARY KEY,
	name NVARCHAR(30) NOT NULL
);

-- Table 2: Chức vụ
CREATE TABLE Position(
	id NVARCHAR(10) NOT NULL PRIMARY KEY,
	name NVARCHAR(30) NOT NULL
);

-- Table 3: Nhân viên
CREATE TABLE Employee(
	id NVARCHAR(10) NOT NULL PRIMARY KEY,
	name NVARCHAR(30) NOT NULL,
	phone NVARCHAR(15) NULL,
	address NVARCHAR(50) NULL,
	startday DATE NULL,
	birthday DATE NULL,
	sex BIT NULL,
	salary INT NULL,
	did NVARCHAR(10) NOT NULL,
	pid NVARCHAR(10) NOT NULL,
	CONSTRAINT FK_Emp_Pos FOREIGN KEY(pid)
		REFERENCES Position(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	CONSTRAINT FK_Emp_Dep FOREIGN KEY(did)
		REFERENCES Department(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	CONSTRAINT check_salary CHECK (salary>0),
	CONSTRAINT check_startday CHECK (startday < GETDATE())
);

-- Table 4: Ký công
CREATE TABLE RollCall(
	id INT IDENTITY(1,1) PRIMARY KEY,
	months INT NOT NULL CHECK (months BETWEEN 1 AND 12),
	years INT NOT NULL CHECK (years >= 2000),
	CONSTRAINT UQ_RollCall UNIQUE(months, years)
);

-- Table 5: Chấm công chi tiết
CREATE TABLE Detailed_Attendance(
	id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	eid NVARCHAR(10) NOT NULL,
	days DATE NOT NULL,
	checkin TIME NULL,
	checkout TIME NULL,
	status NVARCHAR(20) NULL,
	note NVARCHAR(200) NULL,
	CONSTRAINT FK_DTAT_Emp FOREIGN KEY(eid)
		REFERENCES Employee(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	CONSTRAINT UQ_Emp_days UNIQUE(eid, days) -- Thêm ràng buộc nhân viên chỉ có thể chấm công 1 lần 1 ngày
);

-- Table 6: Tổng hợp chấm công theo tháng
CREATE TABLE Summary_Attendance(
	rid INT NOT NULL,
	eid NVARCHAR(10) NOT NULL,
	numOfworkDay INT DEFAULT 0,
	numOfdayOff INT DEFAULT 0,
	netSalary INT DEFAULT 0,
	CONSTRAINT PR_SA PRIMARY KEY(rid, eid),
	CONSTRAINT FK_SA_RC FOREIGN KEY(rid)
		REFERENCES RollCall(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	CONSTRAINT FK_SA_Emp FOREIGN KEY(eid)
		REFERENCES Employee(id)
		ON DElETE CASCADE
		ON UPDATE CASCADE
);

-- Table 7: Phân loại thuốc
CREATE TABLE TypeMedicine(
	id NVARCHAR(10) NOT NULL PRIMARY KEY,
	name NVARCHAR(30) NOT NULL,
	description NVARCHAR(50) NULL
);

INSERT INTO TypeMedicine(id, name, description) VALUES
('TM04', N'Thuốc giảm đau', N'Giảm đau, hạ sốt'),
('TM05', N'Thuốc tim mạch', N'Hỗ trợ tim mạch và huyết áp'),
('TM06', N'Thuốc hô hấp', N'Điều trị bệnh đường hô hấp'),
('TM07', N'Thuốc tiêu hóa', N'Hỗ trợ tiêu hóa, dạ dày'),
('TM08', N'Thuốc da liễu', N'Chăm sóc và điều trị da');

Delete TypeMedicine where id = 'TM04'

-- Table 8: Nhà sản xuất
CREATE TABLE Manufacturer(
	id NVARCHAR(10) NOT NULL PRIMARY KEY,
	name NVARCHAR(30) NOT NULL,
	country NVARCHAR(20) NULL
);

INSERT INTO Manufacturer(id, name, country) VALUES
('M04', 'Pfizer', 'USA'),
('M05', 'Roche', 'Switzerland'),
('M06', 'Novartis', 'Switzerland'),
('M07', 'Sanofi', 'France'),
('M08', 'Bayer', 'Germany'),
('M09', 'GSK', 'UK'),
('M10', 'Abbott', 'USA'),
('M11', 'Takeda', 'Japan');

-- Table 9: Thuốc
CREATE TABLE Medicine(
	id NVARCHAR(10) NOT NULL PRIMARY KEY,
	name NVARCHAR(30) NOT NULL,
	content INT NULL,
	activeIngredient NVARCHAR(30) NULL,
	inputTime DATE NULL,
	productionDate DATE NULL,
	expirationDate DATE NULL,
	unit NVARCHAR(10) NULL,
	tid NVARCHAR(10) NOT NULL,
	mid NVARCHAR(10) NOT NULL,
	quantity INT DEFAULT 0,
	importPrice INT DEFAULT 0,
	sellingPrice INT DEFAULT 0,
	CONSTRAINT FK_Medi_Type FOREIGN KEY(tid)
		REFERENCES TypeMedicine(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	CONSTRAINT FK_Medi_Manu FOREIGN KEY(mid)
		REFERENCES Manufacturer(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	CONSTRAINT check_time CHECK(inputTime >= productionDate AND productionDate < expirationDate)
);
-- Bổ sung thuộc tính ảnh
ALTER TABLE Medicine
ADD ImagePath NVARCHAR(200) NULL;

INSERT INTO Medicine(id, name, content, activeIngredient, inputTime, productionDate, expirationDate, unit, tid, mid, quantity, importPrice, sellingPrice, ImagePath) VALUES
('MD01','Atorvastatin','20','Atorvastatin','2026-01-01','2025-12-01','2027-12-01','viên','TM05','M04',50,40000,45000,NULL),
('MD02','Enalapril','10','Enalapril','2026-01-01','2025-12-05','2027-12-05','viên','TM05','M05',60,35000,38000,NULL),
('MD03','Simvastatin','20','Simvastatin','2026-01-01','2025-11-20','2027-11-20','viên','TM05','M06',40,40000,42000,NULL),
('MD04','Furosemide','40','Furosemide','2026-01-01','2025-10-15','2027-10-15','viên','TM05','M07',70,18000,20000,NULL),
('MD05','Losartan','50','Losartan','2026-01-01','2025-12-10','2027-12-10','viên','TM05','M08',50,35000,38000,NULL),
('MD06','Bisoprolol','10','Bisoprolol','2026-01-01','2025-12-01','2027-12-01','viên','TM05','M09',60,38000,40000,NULL),
('MD07','Amlodipine','5','Amlodipine','2026-01-01','2025-11-25','2027-11-25','viên','TM05','M10',50,32000,35000,NULL),
('MD08','Clarithromycin','500','Clarithromycin','2026-01-01','2025-12-01','2027-12-01','viên','TM06','M11',60,38000,40000,NULL),
('MD09','Amoxicillin','250','Amoxicillin','2026-01-01','2025-11-20','2027-11-20','viên','TM06','M04',80,20000,25000,NULL),
('MD10','Cefuroxime','500','Cefuroxime','2026-01-01','2025-12-05','2027-12-05','viên','TM06','M05',50,27000,30000,NULL),
('MD11','Azithromycin','250','Azithromycin','2026-01-01','2025-12-10','2027-12-10','viên','TM06','M06',70,29000,32000,NULL),
('MD12','Levofloxacin','500','Levofloxacin','2026-01-01','2025-11-25','2027-11-25','viên','TM06','M07',40,42000,45000,NULL),
('MD13','Salbutamol','100','Salbutamol','2026-01-01','2025-12-01','2027-12-01','viên','TM06','M08',60,25000,28000,NULL),
('MD14','Montelukast','10','Montelukast','2026-01-01','2025-12-05','2027-12-05','viên','TM06','M09',50,29000,32000,NULL),
('MD15','Omeprazole','20','Omeprazole','2026-01-01','2025-11-30','2027-11-30','viên','TM07','M10',80,22000,25000,NULL),
('MD16','Lansoprazole','15','Lansoprazole','2026-01-01','2025-12-01','2027-12-01','viên','TM07','M11',70,24000,27000,NULL),
('MD17','Ranitidine','150','Ranitidine','2026-01-01','2025-12-05','2027-12-05','viên','TM07','M04',90,12000,15000,NULL),
('MD18','Esomeprazole','20','Esomeprazole','2026-01-01','2025-11-25','2027-11-25','viên','TM07','M05',40,32000,35000,NULL),
('MD19','Hydrocortisone cream','10','Hydrocortisone','2026-01-01','2025-12-01','2027-12-01','tuýp','TM08','M06',60,15000,20000,NULL),
('MD20','Clotrimazole cream','10','Clotrimazole','2026-01-01','2025-12-05','2027-12-05','tuýp','TM08','M07',50,17000,18000,NULL),
('MD21','Betamethasone cream','10','Betamethasone','2026-01-01','2025-11-20','2027-11-20','tuýp','TM08','M08',60,18000,20000,NULL),
('MD22','Miconazole cream','10','Miconazole','2026-01-01','2025-12-01','2027-12-01','tuýp','TM08','M09',50,20000,22000,NULL),
('MD23','Vitamin C','500','Vitamin C','2026-01-01','2025-12-01','2027-12-01','viên','TM05','M10',120,18000,20000,NULL),
('MD24','Vitamin D3','400','Vitamin D3','2026-01-01','2025-12-05','2027-12-05','viên','TM05','M11',100,20000,22000,NULL),
('MD25','Naproxen','250','Naproxen','2026-01-01','2025-11-25','2027-11-25','viên','TM05','M04',70,16000,18000,NULL),
('MD26','Paracetamol Extra','500','Paracetamol','2026-01-01','2025-12-01','2027-12-01','viên','TM05','M05',90,18000,20000,NULL),
('MD27','Cefixime','400','Cefixime','2026-01-01','2025-12-01','2027-12-01','viên','TM06','M06',60,30000,32000,NULL),
('MD28','Amoxicillin XR','500','Amoxicillin','2026-01-01','2025-12-05','2027-12-05','viên','TM06','M07',40,33000,35000,NULL),
('MD29','Cetirizine','10','Cetirizine','2026-01-01','2025-11-30','2027-11-30','viên','TM06','M08',70,15000,18000,NULL),
('MD30','Ranitidine Plus','150','Ranitidine','2026-01-01','2025-12-01','2027-12-01','viên','TM07','M09',90,14000,16000,NULL);


-- Table 10: Phân loại khách hàng
CREATE TABLE TypeCustomer(
	id NVARCHAR(10) NOT NULL PRIMARY KEY,
	name NVARCHAR(20) NOT NULL,
	minimumLevel INT NOT NULL,
	maximumLevel INT NOT NULL,
	CONSTRAINT check_muc CHECK(minimumLevel < maximumLevel)
);

-- Table 11: Khách hàng
CREATE TABLE Customer(
	id NVARCHAR(10) NOT NULL PRIMARY KEY,
	name NVARCHAR(30) NOT NULL,
	phone NVARCHAR(15) NULL,
	address NVARCHAR(50) NULL,
	tid NVARCHAR(10) NOT NULL,
	totalExpenditure INT DEFAULT 0,
	cumulativePoints INT DEFAULT 0,
	CONSTRAINT FK_Cus_Type FOREIGN KEY(tid)
		REFERENCES TypeCustomer(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

-- Table 12: Phiếu nhập kho
CREATE TABLE WarehouseReceipt(
	id NVARCHAR(10) NOT NULL PRIMARY KEY,
	inputDay DATE NULL,
	eid NVARCHAR(10) NOT NULL,
	totalImport INT DEFAULT 0,
	CONSTRAINT FK_WareRec_Emp FOREIGN KEY(eid)
		REFERENCES Employee(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

-- Table 13: Chi tiết nhập kho
CREATE TABLE WarehouseDetails(
	wid NVARCHAR(10) NOT NULL,
	mid NVARCHAR(10) NOT NULL,
	quantity INT NOT NULL,
	unitPrice INT NOT NULL,
	totalAmount INT NULL,
	CONSTRAINT PK_WD PRIMARY KEY(wid, mid),
	CONSTRAINT FK_WD_Medi FOREIGN KEY(mid)
		REFERENCES Medicine(id)
		ON DELETE CASCADE
		ON UPDAtE CASCADE,
	CONSTRAINT FK_WD_WR FOREIGN KEY(wid)
		REFERENCES WarehouseReceipt(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

-- Table 14: Hóa đơn
CREATE TABLE Bill(
	id NVARCHAR(10) NOT NULL PRIMARY KEY,
	dateOfcreate DATE NOT NULL,
	cid NVARCHAR(10) NOT NULL,
	eid NVARCHAR(10) NOT NULL,
	totalAmount INT DEFAULT 0,
	CONSTRAINT FK_Bill_Cus FOREIGN KEY(cid)
		REFERENCES Customer(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	CONSTRAINT FK_Bill_Emp FOREIGN KEY(eid)
		REFERENCES Employee(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);
-- Cập nhập lại bill khi dùng mã khuyến mãi (nếu có)
ALTER TABLE Bill
ADD PromotionId NVARCHAR(10) NULL,
CONSTRAINT FK_Bill_Promotion FOREIGN KEY(PromotionId)
    REFERENCES Promotion(Id)
    ON DELETE SET NULL
    ON UPDATE CASCADE;
-- Table 15: Chi tiết hóa đơn
CREATE TABLE BillDetails(
	bid NVARCHAR(10) NOT NULL,
	mid NVARCHAR(10) NOT NULL,
	quantity INT DEFAULT 0,
	unitPrice INT DEFAULT 0,
	totalAmount INT DEFAULT 0,
	CONSTRAINT PK_BD PRIMARY KEY(bid, mid),
	CONSTRAINT FK_BD_Medi FOREIGN KEY(mid)
		REFERENCES Medicine(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	CONSTRAINT FK_BD_B FOREIGN KEY(bid)
		REFERENCES Bill(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

-- Table 16: User
CREATE TABLE USERS(
	id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	username NVARCHAR(10) Not Null Unique,
	password NVARCHAR(30) NOT NULL,
	email NVARCHAR(30) NOT NULL,
	phone NVARCHAR(10) NOT NULL,
	eid NVARCHAR(10) NOT NULL FOREIGN KEY
		REFERENCES Employee(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	role Nvarchar(20) NOT NULL,
	status NVARCHAR(20) Not null
);

-- Tạo ràng buộc quan hệ 1-1 cho user và nhân viên
ALTER TABLE USERS
ADD CONSTRAINT UQ_USERS_EID UNIQUE (eid);

-- Thêm ràng buộc kiểm tra duy nhất cho mail
ALTER TABLE USERS
ADD CONSTRAINT UQ_USERS_EMAIL UNIQUE (email);

-- Table 17: Mã khuyến mãi
CREATE TABLE Promotion
(
    Id NVARCHAR(10) PRIMARY KEY,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    RequiredPoints INT NOT NULL,
    Quantity INT NOT NULL,
    DiscountPercent INT NOT NULL
);


INSERT INTO Department(id, name) VALUES
('D01', N'Quản lý'),
('D02', N'Bán hàng'),
('D03', N'Kho'),
('D04', N'Kế toán');

INSERT INTO Position(id, name) VALUES
('P01', N'Quản lý'),
('P02', N'Nhân viên bán hàng'),
('P03', N'Thủ kho'),
('P04', N'Kế toán');

INSERT INTO Employee(id, name, phone, address, startday, birthday, sex, salary, did, pid)
VALUES
('E02', N'Trần Thị Bình', '0901111002', N'TP Huế', '2022-02-10', '1995-08-15', 0, 8000000, 'D02', 'P02'),
('E03', N'Lê Văn Cường', '0901111003', N'TP Huế', '2022-03-05', '1992-03-11', 1, 9000000, 'D03', 'P03'),
('E04', N'Hoàng Thị Dung', '0901111004', N'TP Huế', '2021-10-20', '1996-12-02', 0, 10000000, 'D04', 'P04'),
('E05', N'Phạm Văn Đông', '0901111005', N'TP Huế', '2023-01-15', '1991-04-18', 1, 9500000, 'D01', 'P02'),
('E06', N'Nguyễn Thị Giang', '0901111006', N'TP Huế', '2022-05-12', '1993-09-25', 0, 8700000, 'D02', 'P01'),
('E07', N'Trần Văn Hòa', '0901111007', N'TP Huế', '2021-08-30', '1989-11-12', 1, 12000000, 'D03', 'P05'),
('E08', N'Lê Thị Khánh', '0901111008', N'TP Huế', '2022-11-01', '1994-07-07', 0, 8800000, 'D04', 'P03'),
('E09', N'Hoàng Văn Long', '0901111009', N'TP Huế', '2023-03-20', '1992-02-22', 1, 9200000, 'D01', 'P06'),
('E10', N'Phạm Thị Mai', '0901111010', N'TP Huế', '2022-09-15', '1996-06-18', 0, 8600000, 'D02', 'P04');


INSERT INTO RollCall(months, years)
VALUES (1, 2025), (2, 2025), (3, 2025);

INSERT INTO Detailed_Attendance(eid, days, checkin, checkout, status, note)
VALUES
('E02', '2025-01-02', '08:00', '17:00', N'Đi làm', NULL),
('E02', '2025-01-03', '08:05', '17:10', N'Đi làm', NULL),
('E03', '2025-01-02', '08:10', '17:00', N'Đi làm', NULL),
('E04', '2025-01-02', NULL, NULL, N'Nghỉ phép', N'Lý do cá nhân');

INSERT INTO Summary_Attendance(rid, eid, numOfworkDay, numOfdayOff, netSalary)
VALUES
(1, 'E02', 26, 2, 7800000),
(1, 'E03', 27, 1, 8500000),
(1, 'E04', 25, 3, 7200000);

INSERT INTO TypeMedicine(id, name, description)
VALUES
('TM01', N'Kháng sinh', N'Dùng điều trị nhiễm khuẩn'),
('TM02', N'Giảm đau', N'Giảm đau hạ sốt'),
('TM03', N'Vitamin', N'Bổ sung vitamin');

INSERT INTO Manufacturer(id, name, country)
VALUES
('M01', N'Dược Hậu Giang', N'Việt Nam'),
('M02', N'Pymepharco', N'Việt Nam'),
('M03', N'GSK', N'Anh');

INSERT INTO Medicine(id, name, content, activeIngredient, inputTime, productionDate, expirationDate, unit, tid, mid, quantity, importPrice, sellingPrice)
VALUES
('MD01', N'Amoxicillin 500mg', 500, N'Amoxicillin', '2025-01-01', '2024-12-01', '2026-12-01', N'Viên', 'TM01', 'M01', 200, 1000, 1500),
('MD02', N'Paracetamol 500mg', 500, N'Paracetamol', '2025-01-05', '2024-11-01', '2026-11-01', N'Viên', 'TM02', 'M02', 300, 800, 1200),
('MD03', N'Vitamin C 100mg', 100, N'Ascorbic Acid', '2025-01-10', '2024-12-10', '2027-12-10', N'Viên', 'TM03', 'M03', 150, 500, 900);

INSERT INTO TypeCustomer(id, name, minimumLevel, maximumLevel)
VALUES
('TC01', N'Thường', 0, 1000000),
('TC02', N'Thân thiết', 1000000, 5000000),
('TC03', N'VIP', 5000000, 999999999);

INSERT INTO Customer(id, name, phone, address, tid, totalExpenditure, cumulativePoints)
VALUES
('C01', N'Nguyễn Khách 1', '0988112233', N'Hà Nội', 'TC01', 200000, 20),
('C02', N'Nguyễn Khách 2', '0988777666', N'Hà Nội', 'TC02', 1500000, 150),
('C03', N'Nguyễn Khách 3', '0988333444', N'HCM', 'TC03', 7000000, 700);

INSERT INTO WarehouseReceipt(id, inputDay, eid, totalImport)
VALUES
('W01', '2025-01-01', 'E03', 500000),
('W02', '2025-01-10', 'E03', 800000);

INSERT INTO WarehouseDetails(wid, mid, quantity, unitPrice, totalAmount)
VALUES
('W01', 'MD01', 100, 1000, 100000),
('W01', 'MD02', 200, 800, 160000),
('W02', 'MD03', 150, 500, 75000);

INSERT INTO Bill(id, dateOfcreate, cid, eid, totalAmount)
VALUES
('B01', '2025-01-15', 'C01', 'E02', 50000),
('B02', '2025-01-16', 'C02', 'E02', 120000),
('B03', '2025-01-20', 'C03', 'E04', 300000);

INSERT INTO BillDetails(bid, mid, quantity, unitPrice, totalAmount)
VALUES
('B01', 'MD02', 20, 1200, 24000),
('B01', 'MD03', 30, 900, 27000),

('B02', 'MD01', 30, 1500, 45000),
('B02', 'MD03', 50, 900, 45000),

('B03', 'MD01', 100, 1500, 150000),
('B03', 'MD02', 120, 1200, 144000);

INSERT INTO USERS(username, password, email, phone, eid, role, status)
VALUES
('admin', '123', 'admin@mail.com', '0911000001', 'E01', 'Admin', 'Active'),
('seller1', '123', 'seller@mail.com', '0911000002', 'E02', 'Seller', 'Active'),
('warehouse1', '123', 'warehouse@mail.com', '0911000003', 'E03', 'Warehouse', 'Active');


-- 8. View cảnh báo kho cho WPF
CREATE OR ALTER VIEW vw_StockWarning
AS
SELECT 
    m.id AS MedicineId,
    m.name AS MedicineName,
    m.quantity,
    m.expirationDate,

    -- Loại cảnh báo
    CASE
        WHEN m.quantity <= 20 AND m.expirationDate <= DATEADD(day, 30, GETDATE())
            THEN N'Hết kho & sắp hết hạn'
        WHEN m.quantity <= 20 AND m.quantity > 0
            THEN N'Sắp hết kho'
		WHEN m.quantity = 0
            THEN N'Sắp hết kho'
        WHEN m.expirationDate <= DATEADD(day, 30, GETDATE())
            THEN N'Sắp hết hạn'
    END AS WarningType
FROM Medicine m
WHERE 
    m.quantity <= 20
    OR m.expirationDate <= DATEADD(day, 30, GETDATE());
GO


CREATE OR ALTER VIEW vw_DailyRevenue
AS
SELECT
    CAST(dateOfCreate AS DATE) AS RevenueDate,
    SUM(totalAmount) AS Revenue
FROM Bill
GROUP BY CAST(dateOfCreate AS DATE);
GO


-- 12. Trigger cộng tồn kho sau khi nhập kho
CREATE OR ALTER TRIGGER trg_UpdateStockAfterImport
ON WarehouseDetails
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE m
    SET 
        m.quantity     = m.quantity + i.quantity,
        m.importPrice  = i.unitPrice,
        m.inputTime    = wr.inputDay
    FROM Medicine m
    JOIN inserted i 
        ON m.id = i.mid
    JOIN WarehouseReceipt wr
        ON wr.id = i.wid;
END;
GO

-- 13.Trigger tính tổng tiền chi tiết nhập kho --
CREATE OR ALTER TRIGGER trg_CalcWarehouseDetailAmount
ON WarehouseDetails
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE wd
    SET totalAmount = wd.quantity * wd.unitPrice
    FROM WarehouseDetails wd
    JOIN inserted i
      ON wd.wid = i.wid AND wd.mid = i.mid;
END;
GO

-- 14. Trigger cập nhật tổng tiền phiếu nhập
CREATE OR ALTER TRIGGER trg_UpdateWarehouseTotal
ON WarehouseDetails
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE w
    SET totalImport = ISNULL((
        SELECT SUM(totalAmount)
        FROM WarehouseDetails
        WHERE wid = w.id
    ), 0)
    FROM WarehouseReceipt w
    WHERE w.id IN (
        SELECT wid FROM inserted
        UNION
        SELECT wid FROM deleted
    );
END;
GO


-- 1. FUNCTION DOANH THU THEO KHOẢNG NGÀY
CREATE OR ALTER FUNCTION fn_TotalRevenue
(
    @FromDate DATE,
    @ToDate   DATE
)
RETURNS INT
AS
BEGIN
    DECLARE @Total INT;

    SELECT @Total = ISNULL(SUM(totalAmount),0)
    FROM Bill
    WHERE dateOfcreate >= @FromDate
      AND dateOfcreate < DATEADD(DAY,1,@ToDate);

    RETURN @Total;
END;
GO

DROP FUNCTION IF EXISTS fn_TotalImport;
-- 2. FUNCTION TỔNG NHẬP KHO
CREATE OR ALTER FUNCTION fn_TotalImport
(
    @FromDate DATE,
    @ToDate   DATE
)
RETURNS INT
AS
BEGIN
    DECLARE @Total INT;

    SELECT @Total = ISNULL(SUM(totalImport),0)
    FROM WarehouseReceipt
    WHERE inputDay >= @FromDate
      AND inputDay < DATEADD(DAY,1,@ToDate);

    RETURN @Total;
END;
GO

-- Tổng nhập ngày
CREATE OR ALTER VIEW vw_DailyImport
AS
SELECT
    CAST(w.inputDay AS DATE) AS ImportDate,
    SUM(w.totalImport) AS ImportAmount
FROM WarehouseReceipt w
GROUP BY CAST(w.inputDay AS DATE);
GO




DISABLE TRIGGER trg_AfterInsert_Bill ON Bill;

DROP TRIGGER IF EXISTS trg_CheckStockBeforeSale;
DROP TRIGGER IF EXISTS trg_UpdateStockAfterSale;
DROP TRIGGER IF EXISTS trg_CalcBillDetailAmount;
DROP TRIGGER IF EXISTS trg_UpdateBillTotal;
DROP TRIGGER IF EXISTS trg_UpdateCustomerAfterBill;
DROP TRIGGER IF EXISTS trg_AfterInsert_Bill;
DROP TRIGGER IF EXISTS trg_BillDetails_AfterInsert;
GO


CREATE TRIGGER trg_BillDetails_AfterInsert
ON BillDetails
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Kiểm tra tồn kho
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN Medicine m ON i.mid = m.id
        WHERE i.quantity > m.quantity
    )
    BEGIN
        RAISERROR(N'Không đủ số lượng thuốc trong kho.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 2. Trừ tồn kho
    UPDATE m
    SET m.quantity = m.quantity - i.quantity
    FROM Medicine m
    JOIN inserted i ON m.id = i.mid;

    -- 3. Tính totalAmount BillDetails
    UPDATE bd
    SET totalAmount = bd.quantity * bd.unitPrice
    FROM BillDetails bd
    JOIN inserted i ON bd.bid = i.bid AND bd.mid = i.mid;

    -- 4. Cập nhật tổng tiền Bill
    UPDATE b
    SET totalAmount = ISNULL((SELECT SUM(totalAmount) FROM BillDetails WHERE bid = b.id),0)
    FROM Bill b
    WHERE b.id IN (SELECT DISTINCT bid FROM inserted);

    -- 5. Cập nhật Customer & nâng hạng
    UPDATE c
    SET 
        c.totalExpenditure = ISNULL(c.totalExpenditure,0) + b.totalAmount,
        c.cumulativePoints = ISNULL(c.cumulativePoints,0) +
            CASE 
                WHEN t.id = 'TC01' THEN b.totalAmount * 2 / 100
                WHEN t.id = 'TC02' THEN b.totalAmount * 5 / 100
                WHEN t.id = 'TC03' THEN b.totalAmount * 10 / 100
                ELSE 0
            END,
        c.tid = CASE 
                    WHEN ISNULL(c.totalExpenditure,0) + b.totalAmount >= t3.minimumLevel THEN 'TC03'
                    WHEN ISNULL(c.totalExpenditure,0) + b.totalAmount >= t2.minimumLevel THEN 'TC02'
                    ELSE 'TC01'
                END
    FROM Customer c
    JOIN Bill b ON c.id = b.cid
    JOIN TypeCustomer t ON c.tid = t.id
    CROSS JOIN (SELECT minimumLevel FROM TypeCustomer WHERE id='TC03') t3
    CROSS JOIN (SELECT minimumLevel FROM TypeCustomer WHERE id='TC02') t2
    WHERE b.id IN (SELECT DISTINCT bid FROM inserted);

    -- 6. Xử lý Promotion
    UPDATE p
    SET p.Quantity = p.Quantity - 1
    FROM Promotion p
    JOIN Bill b ON p.id = b.PromotionId
    WHERE b.id IN (SELECT DISTINCT bid FROM inserted) AND p.Quantity > 0;

    UPDATE c
    SET c.CumulativePoints = c.CumulativePoints - p.RequiredPoints
    FROM Customer c
    JOIN Bill b ON c.id = b.cid
    JOIN Promotion p ON p.id = b.PromotionId
    WHERE b.id IN (SELECT DISTINCT bid FROM inserted) AND p.RequiredPoints > 0;
END;
GO

Update Customer
set cumulativePoints = 0

Delete Bill