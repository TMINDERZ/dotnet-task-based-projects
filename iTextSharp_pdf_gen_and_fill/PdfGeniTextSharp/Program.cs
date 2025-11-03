using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

class Program
{
    static void Main()
    {
        string outputPath = "Form.pdf";

        using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
        {
            var doc = new Document(PageSize.A4);
            var writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            var cb = writer.DirectContent;
            var bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

            // Title
            cb.BeginText();
            cb.SetFontAndSize(bf, 16);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Job Application", 200, 800, 0);
            cb.EndText();

            // Create text fields
            CreateTextField(writer, "Name", "Name", 50, 760, 300, 780);
            CreateTextField(writer, "Email", "Email", 50, 720, 300, 740);
            CreateTextField(writer, "Phone", "Phone", 50, 680, 300, 700);

            // ✅ Create checkboxes
            CreateCheckBox(writer, "AcceptTerms", "Accept Terms", 50, 640, 70, 660);
            CreateCheckBox(writer, "Subscribe", "Subscribe to Newsletter", 50, 600, 70, 620);

            doc.Close();
        }

        // Fill the PDF with data
        FillPdfForm(outputPath);
    }

    static void CreateTextField(PdfWriter writer, string fieldName, string label, float x1, float y1, float x2, float y2)
    {
        var cb = writer.DirectContent;

        // Draw label
        cb.BeginText();
        cb.SetFontAndSize(BaseFont.CreateFont(), 12);
        cb.ShowTextAligned(Element.ALIGN_LEFT, label + ":", x1, y2 - 12, 0);
        cb.EndText();

        float labelWidth = 80;
        var field = new TextField(writer, new Rectangle(x1 + labelWidth, y1, x2, y2), fieldName);
        writer.AddAnnotation(field.GetTextField());
    }

    static void CreateCheckBox(PdfWriter writer, string fieldName, string label, float x1, float y1, float x2, float y2)
    {
        var cb = writer.DirectContent;

        // Label next to checkbox
        cb.BeginText();
        cb.SetFontAndSize(BaseFont.CreateFont(), 12);
        cb.ShowTextAligned(Element.ALIGN_LEFT, label, x2 + 5, y1 + 2, 0);
        cb.EndText();

        // Create checkbox using RadioCheckField
        var checkbox = new RadioCheckField(writer, new Rectangle(x1, y1, x2, y2), fieldName, "Yes");
        checkbox.CheckType = RadioCheckField.TYPE_CHECK; // Standard checkbox
        checkbox.BorderColor = BaseColor.Black;
        checkbox.BorderWidth = 1;
        checkbox.BackgroundColor = BaseColor.White;
        checkbox.Checked = false; // initial state

        PdfFormField field = checkbox.CheckField;
        writer.AddAnnotation(field);
    }

    static void FillPdfForm(string pdfPath)
    {
        string filledPdfPath = "FilledForm.pdf";

        using (var reader = new PdfReader(pdfPath))
        using (var fs = new FileStream(filledPdfPath, FileMode.Create, FileAccess.Write))
        using (var stamper = new PdfStamper(reader, fs))
        {
            var form = stamper.AcroFields;
            form.SetField("Name", "John Doe");
            form.SetField("Email", "john.doe@example.com");
            form.SetField("Phone", "+1234567890");

            // ✅ Check the checkboxes
            form.SetField("AcceptTerms", "Yes");
            form.SetField("Subscribe", "Yes");

            stamper.FormFlattening = false; // Set to true to make fields read-only
        }

        System.Console.WriteLine("Filled form saved: " + Path.GetFullPath(filledPdfPath));
    }
}
