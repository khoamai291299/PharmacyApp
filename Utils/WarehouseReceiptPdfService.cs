using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
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
    public static class WarehouseReceiptPdfService
    {
        private static readonly Color Primary = new DeviceRgb(25, 118, 210);
        private static readonly Color Light = new DeviceRgb(227, 242, 253);

        public static string CreateReceipt(
            WarehouseReceiptModel receipt,
            EmployeeModel employee,
            List<WarehouseDetailItemViewModel> items)
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "WarehouseReceipts");

            Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, $"Warehouse_{receipt.Id}.pdf");

            PdfFont font = PdfFontFactory.CreateFont(
                @"C:\Windows\Fonts\arial.ttf",
                PdfEncodings.IDENTITY_H,
                PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            PdfFont bold = PdfFontFactory.CreateFont(
                @"C:\Windows\Fonts\arialbd.ttf",
                PdfEncodings.IDENTITY_H,
                PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            using var writer = new PdfWriter(path);
            using var pdf = new PdfDocument(writer);
            using var doc = new Document(pdf);

            doc.SetFont(font);
            doc.SetMargins(30, 30, 30, 30);

            // ===== HEADER =====
            doc.Add(new Table(1).UseAllAvailableWidth()
                .AddCell(new Cell()
                    .SetBackgroundColor(Primary)
                    .SetBorder(Border.NO_BORDER)
                    .SetPadding(15)
                    .Add(new Paragraph("PHIẾU NHẬP KHO")
                        .SetFont(bold)
                        .SetFontSize(20)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetTextAlignment(TextAlignment.CENTER))));

            doc.Add(new Paragraph("\n"));

            // ===== INFO =====
            Table info = new Table(new float[] { 1, 2 }).UseAllAvailableWidth();
            info.AddCell(CellBold("Mã phiếu:", bold));
            info.AddCell(new Cell().Add(new Paragraph(receipt.Id)));

            info.AddCell(CellBold("Nhân viên:", bold));
            info.AddCell(new Cell().Add(new Paragraph(employee.Name)));

            info.AddCell(CellBold("Ngày nhập:", bold));
            info.AddCell(new Cell().Add(new Paragraph(
                receipt.InputDay?.ToString("dd/MM/yyyy HH:mm"))));

            doc.Add(info);
            doc.Add(new Paragraph("\n"));

            // ===== TABLE =====
            Table table = new Table(new float[] { 4, 1, 2, 2 })
                .UseAllAvailableWidth();

            AddHeader(table, "Thuốc", bold);
            AddHeader(table, "SL", bold);
            AddHeader(table, "Đơn giá", bold);
            AddHeader(table, "Thành tiền", bold);

            foreach (var i in items)
            {
                table.AddCell(Cell(i.MedicineName));
                table.AddCell(Cell(i.Quantity.ToString(), TextAlignment.CENTER));
                table.AddCell(Cell(i.UnitPrice.ToString("N0"), TextAlignment.RIGHT));
                table.AddCell(Cell(i.TotalAmount.ToString("N0"), TextAlignment.RIGHT));
            }

            doc.Add(table);
            doc.Add(new Paragraph($"\nTỔNG TIỀN: {receipt.TotalImport:N0} VNĐ")
                .SetFont(bold)
                .SetFontSize(14)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFontColor(ColorConstants.RED));

            return path;
        }

        private static void AddHeader(Table t, string text, PdfFont f)
        {
            t.AddHeaderCell(new Cell()
                .SetBackgroundColor(Light)
                .SetFont(f)
                .SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph(text)));
        }

        private static Cell Cell(string text, TextAlignment a = TextAlignment.LEFT)
            => new Cell().SetPadding(6).SetTextAlignment(a).Add(new Paragraph(text));

        private static Cell CellBold(string text, PdfFont f)
            => new Cell().Add(new Paragraph(text).SetFont(f));

        public static void Open(string path)
        {
            if (File.Exists(path))
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
    }
}
