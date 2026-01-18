using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using PharmacyApp.Models;
using PharmacyApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PharmacyApp.Utils
{
    public static class InvoicePdfService
    {
        private static readonly Color PrimaryColor = new DeviceRgb(33, 150, 243); // xanh dương
        private static readonly Color LightBlue = new DeviceRgb(227, 242, 253);
        private static readonly Color BorderColor = new DeviceRgb(200, 200, 200);

        public static string CreateInvoice(
            BillModel bill,
            CustomerModel customer,
            EmployeeModel employee,
            List<SaleItemViewModel> items,
            PromotionModel promotion = null) // thêm tham số promotion
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Invoices"
            );
            Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, $"Invoice_{bill.Id}.pdf");

            PdfFont font = PdfFontFactory.CreateFont(
                @"C:\Windows\Fonts\arial.ttf",
                PdfEncodings.IDENTITY_H,
                PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            PdfFont fontBold = PdfFontFactory.CreateFont(
                @"C:\Windows\Fonts\arialbd.ttf",
                PdfEncodings.IDENTITY_H,
                PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            using (var writer = new PdfWriter(path))
            using (var pdf = new PdfDocument(writer))
            using (var doc = new Document(pdf))
            {
                doc.SetFont(font);
                doc.SetMargins(30, 30, 30, 30);

                // ===== HEADER =====
                Table header = new Table(1).UseAllAvailableWidth();
                header.AddCell(new Cell()
                    .SetBackgroundColor(PrimaryColor)
                    .SetPadding(15)
                    .SetBorder(Border.NO_BORDER)
                    .Add(new Paragraph("HÓA ĐƠN BÁN THUỐC")
                        .SetFont(fontBold)
                        .SetFontSize(20)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetTextAlignment(TextAlignment.CENTER))
                );
                doc.Add(header);
                doc.Add(new Paragraph("\n"));

                // ===== THÔNG TIN KHÁCH HÀNG =====
                Table info = new Table(new float[] { 1, 2 }).UseAllAvailableWidth();
                info.SetBorder(new SolidBorder(BorderColor, 1));

                AddInfo(info, "Mã hóa đơn:", bill.Id, fontBold);
                AddInfo(info, "Khách hàng:", customer?.Name ?? "Khách lẻ", fontBold);
                AddInfo(info, "Nhân viên:", employee.Name, fontBold);
                AddInfo(info, "Ngày:", bill.DateOfcreate?.ToString("dd/MM/yyyy HH:mm") ?? "—", fontBold);

                if (promotion != null)
                {
                    AddInfo(info, "Mã khuyến mãi:", $"{promotion.Id} - Giảm {promotion.DiscountPercent}%", fontBold);
                }

                doc.Add(info);
                doc.Add(new Paragraph("\n"));

                // ===== DANH SÁCH SẢN PHẨM =====
                Table table = new Table(new float[] { 4, 1, 2, 2 })
                    .UseAllAvailableWidth()
                    .SetBorder(new SolidBorder(BorderColor, 1));

                AddHeader(table, "Thuốc", fontBold);
                AddHeader(table, "SL", fontBold);
                AddHeader(table, "Đơn giá", fontBold);
                AddHeader(table, "Thành tiền", fontBold);

                int totalBeforeDiscount = 0;

                foreach (var i in items)
                {
                    table.AddCell(CreateCell(i.MedicineName));
                    table.AddCell(CreateCell(i.Quantity.ToString(), TextAlignment.CENTER));
                    table.AddCell(CreateCell(i.UnitPrice.ToString("N0"), TextAlignment.RIGHT));
                    table.AddCell(CreateCell(i.TotalAmount.ToString("N0"), TextAlignment.RIGHT));

                    totalBeforeDiscount += i.TotalAmount;
                }

                doc.Add(table);
                doc.Add(new Paragraph("\n"));

                // ===== TỔNG TIỀN =====
                int totalAfterDiscount = totalBeforeDiscount;
                if (promotion != null)
                {
                    totalAfterDiscount = totalBeforeDiscount - (int)(totalBeforeDiscount * promotion.DiscountPercent / 100.0);
                }

                Table totalTable = new Table(2).UseAllAvailableWidth();

                totalTable.AddCell(new Cell()
                    .SetBorder(Border.NO_BORDER)
                    .Add(new Paragraph("TỔNG TIỀN TRƯỚC GIẢM")
                        .SetFont(fontBold)
                        .SetFontSize(12)));

                totalTable.AddCell(new Cell()
                    .SetBorder(Border.NO_BORDER)
                    .Add(new Paragraph($"{totalBeforeDiscount:N0} VNĐ")
                        .SetFont(fontBold)
                        .SetFontSize(12))
                    .SetTextAlignment(TextAlignment.RIGHT));

                if (promotion != null)
                {
                    totalTable.AddCell(new Cell()
                        .SetBorder(Border.NO_BORDER)
                        .Add(new Paragraph($"GIẢM {promotion.DiscountPercent}% ({promotion.Id})")
                            .SetFont(fontBold)
                            .SetFontSize(12))
                        .SetFontColor(ColorConstants.RED));

                    totalTable.AddCell(new Cell()
                        .SetBorder(Border.NO_BORDER)
                        .Add(new Paragraph($"{totalBeforeDiscount - totalAfterDiscount:N0} VNĐ")
                            .SetFont(fontBold)
                            .SetFontSize(12))
                        .SetFontColor(ColorConstants.RED)
                        .SetTextAlignment(TextAlignment.RIGHT));
                }

                totalTable.AddCell(new Cell()
                    .SetBorder(Border.NO_BORDER)
                    .Add(new Paragraph("TỔNG THANH TOÁN")
                        .SetFont(fontBold)
                        .SetFontSize(14)));

                totalTable.AddCell(new Cell()
                    .SetBorder(Border.NO_BORDER)
                    .Add(new Paragraph($"{totalAfterDiscount:N0} VNĐ")
                        .SetFont(fontBold)
                        .SetFontSize(16)
                        .SetFontColor(ColorConstants.RED))
                    .SetTextAlignment(TextAlignment.RIGHT));

                doc.Add(totalTable);

                // ===== FOOTER =====
                doc.Add(new LineSeparator(new SolidLine()));
                doc.Add(new Paragraph("Xin cảm ơn quý khách đã mua hàng!")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(11)
                    .SetFontColor(ColorConstants.GRAY));
            }

            return path;
        }

        // ===== HÀM PHỤ =====
        private static void AddHeader(Table table, string text, PdfFont boldFont)
        {
            table.AddHeaderCell(new Cell()
                .SetBackgroundColor(LightBlue)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFont(boldFont)
                .SetFontSize(11)
                .Add(new Paragraph(text)));
        }

        private static Cell CreateCell(string text, TextAlignment align = TextAlignment.LEFT)
        {
            return new Cell()
                .SetPadding(6)
                .SetTextAlignment(align)
                .Add(new Paragraph(text));
        }

        private static void AddInfo(Table table, string label, string value, PdfFont boldFont)
        {
            table.AddCell(new Cell().Add(new Paragraph(label).SetFont(boldFont)));
            table.AddCell(new Cell().Add(new Paragraph(value)));
        }

        public static void OpenPdf(string path)
        {
            if (!System.IO.File.Exists(path)) return;

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
    }
}
