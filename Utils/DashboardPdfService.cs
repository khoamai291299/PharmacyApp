using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace PharmacyApp.Utils
{
    public static class DashboardReportPdfService
    {
        // ===== COLOR PALETTE =====
        private static readonly Color Primary = new DeviceRgb(30, 64, 175);   // xanh dương đậm
        private static readonly Color Light = new DeviceRgb(219, 234, 254); // xanh dương nhạt
        private static readonly Color Muted = new DeviceRgb(55, 65, 81);    // xám đậm

        public static string Create(
            DateTime from,
            DateTime to,
            int totalRevenue,
            int totalImport,
            DataTable dailyRevenue,
            DataTable dailyImport   // ⭐ BỔ SUNG
        )
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "DashboardReports");

            Directory.CreateDirectory(folder);

            string path = Path.Combine(
                folder,
                $"Dashboard_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf");

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
            doc.SetMargins(32, 32, 32, 32);

            // ===== HEADER =====
            doc.Add(new Table(1).UseAllAvailableWidth()
                .AddCell(new Cell()
                    .SetBackgroundColor(Primary)
                    .SetBorder(Border.NO_BORDER)
                    .SetPadding(16)
                    .Add(new Paragraph("BÁO CÁO DOANH THU & NHẬP KHO")
                        .SetFont(bold)
                        .SetFontSize(20)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetTextAlignment(TextAlignment.CENTER))));

            doc.Add(new Paragraph("\n"));

            // ===== TIME RANGE =====
            doc.Add(new Paragraph($"Từ ngày {from:dd/MM/yyyy} đến {to:dd/MM/yyyy}")
                .SetFont(bold)
                .SetFontSize(12)
                .SetFontColor(Muted));

            doc.Add(new Paragraph("\n"));

            // ===== SUMMARY =====
            Table summary = new Table(new float[] { 3, 2 })
                .UseAllAvailableWidth();

            summary.AddCell(SumLabel("TỔNG DOANH THU", bold));
            summary.AddCell(SumValue($"{totalRevenue:N0} đ", bold));

            summary.AddCell(SumLabel("TỔNG NHẬP KHO", bold));
            summary.AddCell(SumValue($"{totalImport:N0} đ", bold));

            doc.Add(summary);
            doc.Add(new Paragraph("\n"));

            // =====================================================
            // ===== DOANH THU THEO NGÀY =====
            // =====================================================
            doc.Add(new Paragraph("Chi tiết doanh thu theo ngày")
                .SetFont(bold)
                .SetFontSize(14)
                .SetFontColor(Primary));

            Table revenueTable = new Table(new float[] { 2, 3 })
                .UseAllAvailableWidth();

            revenueTable.AddHeaderCell(Header("Ngày", bold));
            revenueTable.AddHeaderCell(Header("Doanh thu", bold));

            foreach (DataRow r in dailyRevenue.Rows)
            {
                revenueTable.AddCell(NormalCell(
                    Convert.ToDateTime(r["RevenueDate"])
                        .ToString("dd/MM/yyyy")));

                revenueTable.AddCell(NormalCell(
                    Convert.ToInt32(r["Revenue"]).ToString("N0") + " đ",
                    TextAlignment.RIGHT));
            }

            doc.Add(revenueTable);
            doc.Add(new Paragraph("\n"));

            // =====================================================
            // ===== NHẬP KHO THEO NGÀY =====
            // =====================================================
            doc.Add(new Paragraph("Chi tiết nhập kho theo ngày")
                .SetFont(bold)
                .SetFontSize(14)
                .SetFontColor(Primary));

            Table importTable = new Table(new float[] { 2, 3 })
                .UseAllAvailableWidth();

            importTable.AddHeaderCell(Header("Ngày", bold));
            importTable.AddHeaderCell(Header("Giá trị nhập kho", bold));

            foreach (DataRow r in dailyImport.Rows)
            {
                importTable.AddCell(NormalCell(
                    Convert.ToDateTime(r["ImportDate"])
                        .ToString("dd/MM/yyyy")));

                importTable.AddCell(NormalCell(
                    Convert.ToInt32(r["ImportAmount"]).ToString("N0") + " đ",
                    TextAlignment.RIGHT));
            }

            doc.Add(importTable);

            return path;
        }

        // ===== CELL HELPERS =====

        private static Cell Header(string text, PdfFont f)
            => new Cell()
                .SetBackgroundColor(Light)
                .SetBorder(new SolidBorder(Primary, 0.5f))
                .SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph(text)
                    .SetFont(f)
                    .SetFontColor(Primary));

        private static Cell NormalCell(
            string text,
            TextAlignment a = TextAlignment.LEFT)
            => new Cell()
                .SetPadding(8)
                .SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 0.5f))
                .SetTextAlignment(a)
                .Add(new Paragraph(text)
                    .SetFontColor(Muted));

        private static Cell SumLabel(string text, PdfFont f)
            => new Cell()
                .SetBorder(Border.NO_BORDER)
                .Add(new Paragraph(text)
                    .SetFont(f)
                    .SetFontSize(11)
                    .SetFontColor(Muted));

        private static Cell SumValue(string text, PdfFont f)
            => new Cell()
                .SetBorder(Border.NO_BORDER)
                .Add(new Paragraph(text)
                    .SetFont(f)
                    .SetFontSize(14)
                    .SetFontColor(Primary)
                    .SetTextAlignment(TextAlignment.RIGHT));

        public static void Open(string path)
        {
            if (File.Exists(path))
            {
                Process.Start(new ProcessStartInfo(path)
                { UseShellExecute = true });
            }
        }
    }
}
